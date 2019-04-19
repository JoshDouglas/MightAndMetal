using System.Collections;
using System.Collections.Generic;
using Entitas.Unity;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class EnvironmentView : View
{
	private List<Sprite>                       _sprites;
	private Dictionary<string, SpriteRenderer> _spriteRenderers;

	public override void Link(GameEntity entity)
	{
		if (gameObject.GetComponent<EntityLink>() == null)
			gameObject.Link(entity);

		//parent
		transform.position   = entity.position.value;
		transform.localScale = entity.scale.value;

		//sprite data
		if (entity.hasResources)
		{
			foreach (var resource in entity.resources.sprites)
			{
				var sprite = AtlasManager.EnvironmentInstance.GetSprite(resource);
				if (sprite != null)
					_sprites.Add(AtlasManager.EnvironmentInstance.GetSprite(resource));
				else
					Debug.LogWarning($"missing sprite: {resource}");
			}
		}

		//setup sprite renderers
		for (var i = 0; i < _sprites.Count; i++)
		{
			//setup go
			var name = $"environment_{transform.position.x}_{transform.position.y}_{_sprites[i].name}";
			var go   = new GameObject(name);
			go.transform.parent                                 = transform;
			go.transform.position                               = transform.position;
			_spriteRenderers[_sprites[i].name]                  = go.AddComponent<SpriteRenderer>();
			_spriteRenderers[_sprites[i].name].sprite           = _sprites[i];
			_spriteRenderers[_sprites[i].name].sortingOrder     = i;
			_spriteRenderers[_sprites[i].name].sortingLayerName = "environment";
		}
	}

	private void Awake()
	{
		//init
		_sprites         = new List<Sprite>();
		_spriteRenderers = new Dictionary<string, SpriteRenderer>();
	}
}
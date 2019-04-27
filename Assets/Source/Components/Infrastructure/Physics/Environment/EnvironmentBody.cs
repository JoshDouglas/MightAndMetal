using Entitas.Unity;
using Entitas.VisualDebugging.Unity;
using UnityEngine;

public class EnvironmentBody : PhysicalBody
{
	private BoxCollider2D _collider;
	public  GameEntity    _entity => (GameEntity) GetComponent<EntityLink>()?.entity;

	public override void Link(GameEntity entity)
	{
		if (!entity.hasPosition)
			gameObject.DestroyGameObject();

		if (gameObject.GetComponent<EntityLink>() == null)
			gameObject.Link(entity);

		if (entity.isBlockable)
			_collider = gameObject.AddComponent<BoxCollider2D>();
		/*_collider.size   = new Vector2(1f, 0.3f);
		_collider.offset = new Vector2(0f, -0.8f);*/
	}

	void Start()
	{
	}

	public override void Update()
	{
	}
}
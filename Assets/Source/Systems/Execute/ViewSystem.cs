using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;
using UnityEngine.U2D;


public class ViewSystem : IExecuteSystem
{
	readonly Contexts  _contexts;
	private  Transform _environmentParent;
	private  Transform _characterParent;

	public ViewSystem(Contexts contexts)
	{
		_contexts          = contexts;
		_environmentParent = GameObject.Find("Environment")?.transform ?? new GameObject("Environment").transform;
		_characterParent   = GameObject.Find("Characters")?.transform  ?? new GameObject("Characters").transform;
	}

	public void Execute()
	{
		//environment
		var floors = _contexts.game.GetGroup(GameMatcher.Floor);
		foreach (var floor in floors)
		{
			if (!floor.hasView)
				floor.InitEnvironmentView(_environmentParent);
		}

		//characters
		var characters = _contexts.game.GetGroup(GameMatcher.Character);
		foreach (var character in characters)
		{
			if (!character.hasView)
				character.InitCharacterView(_characterParent);
		}
	}
}

public static class GameEntityViewExtensions
{
	public static IView InitEnvironmentView(this GameEntity entity, Transform parent)
	{
		var name = $"floor_{entity.position.value.x}_{entity.position.value.y}";
		var go   = GameObject.Find(name) ?? new GameObject(name);
		go.transform.parent = parent;
		var view = go.AddComponent<EnvironmentView>();
		view.Link(entity);
		entity.AddView(view);

		return view;
	}

	public static IView InitCharacterView(this GameEntity entity, Transform parent)
	{
		var name = $"character_{entity.character.Name}";
		if (entity.isPlayerControlled)
			name = "player";
		var go = GameObject.Find(name) ?? new GameObject(name);
		go.transform.parent = parent;
		var view = go.AddComponent<CharacterView>();
		view.Link(entity);
		entity.AddView(view);

		return view;
	}
}
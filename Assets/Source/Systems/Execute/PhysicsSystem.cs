using Entitas;
using UnityEngine;

public class PhysicsSystem : IExecuteSystem
{
	readonly Contexts _contexts;
	private Transform _environmentParent;
	private Transform _characterParent;

	public PhysicsSystem(Contexts contexts)
	{
		_contexts = contexts;
		_environmentParent = GameObject.Find("Environment")?.transform ?? new GameObject("Environment").transform;
		_characterParent = GameObject.Find("Characters")?.transform ?? new GameObject("Characters").transform;
	}

	public void Execute()
	{
		//environment
		var floors = _contexts.game.GetGroup(GameMatcher.Floor);
		foreach (var floor in floors)
		{
			if (floor.isBlockable && !floor.hasPhysicalBody)
				floor.InitEnvironmentBody(_environmentParent);
		}

		//characters
		var characters = _contexts.game.GetGroup(GameMatcher.Character);
		foreach (var character in characters)
		{
			if (!character.hasPhysicalBody)
				character.InitCharacterBody(_characterParent);
			else if (character.hasPhysicalBody && character.hasPosition)
			{
				var characterBody = (CharacterBody) character.physicalBody.physicalBody;
				if (characterBody.RigidBody.position != character.position.value)
					character.ReplacePosition(characterBody.RigidBody.position);
			}
		}
	}

	public void ExecuteOld()
	{
		//todo: abilities
		var characters = _contexts.game.GetGroup(GameMatcher.Character).GetEntities();
		foreach (var entity in characters)
		{
			if (!entity.hasView)
				continue;


			var view = (CharacterView) entity.view.view;
			var rb   = view.GetComponent<Rigidbody2D>();

			if (entity.hasActing)
			{
				/*rb.velocity = Vector2.zero;*/

				if (entity.hasVelocity)
					entity.RemoveVelocity();

				continue;
			}

			//todo: this makes it feel more like character and less rocket ship
			var speed = entity.input.direction * entity.character.Speed;

			/*if (rb.velocity != speed)*/
			if (!entity.hasVelocity || entity.velocity.value != speed)
				entity.ReplaceVelocity(speed);

			if (speed == Vector2.zero && entity.hasVelocity)
				entity.RemoveVelocity();

			if (entity.hasPosition && entity.position.value != rb.position)
				entity.ReplacePosition(rb.position);
			else if (!entity.hasPosition)
				entity.AddPosition(rb.position);

			if (entity.hasScale && entity.scale.value != (Vector2) view.transform.localScale)
				entity.ReplaceScale(view.transform.localScale);
			else if (!entity.hasScale)
				entity.AddScale(view.transform.localScale);

			if (entity.hasVelocity && entity.velocity.value != rb.velocity)
				entity.ReplaceVelocity(rb.velocity);
			else if (!entity.hasVelocity && speed != Vector2.zero)
				entity.AddVelocity(rb.velocity);
		}
	}
}

public static class GameEntityBodyExtensions
{
	public static IPhysicalBody InitEnvironmentBody(this GameEntity entity, Transform parent)
	{
		var name = $"floor_{entity.position.value.x}_{entity.position.value.y}";
		
		var go = GameObject.Find(name) ?? new GameObject(name);
		go.transform.parent = parent;
		
		var physicalBody = go.AddComponent<PhysicalBody>();
		physicalBody.Link(entity);
		entity.AddPhysicalBody(physicalBody);

		return physicalBody;
	}

	public static IPhysicalBody InitCharacterBody(this GameEntity entity, Transform parent)
	{
		var name = $"character_{entity.character.Name}";
		if (entity.isPlayerControlled)
			name = "player";
		
		var go = GameObject.Find(name) ?? new GameObject(name);
		go.transform.parent = parent;
		
		var physicalBody = go.AddComponent<CharacterBody>();
		physicalBody.Link(entity);
		entity.AddPhysicalBody(physicalBody);

		return physicalBody;
	}
}
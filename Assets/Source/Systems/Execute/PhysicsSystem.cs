using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class PhysicsSystem : ReactiveSystem<GameEntity>
{
	readonly Contexts  _contexts;
	private  Transform _environmentParent;
	private  Transform _characterParent;

	public PhysicsSystem(Contexts contexts) : base(contexts.game)
	{
		_contexts          = contexts;
		_environmentParent = GameObject.Find("Environment")?.transform ?? new GameObject("Environment").transform;
		_characterParent   = GameObject.Find("Characters")?.transform  ?? new GameObject("Characters").transform;
	}

	protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) => context.CreateCollector(GameMatcher.AnyOf(GameMatcher.Input, GameMatcher.Floor));

	protected override bool Filter(GameEntity entity) => !entity.hasPhysicalBody;

	protected override void Execute(List<GameEntity> entities)
	{
		foreach (var e in entities)
		{
			if (e.isFloor)
				e.InitEnvironmentBody(_environmentParent);
			else if (e.hasCharacter)
				e.InitCharacterBody(_characterParent);
		}
	}
}

/*public class PhysicsSystem : IExecuteSystem
{
	readonly Contexts  _contexts;
	private  Transform _environmentParent;
	private  Transform _characterParent;

	public PhysicsSystem(Contexts contexts)
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
			if (floor.isBlockable && !floor.hasPhysicalBody)
				floor.InitEnvironmentBody(_environmentParent);
		}

		//characters
		var characters = _contexts.game.GetGroup(GameMatcher.Character);
		foreach (var character in characters)
		{
			if (!character.hasPhysicalBody)
				character.InitCharacterBody(_characterParent);
			/*else if (character.hasPhysicalBody && character.hasPosition)#1#
			else if (character.hasPhysicalBody && character.hasCombatPhysics)
			{
				var characterBody = (CharacterBody) character.physicalBody.physicalBody;
				if (characterBody.RigidBody.position != character.combatPhysics.position)
					character.ReplaceCombatPhysicsPosition(characterBody.RigidBody.position);
				/*character.ReplacePosition(characterBody.RigidBody.position);#1#
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


			var view = (CharacterAnimatorView) entity.view.view;
			var rb   = view.GetComponent<Rigidbody2D>();

			if (entity.hasCombatAction)
				continue;

			//todo: this makes it feel more like character and less rocket ship
			var speed = entity.input.direction * entity.combat.currentSpeed;

			/*if (rb.velocity != speed)#1#
			if (entity.combatPhysics.velocity != speed && speed != Vector2.zero)
				entity.ReplaceCombatPhysicsVelocity(speed);

			if (entity.combatPhysics.position != rb.position)
				entity.ReplaceCombatPhysicsPosition(rb.position);

			/*if (!entity.hasVelocity || entity.velocity.value != speed)
				entity.ReplaceVelocity(speed);

			if (speed == Vector2.zero && entity.hasVelocity)
				entity.RemoveVelocity();#1#

			/*if (entity.hasPosition && entity.position.value != rb.position)
				entity.ReplacePosition(rb.position);
			else if (!entity.hasPosition)
				entity.AddPosition(rb.position);#1#

			/*if (entity.hasScale && entity.scale.value != (Vector2) view.transform.localScale)
				entity.ReplaceScale(view.transform.localScale);
			else if (!entity.hasScale)
				entity.AddScale(view.transform.localScale);#1#

			/*if (entity.hasVelocity && entity.velocity.value != rb.velocity)
				entity.ReplaceVelocity(rb.velocity);
			else if (!entity.hasVelocity && speed != Vector2.zero)
				entity.AddVelocity(rb.velocity);#1#
		}
	}
}*/

public static class GameEntityBodyExtensions
{
	public static IPhysicalBody InitEnvironmentBody(this GameEntity entity, Transform parent)
	{
		var name = $"floor_{entity.position.value.x}_{entity.position.value.y}";

		var go = GameObject.Find(name) ?? new GameObject(name);
		go.transform.parent = parent;
		go.tag              = "environment";

		var environmentBody = go.AddComponent<EnvironmentBody>();
		environmentBody.Link(entity);
		entity.AddPhysicalBody(environmentBody);

		return environmentBody;
	}

	public static IPhysicalBody InitCharacterBody(this GameEntity entity, Transform parent)
	{
		var name = $"character_{entity.character.name}";
		if (entity.isPlayerControlled)
			name = "player";

		var go = GameObject.Find(name) ?? new GameObject(name);
		go.transform.parent = parent;
		go.tag              = "character";

		var characterBody = go.AddComponent<CharacterBody>();
		characterBody.Link(entity);
		entity.AddPhysicalBody(characterBody);

		//init weapon body with character body as parent
		entity.InitCharacterWeaponBody(go.transform);

		return characterBody;
	}

	public static IPhysicalBody InitCharacterWeaponBody(this GameEntity entity, Transform parent)
	{
		var name = $"character_{entity.character.name}_weapon";
		if (entity.isPlayerControlled)
			name = "player_weapon";

		var go = GameObject.Find(name) ?? new GameObject(name);
		go.transform.parent = parent;
		go.tag              = "weapon";

		var weaponBody = go.AddComponent<CharacterWeaponBody>();
		weaponBody.Link(entity);
		entity.AddWeaponBody(weaponBody);

		return weaponBody;
	}
}
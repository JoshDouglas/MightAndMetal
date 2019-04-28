#region

using Entitas.Unity;
using Entitas.VisualDebugging.Unity;
using UnityEngine;

#endregion

public class CharacterBody : PhysicalBody
{
	public  Rigidbody2D   RigidBody { get; private set; }
	public  BoxCollider2D Collider  { get; private set; }
	private GameEntity    _entity   => (GameEntity) GetComponent<EntityLink>()?.entity;

	public override void Link(GameEntity entity)
	{
		if (!entity.hasPosition)
			gameObject.DestroyGameObject();

		if (gameObject.GetComponent<EntityLink>() == null)
			gameObject.Link(entity);

		entity.AddCombatActionListener(this);
		entity.AddCombatActionRemovedListener(this);
		entity.AddVelocityListener(this);
		entity.AddDefeatedListener(this);

		Collider        = gameObject.AddComponent<BoxCollider2D>();
		Collider.offset = new Vector2(0f, -0.8f);
		Collider.size   = new Vector2(1f, 0.7f);

		RigidBody                        = gameObject.AddComponent<Rigidbody2D>();
		RigidBody.freezeRotation         = true;
		RigidBody.gravityScale           = 0f;
		RigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		RigidBody.drag                   = 0f;
	}

	// Start is called before the first frame update
	private void Start()
	{
	}

	// Update is called once per frame
	public override void Update()
	{
		/*
		 * todo: sync up position, scale, and possibly velocity
		 */
		/*if (!_entity.hasVelocity && !_entity.hasCombatAction && !_entity.isDefeated)
			_entity.AddVelocity(RigidBody.velocity);*/
		/*if (!_entity.hasCombatAction && !_entity.isDefeated)
			_entity.ReplaceCombatPhysicsVelocity(RigidBody.velocity);*/
		_entity.position.value = RigidBody.position;
	}

	public override void OnCombatAction(GameEntity entity, CombatAction action, float elapsed)
	{
		RigidBody.velocity = Vector2.zero;
	}

	public override void OnCombatActionRemoved(GameEntity entity)
	{
	}

	public override void OnVelocity(GameEntity entity, Vector2 value)
	{
		/*if (!entity.hasCombatAction && !entity.isDefeated)*/
		RigidBody.velocity = value;
	}

	public override void OnDefeated(GameEntity entity)
	{
		Collider.enabled = false;
		Destroy(RigidBody);
		enabled = false;
	}
}
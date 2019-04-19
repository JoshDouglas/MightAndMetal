using System.Collections;
using System.Collections.Generic;
using Entitas;
using Entitas.Unity;
using Entitas.VisualDebugging.Unity;
using UnityEngine;

public class CharacterBody : PhysicalBody
{
	public Rigidbody2D RigidBody { get; private set; }
	public GameEntity  Entity    => (GameEntity) GetComponent<EntityLink>()?.entity;

	public override void Link(GameEntity entity)
	{
		if (!entity.hasPosition || !entity.hasScale)
			gameObject.DestroyGameObject();

		if (gameObject.GetComponent<EntityLink>() == null)
			gameObject.Link(entity);

		entity.AddActingListener(this);
		entity.AddActingRemovedListener(this);
		entity.AddVelocityListener(this);
		entity.AddVelocityRemovedListener(this);

		_collider        = gameObject.AddComponent<BoxCollider2D>();
		_collider.size   = new Vector2(1f, 0.3f);
		_collider.offset = new Vector2(0f, -0.8f);

		RigidBody                        = gameObject.AddComponent<Rigidbody2D>();
		RigidBody.freezeRotation         = true;
		RigidBody.gravityScale           = 0f;
		RigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		RigidBody.drag                   = 0f;
	}

	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	public override void Update()
	{
		/*
		 * todo: sync up position, scale, and possibly velocity
		 */
		if (!Entity.hasVelocity && !Entity.hasActing && !Entity.isDefeated)
			Entity.AddVelocity(RigidBody.velocity);
	}

	public override void OnActing(GameEntity entity, float duration, float elapsed, CombatAbility combatAbility)
	{
		RigidBody.velocity = Vector2.zero;
	}

	public override void OnActingRemoved(GameEntity entity)
	{
		/*if (!entity.hasInput || entity.input.direction != Vector2.zero)
			return;*/
	}

	public override void OnVelocity(GameEntity entity, Vector2 value)
	{
		if (entity.hasActing)
			return;

		RigidBody.velocity = value;
	}

	public override void OnVelocityRemoved(GameEntity entity)
	{
		if (entity.hasActing)
			return;

		RigidBody.velocity = Vector2.zero;
	}


	private void OnCollisionEnter2D(Collision2D other)
	{
		/*var rb = gameObject.GetComponent<Rigidbody2D>();
		rb.velocity = Vector2.zero;*/
		if (other.gameObject.name != "player" && other.gameObject.GetComponent<EntityLink>() != null)
		{
			var thisEntityLink = gameObject.GetComponent<EntityLink>();
			var entity         = (GameEntity) thisEntityLink.entity;

			var entityLink  = other.gameObject.GetComponent<EntityLink>();
			var otherEntity = (GameEntity) entityLink.entity;

			if (!entity.hasFaction || !otherEntity.hasFaction)
				return;
			
			if (entity.faction.name != otherEntity.faction.name)
			{
				if (otherEntity.hasActing)
					otherEntity.RemoveActing();

				//TODO: this is ugly
				var otherBody = other.gameObject.GetComponent<CharacterBody>();
				if (otherBody != null)
				{
					otherBody.RigidBody.mass = 2.5f;
					otherBody.RigidBody.drag = 2.5f;
				}

				otherEntity.isDefeated = true;

				if (otherEntity.hasVelocity)
					otherEntity.RemoveVelocity();
			}
		}
	}
}
#region

using System.Collections.Generic;
using Entitas.Unity;
using Entitas.VisualDebugging.Unity;
using UnityEngine;

#endregion

public class CharacterWeaponBody : PhysicalBody
{
	private bool             _acting;
	private float            _actionElapsed;
	private float            _disableAt;
	private float            _enableAt;
	private CharacterHeading _heading;
	public  BoxCollider2D    Collider { get; private set; }
	private GameEntity       _entity  => (GameEntity) GetComponent<EntityLink>()?.entity;

	public override void Link(GameEntity entity)
	{
		if (!entity.hasPosition)
			gameObject.DestroyGameObject();

		if (gameObject.GetComponent<EntityLink>() == null)
			gameObject.Link(entity);

		entity.AddCombatActionListener(this);
		entity.AddCombatActionRemovedListener(this);
		entity.AddVelocityListener(this);

		Collider           = gameObject.AddComponent<BoxCollider2D>();
		Collider.isTrigger = true;
		Collider.enabled   = false;
	}

	private void Start()
	{
	}

	private void Awake()
	{
	}

	public override void Update()
	{
		if (!_acting)
			return;

		_actionElapsed   += Time.deltaTime;
		Collider.enabled =  _actionElapsed >= _enableAt && _actionElapsed <= _disableAt;
	}

	public override void OnCombatAction(GameEntity entity, CombatAction action, float elapsed)
	{
		if (!entity.hasWeapon)
			return;
		if (action.abiltyType != CombatAbilityType.Melee)
			return;

		var weapon       = entity.weapon;
		var colliderInfo = weapon.weaponColliders[_heading];

		Collider.offset = colliderInfo.offset;
		Collider.size   = colliderInfo.size;

		_acting        = true;
		_actionElapsed = 0f;
		_enableAt      = weapon.colliderEnabledAt;
		_disableAt     = weapon.colliderDisabledAt;
	}

	public override void OnCombatActionRemoved(GameEntity entity)
	{
		_acting          = false;
		Collider.enabled = false;
	}

	public override void OnVelocity(GameEntity entity, Vector2 value)
	{
		if (value != Vector2.zero && !entity.hasKnockback)
			_heading = entity.heading;
	}

	private void ProcessWeaponCollision(Collider2D other)
	{
		var entity      = (GameEntity) gameObject.GetComponent<EntityLink>()?.entity;
		var otherEntity = (GameEntity) other.gameObject.GetComponent<EntityLink>()?.entity;

		var isAlly       = entity.hasFaction && otherEntity.hasFaction && entity.faction.name == otherEntity.faction.name;
		var combatEvents = new List<CombatEvent>();

		if (other.gameObject.CompareTag("weapon"))
		{
			var bindEvent = new CombatEvent {other = otherEntity, action = null, type = CombatEventType.weaponHit};
			combatEvents.Add(bindEvent);
		}

		if (other.gameObject.CompareTag("character") && !isAlly)
		{
			var action   = entity.hasCombatAction ? entity.combatAction.action : null;
			var hitEvent = new CombatEvent {other = otherEntity, action = action, type = CombatEventType.bodyHit};
			combatEvents.Add(hitEvent);

			/*if (otherEntity.hasCombatAction)
				otherEntity.RemoveCombatAction();

			var otherBody = other.gameObject.GetComponent<CharacterBody>();
			if (otherBody != null)
			{
				otherBody.RigidBody.mass = 2.5f;
				otherBody.RigidBody.drag = 2.5f;
			}

			otherEntity.isDefeated = true;

			if (otherEntity.hasVelocity)
				otherEntity.ReplaceVelocity(Vector2.zero);*/
		}

		if (combatEvents.Count > 0 && entity.hasCombatEvents)
			entity.combatEvents.value.AddRange(combatEvents);
		else if (combatEvents.Count > 0)
			entity.AddCombatEvents(combatEvents);
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		ProcessWeaponCollision(other);
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		ProcessWeaponCollision(other);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		ProcessWeaponCollision(other);
	}
}
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Entitas.Unity;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// sprites = actual resources (texture/sprite)
/// animations = set of sprites for an animation
/// renderers = swap sprites based in current animation
/// </summary>
public class CharacterView : View
{
	private CharacterAnimationHeading _heading;
	private GameEntity                _entity => (GameEntity) GetComponent<EntityLink>()?.entity;

	/*private CharacterAnimator _characterAnimator;*/
	private SingleSpriteCharacterAnimator _characterAnimator;

	public override void Link(GameEntity entity)
	{
		if (gameObject.GetComponent<EntityLink>() == null)
			gameObject.Link(entity);

		entity.AddActingListener(this);
		entity.AddActingRemovedListener(this);
		entity.AddVelocityListener(this);
		entity.AddVelocityRemovedListener(this);
		entity.AddDefeatedListener(this);

		//parent
		transform.position   = _entity.position.value;
		transform.localScale = _entity.scale.value;

		/*_characterAnimator = gameObject.AddComponent<CharacterAnimator>();*/
		_characterAnimator = gameObject.AddComponent<SingleSpriteCharacterAnimator>();
	}

	private void FixedUpdate()
	{
		/*if (_input != null && _input.actionButton9)
		{
			_characterAnimator.Play($"hu_{_direction.ToSelectorString()}");
			return;
		}
		
		if (_input != null && _input.actionButton8)
		{
			_characterAnimator.Play($"sc_{_direction.ToSelectorString()}");
			return;
		}*/
	}

	public override void Update()
	{
		/*if (_entity.hasActing) //abilities
		{
			if (_entity.character.EquipmentTemplate == EquipmentTemplate.Leather)
				_characterAnimator.Play($"sh_{_direction.ToSelectorString()}");
			else if (_entity.character.EquipmentTemplate == EquipmentTemplate.Mail || _entity.character.EquipmentTemplate == EquipmentTemplate.Plate)
				_characterAnimator.Play($"sl_{_direction.ToSelectorString()}");
			else if (_entity.character.EquipmentTemplate == EquipmentTemplate.Cloth)
				_characterAnimator.Play($"sc_{_direction.ToSelectorString()}");
		}
		else if (_entity.velocity.value != Vector2.zero) //movement
		{
			if (_rigidBody.velocity.x > 0)
				_direction = CharacterAnimationDirection.Right;
			else if (_rigidBody.velocity.x < 0)
				_direction = CharacterAnimationDirection.Left;
			else if (_rigidBody.velocity.y > 0)
				_direction = CharacterAnimationDirection.Up;
			else if (_rigidBody.velocity.y < 0)
				_direction = CharacterAnimationDirection.Down;

			//render
			_characterAnimator.Play($"wc_{_direction.ToSelectorString()}");
		}*/
		/*else
			_characterAnimator.Play($"i_{_direction.ToSelectorString()}");*/

		if (_entity.isPlayerControlled)
		{
			var     target = gameObject;
			Vector3 posNoZ = transform.position;
			posNoZ.z = target.transform.position.z;

			Vector3 targetDirection = (target.transform.position - posNoZ);

			var interpVelocity = targetDirection.magnitude * 5f;

			var targetPos = transform.position + (targetDirection.normalized * interpVelocity * Time.deltaTime);
			var offset    = new Vector3(0, 0, -10);

			Camera.main.transform.position = Vector3.Lerp(transform.position, targetPos + offset, 0.25f);
		}
	}

	public override void OnActing(GameEntity entity, float duration, float elapsed, CombatAbility combatAbility)
	{
		var animationKey = $"{combatAbility.AnimationAction.ToSelectorString()}_{_heading.ToSelectorString()}";
		_characterAnimator.Play(animationKey);
	}

	public override void OnActingRemoved(GameEntity entity)
	{
		if (!entity.hasInput || entity.input.direction != Vector2.zero)
			return;

		var animationKey = $"{CharacterAnimationAction.Idle.ToSelectorString()}_{_heading.ToSelectorString()}";
		_characterAnimator.Play(animationKey);
	}

	public override void OnVelocity(GameEntity entity, Vector2 value)
	{
		if (entity.hasActing)
			return;

		SetHeading(value);

		var animationKey = $"{CharacterAnimationAction.Walk.ToSelectorString()}_{_heading.ToSelectorString()}";
		_characterAnimator.Play(animationKey);
	}

	public override void OnVelocityRemoved(GameEntity entity)
	{
		if (entity.hasActing || entity.isDefeated)
			return;

		var animationKey = $"{CharacterAnimationAction.Idle.ToSelectorString()}_{_heading.ToSelectorString()}";
		_characterAnimator.Play(animationKey);
	}

	public override void OnDefeated(GameEntity entity)
	{
		if (!entity.isDefeated)
			return;

		Debug.Log(gameObject.name + " was defeated");
		var animationKey = $"{CharacterAnimationAction.Hurt.ToSelectorString()}_{CharacterAnimationHeading.Down.ToSelectorString()}";
		_characterAnimator.Play(animationKey);
	}

	private void SetHeading(Vector2 velocity)
	{
		if (velocity.x > 0)
			_heading = CharacterAnimationHeading.Right;
		else if (velocity.x < 0)
			_heading = CharacterAnimationHeading.Left;
		else if (velocity.y > 0)
			_heading = CharacterAnimationHeading.Up;
		else if (velocity.y < 0)
			_heading = CharacterAnimationHeading.Down;
	}

	private void Start()
	{
	}

	private void Awake()
	{
	}
}
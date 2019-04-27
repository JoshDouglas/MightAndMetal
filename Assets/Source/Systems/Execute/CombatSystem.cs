#region

using System;
using Entitas;
using UnityEngine;

#endregion

public class CombatSystem : IExecuteSystem
{
	private readonly Contexts _contexts;

	public CombatSystem(Contexts contexts)
	{
		_contexts = contexts;
	}

	public void Execute()
	{
		var characters = _contexts.game.GetGroup(GameMatcher.Character).GetEntities();

		foreach (var entity in characters)
		{
			if (entity.isDefeated)
				continue;

			ProcessCombatAction(entity);
			ProcessKnockback(entity);
			ProcessStunned(entity);
			ProcessInvincible(entity);
			ProcessCombatEvents(entity);
			ProcessInput(entity);
		}
	}

	private void ProcessInput(GameEntity entity)
	{
		if (entity.hasCombatAction || entity.hasStunned)
			return;

		var          character    = entity.character;
		var          input        = entity.input;
		CombatAction combatAction = null;
		CombatItem   combatItem   = null;

		if (input.actionButton1 && character.abilities.Count > 0)
			combatAction = character.abilities[0];
		else if (input.actionButton2 && character.abilities.Count > 1)
			combatAction = character.abilities[1];
		else if (input.actionButton3 && character.abilities.Count > 2)
			combatAction = character.abilities[2];
		else if (input.actionButton4 && character.abilities.Count > 3)
			combatAction = character.abilities[3];
		else if (input.actionButton5 && character.abilities.Count > 4)
			combatAction = character.abilities[4];
		else if (input.actionButton6 && character.abilities.Count > 5)
			combatAction = character.abilities[5];
		else if (input.actionButton7 && character.abilities.Count > 6)
			combatAction = character.abilities[6];
		else if (input.actionButton8 && character.abilities.Count > 7)
			combatAction = character.abilities[7];
		else if (input.actionButton9 && character.abilities.Count > 8)
			combatAction = character.abilities[8];
		else if (!entity.hasKnockback)
		{
			var speed = input.direction * entity.combat.currentSpeed;

			//only allow movement on one axis at a time
			/*if (speed.x != 0 && speed.y != 0)
				speed.y = 0;*/

			if (entity.velocity.value != speed)
				entity.ReplaceVelocity(speed);
		}


		if (combatAction != null)
		{
			//set their heading before acting to allow them to change direction while spamming
			entity.ReplaceVelocity(input.direction * .1f);
			entity.AddCombatAction(combatAction, 0);
		}
	}

	private void ProcessCombatAction(GameEntity entity)
	{
		if (!entity.hasCombatAction)
			return;
		entity.combatAction.elapsed += Time.deltaTime;

		if (entity.combatAction.elapsed >= entity.combatAction.action.duration)
			entity.RemoveCombatAction();
	}

	private void ProcessKnockback(GameEntity entity)
	{
		if (!entity.hasKnockback)
			return;
		entity.knockback.elapsed += Time.deltaTime;

		if (entity.velocity.value != entity.knockback.direction)
			entity.ReplaceVelocity(entity.knockback.direction);

		if (entity.knockback.elapsed >= entity.knockback.duration)
		{
			entity.RemoveKnockback();
			entity.ReplaceVelocity(Vector2.zero);
		}
	}

	private void ProcessStunned(GameEntity entity)
	{
		if (!entity.hasStunned)
			return;

		entity.stunned.elapsed += Time.deltaTime;

		if (entity.stunned.elapsed >= entity.stunned.duration)
			entity.RemoveStunned();
	}

	private void ProcessInvincible(GameEntity entity)
	{
		if (!entity.hasInvincible)
			return;

		entity.invincible.elapsed += Time.deltaTime;

		if (entity.invincible.elapsed >= entity.invincible.duration)
			entity.RemoveInvincible();
	}

	private void ProcessCombatEvents(GameEntity entity)
	{
		if (!entity.hasCombatEvents || entity.combatEvents.value == null || entity.hasInvincible)
			return;

		//process all events for this frame, to keep things consistent, but they will have i-frames after
		foreach (var ce in entity.combatEvents.value)
			switch (ce.type)
			{
				case CombatEventType.bodyHit:
					ProcessHit(entity, ce.other, ce.action);
					break;
				case CombatEventType.weaponHit:
					ProcessBind(ce.other, entity);
					break;
				case CombatEventType.shieldHit:
					ProcessBlock(ce.other, entity);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
	}

	private void ProcessHit(GameEntity hitter, GameEntity victim, CombatAction action)
	{
		if (action.effects == null)
			return;

		foreach (var effect in action.effects)
			switch (effect.EffectType)
			{
				case CombatEffectType.Buff:
					break;
				case CombatEffectType.Debuff:
					break;
				case CombatEffectType.Restorative:
					victim.combat.currentHealth += effect.Power;
					break;
				case CombatEffectType.Destructive:
					if (victim.hasInvincible)
						break;
					victim.combat.currentHealth -= effect.Power;
					if (victim.combat.currentHealth <= 0)
						victim.isDefeated = true;
					if (victim.hasCombatAction)
					{
						//ranged abilities - increase cast time
						if (effect.interuptRatio.HasValue)
							victim.combatAction.elapsed -= effect.interuptRatio.Value * victim.combatAction.action.duration;

						//melee abilities - interrupt
						if (victim.combatAction.action.abiltyType == CombatAbilityType.Melee)
							victim.RemoveCombatAction();
					}

					if (!victim.isDefeated)
					{
						victim.ReplaceInvincible(.25f, 0f);
						var direction = (victim.position.value - hitter.position.value).normalized;
						var knockback = 10f;
						victim.ReplaceKnockback(.2f, 0f, direction * knockback);
						victim.ReplaceStunned(.2f, 0f);
					}

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
	}

	private void ProcessBlock(GameEntity attacker, GameEntity blocker)
	{
		attacker.ReplaceKnockback(.05f, 0f, blocker.heading.ToVector2());
		attacker.ReplaceStunned(.5f, 0f);
	}

	private void ProcessBind(GameEntity binder1, GameEntity binder2)
	{
		var direction = (binder1.position.value - binder2.position.value).normalized;
		var knockback = 7f;
		binder1.ReplaceKnockback(.2f, 0f, direction             * knockback);
		binder2.ReplaceKnockback(.2f, 0f, direction * knockback * -1);
	}
}
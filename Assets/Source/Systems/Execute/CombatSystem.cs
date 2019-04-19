using System;
using Entitas;
using UnityEngine;

public class CombatSystem : IExecuteSystem
{
	readonly Contexts _contexts;

	public CombatSystem(Contexts contexts)
	{
		_contexts = contexts;
	}

	public void Execute()
	{
		var characters       = _contexts.game.GetGroup(GameMatcher.Character).GetEntities();
		var actingCharacters = _contexts.game.GetGroup(GameMatcher.Acting).GetEntities();
		
		/*
		 * combat system does the following
		 * 1. process actions
             * a. casting
             * b. projectiles
             * c. melee
		 * 2. process desired movement (if they can move or not)
		 * 3. process collisions
		 * a. action hits
		 * b. blocks (shield on weapon w/ knockback)
		 * c. weapon on weapon (clink knockback)
		 * 
		 */


		//create entities for abilities (weapons, projectiles, etc.)
		/*
		 * abilities work as follows:
		 * 1. entity is created (if casted already)
		 * 2. entity has size/velocity/mass/etc
		 * 3. ability entity last for certain period of time
		 * 4. if collision occurs, take ability object and apply results to victim
		 */

		foreach (var entity in actingCharacters)
		{
			if (!entity.hasActing)
				continue;

			var acting = entity.acting;
			acting.elapsed += Time.deltaTime;
			if (acting.elapsed >= acting.duration)
			{
				//todo: projectiles and shit
				entity.RemoveActing();
			}
		}

		foreach (var entity in characters)
		{
			if (!entity.hasCharacter || !entity.hasInput || entity.hasActing || entity.isDefeated)
				continue;

			var           character     = entity.character;
			var           input         = entity.input;
			CombatAbility combatAbility = null;
			Item          item          = null;

			if (input.actionButton1 && character.CombatAbilities.Count > 0)
				combatAbility = character.CombatAbilities[0];
			else if (input.actionButton2 && character.CombatAbilities.Count > 1)
				combatAbility = character.CombatAbilities[1];
			else if (input.actionButton3 && character.CombatAbilities.Count > 2)
				combatAbility = character.CombatAbilities[2];
			else if (input.actionButton4 && character.CombatAbilities.Count > 3)
				combatAbility = character.CombatAbilities[3];
			else if (input.actionButton5 && character.CombatAbilities.Count > 4)
				combatAbility = character.CombatAbilities[4];
			else if (input.actionButton6 && character.CombatAbilities.Count > 5)
				combatAbility = character.CombatAbilities[5];
			else if (input.actionButton7 && character.CombatAbilities.Count > 6)
				combatAbility = character.CombatAbilities[6];
			else if (input.actionButton8 && character.CombatAbilities.Count > 7)
				combatAbility = character.CombatAbilities[7];
			else if (input.actionButton9 && character.CombatAbilities.Count > 8)
				combatAbility = character.CombatAbilities[8];
			/*else if (input.specialButton1)	
			else if (input.specialButton2)	*/
			else if (input.itemButton1 && character.Items.Count > 0)
				item = character.Items[0];
			else if (input.itemButton2 && character.Items.Count > 1)
				item = character.Items[1];
			else if (input.itemButton3 && character.Items.Count > 2)
				item = character.Items[2];
			else if (input.itemButton4 && character.Items.Count > 3)
				item = character.Items[3];
			else if (input.direction != Vector2.zero)
				entity.ReplaceVelocity(input.direction * character.Speed);
			else if (input.direction == Vector2.zero && entity.hasVelocity)
				entity.RemoveVelocity();


			if (combatAbility != null)
				CreateAbilityEntity(entity, combatAbility);

			if (item != null)
				CreateItemUseEntity(entity, item);
		}


		//channel abilities ready to go?

		//process ability collisions
	}

	private void CreateAbilityEntity(GameEntity character, CombatAbility ability)
	{
		character.AddActing(ability.PerformanceSeconds, 0f, ability);
		/*var ranged = ability.AbiltyType == CombatAbilityType.Spell || ability.AbiltyType == CombatAbilityType.Ranged;
		var melee  = ability.AbiltyType == CombatAbilityType.Melee;


		//melee abilities
		if (melee)
		{
			var abilityEntity = _contexts.game.CreateEntity();
			abilityEntity.isMovable   = false;
			abilityEntity.isBlockable = true;
			abilityEntity.AddPosition(character.position.value);
		}


		//ranged abilities
		foreach (var projectile in ability.CombatProjectiles)
		{
			var abilityEntity = _contexts.game.CreateEntity();
			abilityEntity.isMovable   = true;
			abilityEntity.isBlockable = true;
			abilityEntity.AddVelocity(projectile.Velocity);
			abilityEntity.AddPosition(character.position.value + projectile.SpawnPositionOffset);
		}*/
	}

	private void CreateItemUseEntity(GameEntity character, Item item)
	{
	}
}
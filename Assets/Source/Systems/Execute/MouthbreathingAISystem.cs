#region

using System.Linq;
using Entitas;
using UnityEngine;

#endregion

public class MouthbreathingAISystem : IExecuteSystem
{
	private readonly Contexts _contexts;
	private          float    _elapsedTime;

	public MouthbreathingAISystem(Contexts contexts)
	{
		_contexts = contexts;
	}

	public void Execute()
	{
		//update input every .25 seconds
		_elapsedTime += Time.fixedDeltaTime;
		if (_elapsedTime >= 0.50f)
		{
			var characters = _contexts.game.GetGroup(GameMatcher.Character).GetEntities();

			foreach (var entity in characters)
			{
				if (entity.hasCombatAction || entity.isPlayerControlled)
				{
					entity.input.actionButton1 = false;
					entity.input.direction     = Vector2.zero;
					continue;
				}

				var position         = entity.position.value;
				var combatAbilities  = entity.character.abilities;
				var isRanged         = combatAbilities[0].abiltyType == CombatAbilityType.Ranged || combatAbilities[0].abiltyType == CombatAbilityType.Spell;
				var isHealer         = combatAbilities[0].name == CombatAbilityName.Heal;
				var actionRangeUnits = isRanged ? 10f : 3f;

				//figure out which stupid idiot to heal
				var sickestAlly  = characters.Where(c => !c.isDefeated).Where(sw => IsAlly(entity, sw)).OrderBy(sw => sw.character.health).FirstOrDefault();
				var closestEnemy = characters.Where(c => !c.isDefeated).Where(sw => !IsAlly(entity, sw)).OrderBy(sw => (sw.position.value - position).magnitude).FirstOrDefault();
				var sickestEnemy = characters.Where(c => !c.isDefeated).Where(sw => !IsAlly(entity, sw)).OrderBy(sw => sw.character.health).FirstOrDefault();
				var allyLeader   = characters.Where(c => !c.isDefeated).Where(sw => IsAlly(entity, sw)).SingleOrDefault(sw => sw.hasLeader);

				//lock on target
				GameEntity target = null;
				if (isHealer)
					target = sickestAlly;
				else if (closestEnemy != null)
					target = closestEnemy;
				else if (allyLeader != null)
					target = allyLeader;

				//what should we do?
				if (target == null)
					continue;


				var targetDistance = position - target.position.value;
				var actOnTarget    = targetDistance.magnitude <= actionRangeUnits;
				var moveToTarget   = targetDistance.magnitude > actionRangeUnits;
				var targetIsAlly   = entity.faction.name.Equals(target.faction.name);
				var targetLocation = target.position.value;
				var heading        = targetLocation - position;

				if (moveToTarget)
					entity.input.direction = heading.normalized;

				if (!isRanged && actOnTarget && !targetIsAlly)
				{
					entity.input.direction = heading.normalized;
					entity.input.actionButton1 = true;
				}

				if (isRanged && actOnTarget)
					entity.input.actionButton1 = true;

				/*if (moveToTarget)
				{
					var targetLocation = target.position.value;
					var heading        = targetLocation - position;
					var inputX = heading.x > .5f  ? 1 :
								 heading.x < -.5f ? -1 : 0;
					var inputY = heading.y > 0 ? 1 :
								 heading.y < 0 ? -1 : 0;
					entity.input.direction = new Vector2(inputX, inputY);
				}
				else if (!isRanged && actOnTarget && !targetIsAlly)
					entity.input.actionButton1 = true;
				else if (isRanged && actOnTarget)
					entity.input.actionButton1 = true;
				//follow leader
				else
					entity.input.direction = Vector2.zero;*/
			}

			_elapsedTime = 0f;
		}
	}

	private bool IsAlly(GameEntity character, GameEntity other)
	{
		return character.faction.name.Equals(other.faction.name);
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public enum CombatAbilityName
{
	Swing,
	Shoot,
	Heal,
	Defend
}

public enum CombatAbilityType
{
	Melee,
	Ranged,
	Spell,
	Special
}

public enum CombatAbilityPowerType
{
	Destructive,
	Restorative
}

public class CombatAbility
{
	public bool BlockMelee { get; set; }
	public bool BlockRanged { get; set; }
	public bool BlockSpell { get; set; }
	public bool BlockSpecial { get; set; }
	public bool ReflectMelee { get; set; }
	public bool ReflectRanged { get; set; }
	public bool ReflectSpell { get; set; }
	public bool ReflectSpecial { get; set; }
	public bool Revive { get; set; }
	public bool Knockout { get; set; }
	public CombatAbilityName Name { get; set; }
	public CombatAbilityType AbiltyType { get; set; }
	public string Description { get; set; }
	public int Power { get; set; }
	public CombatAbilityPowerType PowerType { get; set; }
	public Vector2 MeleeColliderSize { get; set; }
	public Vector2 MeleeColliderVelocity { get; set; }
	public List<CombatProjectile> CombatProjectiles { get; set; }
	public List<CombatEffect> MeleeCombatEffects { get; set; }
	public float PerformanceSeconds { get; set; }
	public float CooldownSeconds { get; set; }
	public CharacterAnimationAction AnimationAction { get; set; }
}

public static class CombatAbilities
{
	public static CombatAbility Swing
	{
		get
		{
			return new CombatAbility
			{
						Name = CombatAbilityName.Swing,
						AbiltyType = CombatAbilityType.Melee,
						Description = "Swing your weapon",
						Power = 25,
						PowerType = CombatAbilityPowerType.Destructive,
						MeleeColliderSize = Vector2.one * .5f,
						MeleeColliderVelocity = Vector2.one,
						PerformanceSeconds = .35f,
						CooldownSeconds = 0f,
						AnimationAction = CharacterAnimationAction.Slash
			};
		}
	}

	public static CombatAbility Shoot
	{
		get
		{
			var power = 10;
			var projectile = new CombatProjectile
			{
						Name = $"{CombatAbilityName.Shoot} projectile",
						Duration = 1f,
						IsBlockable = true,
						DestroyOnCollision = true,
						PowerType = CombatAbilityPowerType.Destructive,
						Power = power,
						Size = Vector2.one * .25f,
						Velocity = Vector2.one * 5f
			};

			return new CombatAbility
			{
						Name = CombatAbilityName.Shoot,
						AbiltyType = CombatAbilityType.Ranged,
						Description = "Shoot your weapon",
						Power = power,
						PowerType = CombatAbilityPowerType.Destructive,
						PerformanceSeconds = .5f,
						CooldownSeconds = 0f,
						CombatProjectiles = new List<CombatProjectile> {projectile},
						AnimationAction = CharacterAnimationAction.Shoot
			};
		}
	}

	public static CombatAbility Heal
	{
		get
		{
			var power = 15;
			var projectile = new CombatProjectile
			{
						Name = $"{CombatAbilityName.Heal} projectile",
						Duration = .25f,
						IsBlockable = false,
						DestroyOnCollision = false,
						PowerType = CombatAbilityPowerType.Destructive,
						Power = power,
						Size = Vector2.one * .50f,
						Velocity = Vector2.zero,
			};
			return new CombatAbility
			{
						Name = CombatAbilityName.Heal,
						AbiltyType = CombatAbilityType.Spell,
						Description = $"Restore {power} health",
						Power = power,
						PowerType = CombatAbilityPowerType.Restorative,
						PerformanceSeconds = .75f,
						CooldownSeconds = 0f,
						CombatProjectiles = new List<CombatProjectile> {projectile},
						AnimationAction = CharacterAnimationAction.Spellcast
			};
		}
	}

	public static CombatAbility Defend
	{
		get
		{
			return new CombatAbility
			{
						Name = CombatAbilityName.Defend,
						AbiltyType = CombatAbilityType.Melee,
						Description = $"Block enemy attacks",
						Power = 0,
						PowerType = CombatAbilityPowerType.Destructive,
						PerformanceSeconds = .25f,
						CooldownSeconds = 0f,
						AnimationAction = CharacterAnimationAction.Thrust
			};
		}
	}
}
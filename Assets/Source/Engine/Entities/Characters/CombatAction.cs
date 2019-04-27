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

public class CombatAction
{
	//game state
	public CombatAbilityName      name               { get; set; }
	public string                 description        { get; set; }
	public CombatAbilityType      abiltyType         { get; set; }
	public int                    power              { get; set; }
	public int                    resourceCost              { get; set; }
	public CombatAbilityPowerType powerType          { get; set; }
	public bool                   destroyOnCollision { get; set; }
	public bool                   isBlockable        { get; set; }
	public bool                   isReflectable      { get; set; }
	public float                  duration           { get; set; }
	public float                  cooldownDuration   { get; set; }
	public List<CombatEffect>     effects            { get; set; }

	//animation
	public CharacterAnimationAction action { get; set; }

	//physics
	public Dictionary<CharacterHeading, CombatColliderInfo> abilityColliderInfo;
	public Vector2?                                         projectileVelocity { get; set; }
	public Vector2?                                         projectilePosition { get; set; }
	public Vector2?                                         projectileDrag     { get; set; }
}

public static class CombatAbilities
{
	public static CombatAction Swing =>
		new CombatAction
		{
			name             = CombatAbilityName.Swing,
			abiltyType       = CombatAbilityType.Melee,
			description      = "Swing your weapon",
			power            = 20,
			powerType        = CombatAbilityPowerType.Destructive,
			duration         = .25f,
			cooldownDuration = 0f,
			action           = CharacterAnimationAction.Slash,
			effects = new List<CombatEffect>{ CombatEffects.Get(CombatEffectName.slash)}
		};

	public static CombatAction Shoot =>
		new CombatAction
		{
			name             = CombatAbilityName.Shoot,
			abiltyType       = CombatAbilityType.Ranged,
			description      = "Shoot your weapon",
			power            = 10,
			powerType        = CombatAbilityPowerType.Destructive,
			duration         = .45f,
			cooldownDuration = 0f,
			action           = CharacterAnimationAction.Shoot,
			effects = new List<CombatEffect>{ CombatEffects.Get(CombatEffectName.shoot)}
		};

	public static CombatAction Heal =>
		new CombatAction
		{
			name             = CombatAbilityName.Heal,
			abiltyType       = CombatAbilityType.Spell,
			description      = $"Restore 15 health",
			power            = 15,
			powerType        = CombatAbilityPowerType.Restorative,
			duration         = .60f,
			cooldownDuration = 0f,
			action           = CharacterAnimationAction.Spellcast,
			effects = new List<CombatEffect>{ CombatEffects.Get(CombatEffectName.heal)}
		};

	public static CombatAction Defend =>
		new CombatAction
		{
			name             = CombatAbilityName.Defend,
			abiltyType       = CombatAbilityType.Melee,
			description      = $"Block incoming attacks",
			power            = 5,
			powerType        = CombatAbilityPowerType.Destructive,
			duration         = .35f,
			cooldownDuration = 0f,
			action           = CharacterAnimationAction.Thrust
		};
}
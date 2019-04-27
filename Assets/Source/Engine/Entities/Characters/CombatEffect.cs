using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public enum CombatEffectType
{
	Buff,
	Debuff,
	Restorative,
	Destructive
}

public enum CombatEffectName
{
	slash,
	shoot,
	heal
}

public class CombatEffect 
{
	public CombatEffectName Name { get; set; }
	public CombatEffectType EffectType { get; set; } 
	public float? TickInterval { get; set; }
	public float? Duration { get; set; }
	public bool Stun { get; set; }
	public bool FreezeMovement { get; set; }
	public bool LockCasting { get; set; }
	public float? interuptRatio { get; set; }
	public int Power { get; set; }
	public int Health { get; set; }
	public int Stamina { get; set; }
	public int Defense { get; set; }
	public float Speed { get; set; }
}

public static class CombatEffects
{
	private static Dictionary<CombatEffectName, CombatEffect> _combatEffects;

	static CombatEffects()
	{
		var slash = new CombatEffect
		{
			Name = CombatEffectName.slash,
			EffectType = CombatEffectType.Destructive,
			interuptRatio = .4f,
			Power = 10
		};
		var shoot = new CombatEffect
		{
			Name = CombatEffectName.shoot,
			EffectType = CombatEffectType.Destructive,
			interuptRatio = .2f,
			Power = 10
		};
		var heal = new CombatEffect
		{
			Name = CombatEffectName.heal,
			EffectType = CombatEffectType.Restorative,
			Power = 15
		};

		_combatEffects = new Dictionary<CombatEffectName, CombatEffect> {{CombatEffectName.slash, slash}, {CombatEffectName.shoot, shoot}, {CombatEffectName.heal, heal}};
	}

	public static CombatEffect Get(CombatEffectName combatEffectName)
	{
		return _combatEffects[combatEffectName];
	}
}

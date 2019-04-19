using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public enum CombatAuraType
{
	BuffOverTimeEffect,
	DebuffOverTimeEffect,
	Buff,
	Debuff
}

public class CombatEffect 
{
	public string Name { get; set; }
	public CombatAuraType AuraType { get; set; } 
	public float TickInterval { get; set; }
	public float Duration { get; set; }
	public bool Stun { get; set; }
	public bool FreezeMovement { get; set; }
	public bool LockCasting { get; set; }
	public int Power { get; set; }
	public int Health { get; set; }
	public int Stamina { get; set; }
	public int Defense { get; set; }
	public float Speed { get; set; }
}

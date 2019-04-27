using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatItem 
{
	public string name { get; set; }
	public CombatItemType type { get; set; }
	public string description { get; set; }
	public int cost { get; set; }
	public CharacterAttributes attributes { get; set; }
	public int healthRestore { get; set; }
	public int resourceRestore { get; set; }
	public float? duration { get; set; }
	public float? tickInterval { get; set; }
	public bool revive { get; set; }
	public bool meleeImmune { get; set; }
	public bool spellImmune { get; set; }
}

public enum CombatItemType
{
	standard,
	aura
}

using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class CombatProjectile 
{
	public string Name { get; set; }
	public float Duration { get; set; }
	public bool IsBlockable { get; set; }
	public bool DestroyOnCollision { get; set; }
	public CombatAbilityPowerType PowerType { get; set; }
	public int Power { get; set; }
	public Vector2 SpawnPositionOffset { get; set; }
	public Vector2 Size { get; set; }
	public Vector2 Velocity { get; set; }
	public List<CombatEffect> CombatEffects { get; set; }
}

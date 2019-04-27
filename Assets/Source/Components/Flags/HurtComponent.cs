using System.Collections;
using System.Collections.Generic;
using Entitas;
using Entitas.CodeGeneration.Attributes;

[Event(EventTarget.Self)]
[Event(EventTarget.Self, EventType.Removed)]
public class HurtComponent : IComponent
{
	public float duration;
	public float elapsed;
	public List<CombatWeaponCollision> weaponCollisions;
	public List<CombatProjectileCollision> projectileCollisions;
}

public partial class GameEntity
{
	public void AddHurt()
	{
		AddHurt(.5f, 0f, new List<CombatWeaponCollision>(), new List<CombatProjectileCollision>());
	}
}

public class CombatWeaponCollision
{
	public GameEntity owner { get; set; }
	public CombatAction Action { get; set; }
}

public class CombatProjectileCollision
{
	public GameEntity owner { get; set; }
	public CombatAction Action { get; set; }
}

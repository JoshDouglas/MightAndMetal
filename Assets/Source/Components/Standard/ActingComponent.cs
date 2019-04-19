using System.Collections;
using System.Collections.Generic;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;
using EventType = Entitas.CodeGeneration.Attributes.EventType;

[Event(EventTarget.Self)]
[Event(EventTarget.Self, EventType.Removed)]
public class ActingComponent : IComponent
{
	public float duration;
	public float elapsed;
	public CombatAbility combatAbility;
}

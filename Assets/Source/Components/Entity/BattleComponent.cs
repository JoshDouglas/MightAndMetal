using System;
using System.Collections;
using System.Collections.Generic;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

public class BattleComponent : IComponent
{
	[EntityIndex]
	public Guid id { get; set; } 
}

using System;
using System.Collections;
using System.Collections.Generic;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

public class SquadComponent : IComponent
{
	[EntityIndex]
	public string name { get; set; } 
}

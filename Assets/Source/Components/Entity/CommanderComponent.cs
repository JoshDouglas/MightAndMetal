using System.Collections;
using System.Collections.Generic;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

public class CommanderComponent : IComponent
{
	[EntityIndex]
	public string name { get; set; } 
}

using System;
using System.Collections;
using System.Collections.Generic;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

public class FactionComponent : IComponent
{
	[EntityIndex]
	public string name { get; set; } 
}

﻿using System.Collections;
using System.Collections.Generic;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;
using EventType = Entitas.CodeGeneration.Attributes.EventType;

[Event(EventTarget.Self)]
public class VelocityComponent : IComponent
{
	public Vector2 value;
}


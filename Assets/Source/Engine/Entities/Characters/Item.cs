using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item 
{
	public string Name { get; set; }
	public string Description { get; set; }
	public int Cost { get; set; }
	public int Power { get; set; }
	public int Health { get; set; }
	public int Stamina { get; set; }
	public int Defense { get; set; }
	public float Speed { get; set; }
	public float Duration { get; set; }
	public float TickInterval { get; set; }
	public bool Revive { get; set; }
	public bool HurtImmune { get; set; }
}

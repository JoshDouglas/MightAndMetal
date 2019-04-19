using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class InputComponent : IComponent
{
	public Vector2 direction      { get; set; }
	public bool    actionButton1  { get; set; }
	public bool    actionButton2  { get; set; }
	public bool    actionButton3  { get; set; }
	public bool    actionButton4  { get; set; }
	public bool    actionButton5  { get; set; }
	public bool    actionButton6  { get; set; }
	public bool    actionButton7  { get; set; }
	public bool    actionButton8  { get; set; }
	public bool    actionButton9  { get; set; }
	public bool    itemButton1    { get; set; }
	public bool    itemButton2    { get; set; }
	public bool    itemButton3    { get; set; }
	public bool    itemButton4    { get; set; }
	public bool    specialButton1 { get; set; }
	public bool    specialButton2 { get; set; }
	public bool    sprint         { get; set; }
	public bool    block          { get; set; }
}

public partial class GameEntity
{
	public void AddInput()
	{
		var index     = GameComponentsLookup.Input;
		var component = (InputComponent) CreateComponent(index, typeof(InputComponent));
		component.direction      = Vector2.zero;
		component.actionButton1  = false;
		component.actionButton2  = false;
		component.actionButton3  = false;
		component.actionButton4  = false;
		component.actionButton5  = false;
		component.actionButton6  = false;
		component.actionButton7  = false;
		component.actionButton8  = false;
		component.actionButton9  = false;
		component.itemButton1    = false;
		component.itemButton2    = false;
		component.itemButton3    = false;
		component.itemButton4    = false;
		component.specialButton1 = false;
		component.specialButton2 = false;
		component.sprint         = false;
		component.block          = false;
		AddComponent(index, component);
	}
}
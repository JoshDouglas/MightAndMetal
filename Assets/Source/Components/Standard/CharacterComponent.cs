using System;
using System.Collections.Generic;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;
using EventType = Entitas.CodeGeneration.Attributes.EventType;

public class CharacterComponent : IComponent
{
	public string              name;
	public CharacterAttributes attributes;
	public int                 experience;
	public List<CombatAction>  abilities { get; set; }

	public int   level        => experience == 0 ? 1 : experience                  / 1000;
	public int   health       => attributes?.fortitude                             * 20 ?? 0;
	public int   healthRegen  => (attributes?.fortitude + attributes?.athleticism) * 1  ?? 0;
	public int   stamina      => attributes?.dexterity                             * 10 ?? 0;
	public int   staminaRegen => (attributes?.dexterity + attributes?.athleticism) * 1  ?? 0;
	public int   power        => Math.Max((attributes?.strength ?? 0) * 10, (attributes?.intellect ?? 0) * 10);
	public int   defense      => attributes?.toughness   * 10 ?? 0;
	public float speed        => attributes?.athleticism * 5  ?? 0;
}

[Event(EventTarget.Self)]
[Event(EventTarget.Self, EventType.Removed)]
public class CombatComponent : IComponent
{
	public CharacterAttributes attributes;
	public List<CombatAction>  actions;
	public List<CombatAction>  activeProjectiles;

	public int   maxHealth => attributes?.fortitude * 20 ?? 0;
	public int   currentHealth;
	public int   maxHealthRegen => (attributes?.fortitude + attributes?.athleticism) * 1 ?? 0;
	public int   currentHealthRegen;
	public int   maxStamina => attributes?.dexterity * 10 ?? 0;
	public int   currentStamina;
	public int   maxStaminaRegen => (attributes?.dexterity + attributes?.athleticism) * 1 ?? 0;
	public int   currentStaminaRegen;
	public int   maxPower => Math.Max((attributes?.strength ?? 0) * 10, (attributes?.intellect ?? 0) * 10);
	public int   currentPower;
	public int   maxDefense => attributes?.toughness * 10 ?? 0;
	public int   currentDefense;
	public float maxSpeed => attributes?.athleticism * 2 ?? 0;
	public float currentSpeed;

	public CharacterHeading heading;
	public Vector2 scale;
	public float   mass;
	public float   drag;
}

public class CombatEventsComponent : IComponent
{
	public List<CombatEvent> value;
}

[Event(EventTarget.Self)]
[Event(EventTarget.Self, EventType.Removed)]
public class CombatActionComponent : IComponent
{
	public CombatAction action;
	public float        elapsed;
}

public class EquipmentComponent : IComponent
{
	public Dictionary<EquipmentType, Equipment> equipment;
	public string                               setSelector;
}

public class ItemsComponent : IComponent
{
	public List<CombatItem> items;
}

public class CombatEvent
{
	public GameEntity      other;
	public CombatEventType type;
	public CombatAction    action;
}

public enum CombatEventType
{
	bodyHit,
	weaponHit,
	shieldHit
}

public class CharacterAttributes
{
	//health
	public int fortitude;

	//resources
	public int dexterity;

	//power
	public int strength;

	public int intellect;

	//defense
	public int toughness;

	//speed
	public int athleticism;
}

public partial class GameEntity
{
	public void AddCharacter(string name, CharacterTemplate template)
	{
		var index     = GameComponentsLookup.Character;
		var component = (CharacterComponent) CreateComponent(index, typeof(CharacterComponent));
		component.name = name;
		component.attributes = new CharacterAttributes
		{
			strength    = 5,
			athleticism = 2,
			dexterity   = 3,
			fortitude   = 4,
			intellect   = 0,
			toughness   = 2
		};
		component.experience = 0;


		switch (template)
		{
			case CharacterTemplate.Swordsman:
				component.abilities = new List<CombatAction> {CombatAbilities.Swing, CombatAbilities.Defend};
				break;
			case CharacterTemplate.Archer:
				component.abilities = new List<CombatAction> {CombatAbilities.Shoot};
				break;
			case CharacterTemplate.Cleric:
				component.abilities = new List<CombatAction> {CombatAbilities.Heal};
				break;
			case CharacterTemplate.Captain:
				component.abilities = new List<CombatAction> {CombatAbilities.Swing, CombatAbilities.Defend};
				break;
			case CharacterTemplate.Player:
				component.abilities = new List<CombatAction> {CombatAbilities.Swing, CombatAbilities.Defend};
				break;
		}

		AddComponent(index, component);
	}

	public void AddCombat()
	{
		var index     = GameComponentsLookup.Combat;
		var component = (CombatComponent) CreateComponent(index, typeof(CombatComponent));

		if (!hasCharacter)
			return;

		component.attributes          = character.attributes;
		component.actions             = character.abilities;
		component.activeProjectiles   = new List<CombatAction>();
		component.currentHealth       = component.maxHealth;
		component.currentHealthRegen  = component.maxHealthRegen;
		component.currentStamina      = component.maxStamina;
		component.currentStaminaRegen = component.maxStaminaRegen;
		component.currentPower        = component.maxPower;
		component.currentDefense      = component.maxDefense;
		component.currentSpeed        = component.maxSpeed;

		AddComponent(index, component);
	}

	public CharacterHeading heading
	{
		get
		{
			if (!hasVelocity)
				return CharacterHeading.Down;

			if (velocity.value.y > 0 && velocity.value.x > 0)
				return velocity.value.y > velocity.value.x ? CharacterHeading.Up : CharacterHeading.Right;

			if (velocity.value.y < 0 && velocity.value.x < 0)
				return velocity.value.y < velocity.value.x ? CharacterHeading.Down : CharacterHeading.Left;

			if (velocity.value.x > 0)
				return CharacterHeading.Right;

			if (velocity.value.x < 0)
				return CharacterHeading.Left;

			if (velocity.value.y > 0)
				return CharacterHeading.Up;

			if (velocity.value.y < 0)
				return CharacterHeading.Down;

			return CharacterHeading.Down;
		}
	}

	public void AddCombatEvents()
	{
		var index     = GameComponentsLookup.CombatEvents;
		var component = (CombatEventsComponent) CreateComponent(index, typeof(CombatEventsComponent));

		new List<CombatEvent>();

		AddComponent(index, component);
	}

	public void AddCombatAction()
	{
		var index     = GameComponentsLookup.CombatAction;
		var component = (CombatActionComponent) CreateComponent(index, typeof(CombatActionComponent));

		component.elapsed = 0;
		component.action  = null;

		AddComponent(index, component);
	}

	public void AddEquipment(CharacterTemplate template)
	{
		var index     = GameComponentsLookup.Equipment;
		var component = (EquipmentComponent) CreateComponent(index, typeof(EquipmentComponent));

		component.equipment = new Dictionary<EquipmentType, Equipment>();

		switch (template)
		{
			case CharacterTemplate.Swordsman:
				component.setSelector = "templates_mail";
				break;
			case CharacterTemplate.Archer:
				component.setSelector = "templates_leather";
				break;
			case CharacterTemplate.Cleric:
				component.setSelector = "templates_cloth";
				break;
			case CharacterTemplate.Captain:
				component.setSelector = "templates_plate";
				break;
			case CharacterTemplate.Player:
				component.setSelector = "templates_plate";
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(template), template, null);
		}
		//todo: default equipment

		AddComponent(index, component);
	}

	public void AddItems()
	{
		var index     = GameComponentsLookup.Items;
		var component = (ItemsComponent) CreateComponent(index, typeof(ItemsComponent));

		component.items = new List<CombatItem>();

		AddComponent(index, component);
	}
}
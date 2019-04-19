using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class CharacterComponent : IComponent
{
	public string Name;
	public int Health;
	public int MaxHealth;
	public int Stamina;
	public int MaxStamina;
	public int Power;
	public int MaxPower;
	public int Defense;
	public int MaxDefense;
	public float Speed;
	public float MaxSpeed;
	public List<CombatAbility> CombatAbilities { get; set; }
	public List<Equipment> Equipment { get; set; }
	public EquipmentTemplate EquipmentTemplate { get; set; }
	public List<Item> Items { get; set; }
}

public partial class GameEntity
{
	public void AddCharacter(string name, CharacterTemplate template)
	{
		var index = GameComponentsLookup.Character;
		var component = (CharacterComponent) CreateComponent(index, typeof(CharacterComponent));
		component.Name = name;
		component.Stamina = 100;
		component.MaxStamina = 100;
		component.Power = 5;
		component.MaxPower = 5;
		component.Speed = 4f;
		component.MaxSpeed = 4f;
		
		switch (template)
		{
			case CharacterTemplate.Swordsman:
				component.Health = 150;
				component.MaxHealth = 150;
				component.Defense = 5;
				component.MaxDefense = 5;
				component.CombatAbilities = new List<CombatAbility>{CombatAbilities.Swing};
				component.EquipmentTemplate = EquipmentTemplate.Mail;
				break;
			case CharacterTemplate.Archer:
				component.Health = 90;
				component.MaxHealth = 90;
				component.Defense = 2;
				component.MaxDefense = 2;
				component.CombatAbilities = new List<CombatAbility>{CombatAbilities.Shoot};
				component.EquipmentTemplate = EquipmentTemplate.Leather;
				break;
			case CharacterTemplate.Cleric:
				component.Health = 110;
				component.MaxHealth = 110;
				component.Defense = 1;
				component.MaxDefense = 1;
				component.CombatAbilities = new List<CombatAbility>{CombatAbilities.Heal};
				component.EquipmentTemplate = EquipmentTemplate.Cloth;
				break;
			case CharacterTemplate.Captain:
				component.Health = 250;
				component.MaxHealth = 250;
				component.Defense = 7;
				component.MaxDefense = 7;
				component.CombatAbilities = new List<CombatAbility>{CombatAbilities.Swing, CombatAbilities.Defend};
				component.EquipmentTemplate = EquipmentTemplate.Plate;
				break;
			case CharacterTemplate.Player:
				component.Health = 300;
				component.MaxHealth = 300;
				component.Defense = 8;
				component.MaxDefense = 8;
				component.CombatAbilities = new List<CombatAbility>{CombatAbilities.Swing, CombatAbilities.Defend};
				component.EquipmentTemplate = EquipmentTemplate.Plate;
				break;
		}

		/*component.Equipment = newEquipment;
		component.Items = newItems;*/
		
		AddComponent(index, component);
	}
}

public enum CharacterTemplate
{
	Swordsman,
	Archer,
	Cleric,
	Captain,
	Player
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentTemplate
{
	Cloth,
	Leather,
	Mail,
	Plate
}

public enum EquipmentType
{
	Feet,
	Legs,
	Belt,
	Chest,
	Arms,
	Hands,
	Shoulders,
	Head,
	Back,
	OneHand,
	TwoHand,
	Shield,
	Neck,
	Ring
}

public class Equipment
{
	public string             Name           { get; set; }
	public string             SpriteSelector { get; set; }
	public string             Description    { get; set; }
	public CharacterPiecePart Part           { get; set; }
	public int                Cost           { get; set; }
	public int                Health         { get; set; }
	public int                Stamina        { get; set; }
	public int                Attack         { get; set; }
	public int                Defense        { get; set; }
	public float              Speed          { get; set; }
}

public static class Equipments
{
	private static List<Equipment> _equipment;

	static class Weapons
	{
		private static List<Equipment> _armor;

		static class Swords
		{
		}

		static class Shields
		{
		}

		static class Bows
		{
		}

		static class Wands
		{
		}
	}

	static class Armor
	{
		private static List<Equipment> _weapons;

		static class Feet
		{
		}

		static class Legs
		{
		}

		static class Chest
		{
		}

		static class Arms
		{
		}

		static class Hands
		{
		}

		static class Head
		{
		}
	}
}
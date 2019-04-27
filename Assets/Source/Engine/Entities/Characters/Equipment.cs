using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
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
	public string              name        { get; set; }
	public string              description { get; set; }
	public EquipmentType       type        { get; set; }
	public string              spriteKey   { get; set; }
	public int                 cost        { get; set; }
	public CharacterAttributes attributes  { get; set; }
}

public static class Equipments
{
	private static List<Equipment> _equipment;

	static class Weapons
	{
		private static List<Equipment> _armor;

		static class Swords
		{
			private static Equipment longSword;
			private static Equipment mace;
			private static Equipment rapier;
			private static Equipment saber;

			static Swords()
			{
				longSword = new Equipment {name = "longsword", type = EquipmentType.OneHand, spriteKey = "righthand_male_longsword"};
				mace = new Equipment {name = "mace", type = EquipmentType.OneHand, spriteKey = "righthand_male_mace"};
				rapier = new Equipment {name = "rapier", type = EquipmentType.OneHand, spriteKey = "righthand_male_rapier"};
				saber = new Equipment {name = "saber", type = EquipmentType.OneHand, spriteKey = "righthand_male_saber"};
			}

			public static Equipment LongSword => longSword;

			public static Equipment Mace => mace;

			public static Equipment Rapier => rapier;

			public static Equipment Saber => saber;
		}

		static class Shields
		{
			private static Equipment woodShield;

			static Shields()
			{
				woodShield = new Equipment {name = "wood shield", type = EquipmentType.Shield, spriteKey = "lefthand_male_shield"};
			}

			public static Equipment WoodShield => woodShield;
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
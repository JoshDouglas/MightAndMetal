using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CharacterPiecePart
{
	Body,
	Face,
	Hair,
	Head,
	FacialHair,
	Ears,
	Eyes,
	Nose,
	Neck,
	Chest,
	Shoulder,
	Arms,
	Wrists,
	Hands,
	Back,
	Back2,
	Waist,
	Legs,
	Feet,
	RightHand,
	LeftHand,
	BothHand
}

public static class CharacterPiecePartExtensions
{
	public static string ToSelectorString(this CharacterPiecePart part)
	{
		switch (part)
		{
			case CharacterPiecePart.Body:
				return "body";
			case CharacterPiecePart.Face:
				return "face";
			case CharacterPiecePart.Hair:
				return "hair";
			case CharacterPiecePart.Head:
				return "head";
			case CharacterPiecePart.FacialHair:
				return "facialhair";
			case CharacterPiecePart.Ears:
				return "ears";
			case CharacterPiecePart.Eyes:
				return "eyes";
			case CharacterPiecePart.Nose:
				return "nose";
			case CharacterPiecePart.Neck:
				return "neck";
			case CharacterPiecePart.Chest:
				return "chest";
			case CharacterPiecePart.Shoulder:
				return "shoulder";
			case CharacterPiecePart.Arms:
				return "arms";
			case CharacterPiecePart.Wrists:
				return "wrists";
			case CharacterPiecePart.Hands:
				return "hands";
			case CharacterPiecePart.Back:
				return "back";
			case CharacterPiecePart.Back2:
				return "back2";
			case CharacterPiecePart.Waist:
				return "waist";
			case CharacterPiecePart.Legs:
				return "legs";
			case CharacterPiecePart.Feet:
				return "feet";
			case CharacterPiecePart.RightHand:
				return "righthand";
			case CharacterPiecePart.LeftHand:
				return "lefthand";
			case CharacterPiecePart.BothHand:
				return "bothhand";
			default:
				throw new ArgumentOutOfRangeException(nameof(part), part, null);
		}
	}
}

public enum CharacterPieceGender
{
	male,
	female
}

public static class CharacterPieceGenderExtensions
{
	public static string ToSelectorString(this CharacterPieceGender gender)
	{
		switch (gender)
		{
			case CharacterPieceGender.male:
				return "male";
			case CharacterPieceGender.female:
				return "female";
			default:
				throw new ArgumentOutOfRangeException(nameof(gender), gender, null);
		}
	}
}

public enum CharacterPieceStyle
{
	chainmail,
	leather,
	platemail
}

public static class CharacterPieceStyleExtensions
{
	public static string ToSelectorString(this CharacterPieceStyle style)
	{
		switch (style)
		{
			case CharacterPieceStyle.chainmail:
				return "chainmail";
			case CharacterPieceStyle.leather:
				return "leather";
			case CharacterPieceStyle.platemail:
				return "platemail";
			default:
				throw new ArgumentOutOfRangeException(nameof(style), style, null);
		}
	}
}

public class CharacterPiece
{
	public CharacterPiecePart   part     { get; }
	public CharacterPieceGender gender   { get; }
	public string               style    { get; }
	public Color                color    { get; }
	public string               selector => $"{part.ToSelectorString()}_{gender.ToSelectorString()}_{style}";
	public bool                 enabled  { get; set; }

	public static List<CharacterPiecePart>   Parts   => Enum.GetValues(typeof(CharacterPiecePart)).Cast<CharacterPiecePart>().ToList();
	public static List<CharacterPieceGender> Genders => Enum.GetValues(typeof(CharacterPieceGender)).Cast<CharacterPieceGender>().ToList();
	public static List<CharacterPieceStyle>  Styles  => Enum.GetValues(typeof(CharacterPieceStyle)).Cast<CharacterPieceStyle>().ToList();

	public CharacterPiece(CharacterPiecePart part, CharacterPieceGender gender, string style, Color color)
	{
		this.part    = part;
		this.gender  = gender;
		this.style   = style;
		this.color   = color;
		this.enabled = true;
	}

	public CharacterPiece(CharacterPiecePart part, CharacterPieceGender gender, string style)
	{
		this.part    = part;
		this.gender  = gender;
		this.style   = style;
		this.color   = Color.clear;
		this.enabled = true;
	}

	public CharacterPiece(CharacterPiecePart part, CharacterPieceGender gender, CharacterPieceStyle style, Color color)
	{
		this.part    = part;
		this.gender  = gender;
		this.style   = style.ToSelectorString();
		this.color   = color;
		this.enabled = true;
	}

	public CharacterPiece(CharacterPiecePart part, CharacterPieceGender gender, CharacterPieceStyle style)
	{
		this.part    = part;
		this.gender  = gender;
		this.style   = style.ToSelectorString();
		this.color   = Color.clear;
		this.enabled = true;
	}
}



using System;
using UnityEngine;

public enum CharacterHeading
{
	Up,
	Down,
	Left,
	Right
}

public static class CharacterHeadingExtensions
{
	public static string ToSelectorString(this CharacterHeading heading)
	{
		switch (heading)
		{
			case CharacterHeading.Up:
				return "t";
			case CharacterHeading.Down:
				return "d";
			case CharacterHeading.Left:
				return "l";
			case CharacterHeading.Right:
				return "r";
			default:
				throw new ArgumentOutOfRangeException(nameof(heading), heading, null);
		}
	}
	
	public static Vector2 ToVector2(this CharacterHeading heading)
	{
		switch (heading)
		{
			case CharacterHeading.Up:
				return Vector2.up;
			case CharacterHeading.Down:
				return Vector2.down;
			case CharacterHeading.Left:
				return Vector2.left;
			case CharacterHeading.Right:
				return Vector2.right;
			default:
				throw new ArgumentOutOfRangeException(nameof(heading), heading, null);
		}
	}
}
using System;
using UnityEngine;

public enum CharacterAnimationAction
{
	Walk,
	Slash,
	Shoot,
	Thrust,
	Spellcast,
	Hurt,
	Idle
}

public static class CharacterAnimationActionExtensions
{
	public static string ToSelectorString(this CharacterAnimationAction action)
	{
		switch (action)
		{
			case CharacterAnimationAction.Walk:
				return "wc";
			case CharacterAnimationAction.Slash:
				return "sl";
			case CharacterAnimationAction.Shoot:
				return "sh";
			case CharacterAnimationAction.Thrust:
				return "th";
			case CharacterAnimationAction.Spellcast:
				return "sc";
			case CharacterAnimationAction.Hurt:
				return "hu";
			case CharacterAnimationAction.Idle:
				return "i";
			default:
				throw new ArgumentOutOfRangeException(nameof(action), action, null);
		}
	}
}
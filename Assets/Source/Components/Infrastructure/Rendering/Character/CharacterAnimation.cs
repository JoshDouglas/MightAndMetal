using System.Collections.Generic;

public static class CharacterAnimations
{
	public static Dictionary<string, CharacterAnimation> Animations { get; }
	public static List<CharacterAnimationAction>         Actions    { get; }
	public static List<CharacterHeading>                 Directions { get; }

	public static CharacterAnimation WalkUp   { get; }
	public static CharacterAnimation ShootUp  { get; }
	public static CharacterAnimation SlashUp  { get; }
	public static CharacterAnimation ThrustUp { get; }
	public static CharacterAnimation CastUp   { get; }
	public static CharacterAnimation IdleUp   { get; }

	public static CharacterAnimation WalkDown   { get; }
	public static CharacterAnimation ShootDown  { get; }
	public static CharacterAnimation SlashDown  { get; }
	public static CharacterAnimation ThrustDown { get; }
	public static CharacterAnimation CastDown   { get; }
	public static CharacterAnimation IdleDown   { get; }

	public static CharacterAnimation WalkLeft   { get; }
	public static CharacterAnimation ShootLeft  { get; }
	public static CharacterAnimation SlashLeft  { get; }
	public static CharacterAnimation ThrustLeft { get; }
	public static CharacterAnimation CastLeft   { get; }
	public static CharacterAnimation IdleLeft   { get; }

	public static CharacterAnimation WalkRight   { get; }
	public static CharacterAnimation ShootRight  { get; }
	public static CharacterAnimation SlashRight  { get; }
	public static CharacterAnimation ThrustRight { get; }
	public static CharacterAnimation CastRight   { get; }
	public static CharacterAnimation IdleRight   { get; }

	public static CharacterAnimation Hurt { get; }

	static CharacterAnimations()
	{
		Actions = new List<CharacterAnimationAction>
		{
			CharacterAnimationAction.Walk,
			CharacterAnimationAction.Shoot,
			CharacterAnimationAction.Slash,
			CharacterAnimationAction.Thrust,
			CharacterAnimationAction.Spellcast,
			CharacterAnimationAction.Hurt,
			CharacterAnimationAction.Idle
		};
		Directions = new List<CharacterHeading> {CharacterHeading.Up, CharacterHeading.Down, CharacterHeading.Left, CharacterHeading.Right};

		WalkUp   = new CharacterAnimation(CharacterAnimationAction.Walk,      CharacterHeading.Up, WalkFrameCount);
		ShootUp  = new CharacterAnimation(CharacterAnimationAction.Shoot,     CharacterHeading.Up, ShootFrameCount);
		SlashUp  = new CharacterAnimation(CharacterAnimationAction.Slash,     CharacterHeading.Up, SlashFrameCount);
		ThrustUp = new CharacterAnimation(CharacterAnimationAction.Thrust,    CharacterHeading.Up, ThrustFrameCount);
		CastUp   = new CharacterAnimation(CharacterAnimationAction.Spellcast, CharacterHeading.Up, CastFrameCount);
		IdleUp   = new CharacterAnimation(CharacterAnimationAction.Idle,      CharacterHeading.Up, IdleFrameCount);

		WalkDown   = new CharacterAnimation(CharacterAnimationAction.Walk,      CharacterHeading.Down, WalkFrameCount);
		ShootDown  = new CharacterAnimation(CharacterAnimationAction.Shoot,     CharacterHeading.Down, ShootFrameCount);
		SlashDown  = new CharacterAnimation(CharacterAnimationAction.Slash,     CharacterHeading.Down, SlashFrameCount);
		ThrustDown = new CharacterAnimation(CharacterAnimationAction.Thrust,    CharacterHeading.Down, ThrustFrameCount);
		CastDown   = new CharacterAnimation(CharacterAnimationAction.Spellcast, CharacterHeading.Down, CastFrameCount);
		IdleDown   = new CharacterAnimation(CharacterAnimationAction.Idle,      CharacterHeading.Down, IdleFrameCount);

		WalkRight   = new CharacterAnimation(CharacterAnimationAction.Walk,      CharacterHeading.Right, WalkFrameCount);
		ShootRight  = new CharacterAnimation(CharacterAnimationAction.Shoot,     CharacterHeading.Right, ShootFrameCount);
		SlashRight  = new CharacterAnimation(CharacterAnimationAction.Slash,     CharacterHeading.Right, SlashFrameCount);
		ThrustRight = new CharacterAnimation(CharacterAnimationAction.Thrust,    CharacterHeading.Right, ThrustFrameCount);
		CastRight   = new CharacterAnimation(CharacterAnimationAction.Spellcast, CharacterHeading.Right, CastFrameCount);
		IdleRight   = new CharacterAnimation(CharacterAnimationAction.Idle,      CharacterHeading.Right, IdleFrameCount);

		WalkLeft   = new CharacterAnimation(CharacterAnimationAction.Walk,      CharacterHeading.Left, WalkFrameCount);
		ShootLeft  = new CharacterAnimation(CharacterAnimationAction.Shoot,     CharacterHeading.Left, ShootFrameCount);
		SlashLeft  = new CharacterAnimation(CharacterAnimationAction.Slash,     CharacterHeading.Left, SlashFrameCount);
		ThrustLeft = new CharacterAnimation(CharacterAnimationAction.Thrust,    CharacterHeading.Left, ThrustFrameCount);
		CastLeft   = new CharacterAnimation(CharacterAnimationAction.Spellcast, CharacterHeading.Left, CastFrameCount);
		IdleLeft   = new CharacterAnimation(CharacterAnimationAction.Idle,      CharacterHeading.Left, IdleFrameCount);

		Hurt = new CharacterAnimation(CharacterAnimationAction.Hurt, CharacterHeading.Down, HurtFrameCount);

		Animations = new Dictionary<string, CharacterAnimation>
		{
			{WalkUp.AnimationKey, WalkUp},
			{ShootUp.AnimationKey, ShootUp},
			{SlashUp.AnimationKey, SlashUp},
			{ThrustUp.AnimationKey, ThrustUp},
			{CastUp.AnimationKey, CastUp},
			{IdleUp.AnimationKey, IdleUp},
			{WalkDown.AnimationKey, WalkDown},
			{ShootDown.AnimationKey, ShootDown},
			{SlashDown.AnimationKey, SlashDown},
			{ThrustDown.AnimationKey, ThrustDown},
			{CastDown.AnimationKey, CastDown},
			{IdleDown.AnimationKey, IdleDown},
			{WalkLeft.AnimationKey, WalkLeft},
			{ShootLeft.AnimationKey, ShootLeft},
			{SlashLeft.AnimationKey, SlashLeft},
			{ThrustLeft.AnimationKey, ThrustLeft},
			{CastLeft.AnimationKey, CastLeft},
			{IdleLeft.AnimationKey, IdleLeft},
			{WalkRight.AnimationKey, WalkRight},
			{ShootRight.AnimationKey, ShootRight},
			{SlashRight.AnimationKey, SlashRight},
			{ThrustRight.AnimationKey, ThrustRight},
			{CastRight.AnimationKey, CastRight},
			{IdleRight.AnimationKey, IdleRight},
			{Hurt.AnimationKey, Hurt}
		};
	}

	public static int WalkFrameCount   => 9;
	public static int ShootFrameCount  => 13;
	public static int SlashFrameCount  => 6;
	public static int ThrustFrameCount => 8;
	public static int CastFrameCount   => 7;
	public static int HurtFrameCount   => 6;
	public static int IdleFrameCount   => 1;
}

public class CharacterAnimation
{
	public string                   AnimationKey => $"{Action.ToSelectorString()}_{Heading.ToSelectorString()}";
	public CharacterAnimationAction Action       { get; set; }
	public CharacterHeading         Heading      { get; set; }
	public int                      FrameCount   { get; set; }


	public CharacterAnimation(CharacterAnimationAction action, CharacterHeading heading, int frameCount)
	{
		Action     = action;
		Heading    = heading;
		FrameCount = frameCount;
	}
}
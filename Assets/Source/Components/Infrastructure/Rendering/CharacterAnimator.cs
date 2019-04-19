using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Entitas.Unity;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.U2D;


public class SingleSpriteCharacterAnimator : MonoBehaviour
{
	/* the animation clips */
	private Dictionary<string, Sprite[]> _bodyClips;
	private Dictionary<string, Sprite[]> _weaponClips;
	private Sprite[]                     _currentBodyAnimationClip;
	private Sprite[]                     _currentWeaponAnimationClip;

	/* behavior */
	private SpriteRenderer _bodyRenderer;
	private SpriteRenderer _weaponRenderer;
	public  string         _currentAnimationKey;
	private int            _currentFrameIndex;
	private int            _currentFrameCount;
	private float          _currentClipElapsed;
	private float          _currentClipElapsedLimit;
	private float          _currentFrameElapsed;
	private float          _currentSingleFrameLimit;
	private bool           _stopWhenFinished;

	private GameEntity _entity => (GameEntity) GetComponent<EntityLink>()?.entity;


	private void InitClips()
	{
		var bodySelector = string.Empty;
		switch (_entity.character.EquipmentTemplate)
		{
			case EquipmentTemplate.Cloth:
				bodySelector = "templates_cleric_shield";
				break;
			case EquipmentTemplate.Leather:
				bodySelector = "templates_archer_bow";
				break;
			case EquipmentTemplate.Mail:
				bodySelector = "templates_sword_shield";
				break;
			case EquipmentTemplate.Plate:
				bodySelector = "templates_leader_shield";
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		var weaponSelector = string.Empty;
		switch (_entity.character.EquipmentTemplate)
		{
			case EquipmentTemplate.Cloth:
				/*weaponSelector = "righthand_male_mace";*/
				break;
			/*case EquipmentTemplate.Leather:
				weaponSelector = "templates_archer_bow";
				break;*/
			case EquipmentTemplate.Mail:
				weaponSelector = "righthand_male_rapier";
				break;
			case EquipmentTemplate.Plate:
				/*weaponSelector = "righthand_male_longsword";*/
				weaponSelector = "righthand_male_rapier";
				break;
		}

		if (_entity.isPlayerControlled)
			weaponSelector = "righthand_male_longsword";

		/* import all piece clips for each animation */
		var animations = CharacterAnimations.Animations;
		foreach (var animation in animations)
		{
			var bodySprites = new Sprite[animation.Value.FrameCount];
			for (var j = 0; j < animation.Value.FrameCount; j++)
			{
				var spriteKey = $"{bodySelector}_{animation.Value.Action.ToSelectorString()}_{animation.Value.Heading.ToSelectorString()}_{j}";
				if (animation.Value.Action == CharacterAnimationAction.Idle)
					spriteKey = $"{bodySelector}_{CharacterAnimationAction.Walk.ToSelectorString()}_{animation.Value.Heading.ToSelectorString()}_{j}";
				if (animation.Value.Action == CharacterAnimationAction.Hurt)
					spriteKey = $"{bodySelector}_{CharacterAnimationAction.Hurt.ToSelectorString()}_{j}";

				bodySprites[j] = AtlasManager.CharacterTemplateInstance.GetSprite(spriteKey);
			}

			_bodyClips.Add(animation.Key, bodySprites);

			if (!string.IsNullOrEmpty(weaponSelector))
			{
				var weaponSprites = new Sprite[animation.Value.FrameCount];
				if (animation.Value.Action == CharacterAnimationAction.Slash)
				{
					for (var j = 0; j < animation.Value.FrameCount; j++)
					{
						var headingKey = animation.Value.Heading.ToSelectorString();
						/*if (animation.Value.Heading == CharacterAnimationHeading.Left)
							headingKey = CharacterAnimationHeading.Down.ToSelectorString();
						else if (animation.Value.Heading == CharacterAnimationHeading.Right)
							headingKey = CharacterAnimationHeading.Up.ToSelectorString();*/

						var spriteKey = $"{weaponSelector}_{animation.Value.Action.ToSelectorString()}_{headingKey}_{j}";
						weaponSprites[j] = AtlasManager.CharacterTemplateInstance.GetSprite(spriteKey);
					}

					_weaponClips.Add(animation.Key, weaponSprites);
				}
			}
		}
	}

	private void InitRenderers()
	{
		//create go with sprite renderer for each piece
		var bodyGo = new GameObject("body");
		bodyGo.transform.parent        = transform;
		bodyGo.transform.position      = transform.position;
		_bodyRenderer                  = bodyGo.AddComponent<SpriteRenderer>();
		_bodyRenderer.sortingLayerName = "characters";
		var weaponGo = new GameObject("weapon");
		weaponGo.transform.parent        = transform;
		weaponGo.transform.position      = transform.position;
		_weaponRenderer                  = weaponGo.AddComponent<SpriteRenderer>();
		_weaponRenderer.sortingLayerName = "characters";
	}

	private void Start()
	{
		InitRenderers();
		enabled = true;
		Play("wc_d");
	}

	private void Awake()
	{
		_bodyClips   = new Dictionary<string, Sprite[]>();
		_weaponClips = new Dictionary<string, Sprite[]>();
		InitClips();
	}

	public void Play(string animationKey)
	{
		/* clip not found */
		if (!_bodyClips.ContainsKey(animationKey) || _bodyClips[animationKey].Length == 0 || string.IsNullOrEmpty(animationKey))
		{
			Debug.Log($"trying to play animation that is not found: {animationKey}");
			return;
		}

		/* clip already playing (don't interupt current iteration) */
		if (_currentAnimationKey == animationKey)
			return;

		_currentAnimationKey      = animationKey;
		_stopWhenFinished         = !animationKey.Contains("wc");
		_currentFrameIndex        = 0;
		_currentClipElapsed       = 0f;
		_currentBodyAnimationClip = _bodyClips[animationKey];

		if (_weaponClips.ContainsKey(animationKey))
			_currentWeaponAnimationClip = _weaponClips[animationKey];
		else
			_currentWeaponAnimationClip = null;

		_currentFrameCount = _currentBodyAnimationClip.Length;

		if (_entity.hasActing)
			_currentSingleFrameLimit = _entity.acting.duration / _currentFrameCount;
		else
			_currentSingleFrameLimit = .5f / _currentFrameCount;

		//render the first frame of the animation
		RenderAnimationFrame();
	}

	public void Stop()
	{
		_currentAnimationKey        = null;
		_currentBodyAnimationClip   = null;
		_currentWeaponAnimationClip = null;
		_currentFrameIndex          = 0;
		_currentClipElapsed         = 0f;
		_currentFrameElapsed        = 0f;
		_currentFrameCount          = 0;
		_currentSingleFrameLimit    = 0f;
		_stopWhenFinished           = true;
	}

	public void Update()
	{
		_bodyRenderer.sortingOrder   = (int) (transform.position.y * -1000);
		_weaponRenderer.sortingOrder = (int) (transform.position.y * -1000);
		
		if (string.IsNullOrEmpty(_currentAnimationKey))
			return;

		//update timers
		_currentClipElapsed  += Time.deltaTime;
		_currentFrameElapsed += Time.deltaTime;

		//show next frame?
		if (_currentFrameElapsed >= _currentSingleFrameLimit)
		{
			_currentFrameIndex++;
			_currentFrameElapsed = 0f;

			RenderAnimationFrame();

			var animationComplete = _currentFrameIndex >= _currentFrameCount - 1;
			//continue
			if (animationComplete)
				_currentFrameIndex = 0;
			//replay
			if (_stopWhenFinished && animationComplete)
				Stop();
		}
	}

	private void RenderAnimationFrame()
	{
		if (_bodyRenderer != null && _currentBodyAnimationClip != null && _currentFrameIndex <= _currentFrameCount - 1)
			_bodyRenderer.sprite = _currentBodyAnimationClip[_currentFrameIndex];

		if (_weaponRenderer != null && _currentWeaponAnimationClip != null)
			_weaponRenderer.sprite = _currentWeaponAnimationClip[_currentFrameIndex];
		/*if (_currentAnimationKey.Contains("_l"))
			_weaponRenderer.flipX = true;
		else
			_weaponRenderer.flipX = false;*/
		else if (_weaponRenderer != null)
			_weaponRenderer.sprite = null;
	}
}

public class CharacterAnimator : MonoBehaviour
{
	/* the animation clips */
	private Dictionary<CharacterPiecePart, CharacterPiece>               _pieces;
	private Dictionary<string, Dictionary<CharacterPiecePart, Sprite[]>> _animationClips;
	private Dictionary<CharacterPiecePart, Sprite[]>                     _currentAnimationClip;

	/* behavior */
	private Dictionary<CharacterPiecePart, SpriteRenderer> _spriteRenderers;
	private string                                         _currentAnimationKey;
	private int                                            _currentFrameIndex;
	private int                                            _currentFrameCount;
	private float                                          _currentClipElapsed;
	private float                                          _currentClipElapsedLimit;
	private float                                          _currentFrameElapsed;
	private float                                          _currentSingleFrameLimit;
	private bool                                           _stopWhenFinished;

	private GameEntity _entity => (GameEntity) GetComponent<EntityLink>()?.entity;

	private void SetIdentity(CharacterPieceStyle style)
	{
		//body (gender/race)

		//eyes

		//ears

		//face (mask)

		//neck

		//nose

		//hair

		//facial hair

		//back
	}

	private void SetWeapons(CharacterPieceGender gender, bool sword, bool shield, bool wand, bool bow)
	{
		if (sword)
			_pieces.Add(CharacterPiecePart.RightHand, new CharacterPiece(CharacterPiecePart.RightHand, gender, "spear"));
		if (shield)
			_pieces.Add(CharacterPiecePart.LeftHand, new CharacterPiece(CharacterPiecePart.LeftHand, gender, "shield"));
		if (wand)
			_pieces.Add(CharacterPiecePart.RightHand, new CharacterPiece(CharacterPiecePart.RightHand, gender, "wand_wood"));
		if (bow)
		{
			_pieces.Add(CharacterPiecePart.RightHand, new CharacterPiece(CharacterPiecePart.RightHand, gender, "either_bow"));
			_pieces.Add(CharacterPiecePart.LeftHand,  new CharacterPiece(CharacterPiecePart.LeftHand,  gender, "either_arrow"));
			_pieces.Add(CharacterPiecePart.Back2,     new CharacterPiece(CharacterPiecePart.Back2,     gender, "either_quiver"));
		}
	}

	private void SetStyle(CharacterPieceGender gender, CharacterPieceStyle style)
	{
		_pieces.Add(CharacterPiecePart.Head,     new CharacterPiece(CharacterPiecePart.Head,     gender, style));
		_pieces.Add(CharacterPiecePart.Chest,    new CharacterPiece(CharacterPiecePart.Chest,    gender, style));
		_pieces.Add(CharacterPiecePart.Shoulder, new CharacterPiece(CharacterPiecePart.Shoulder, gender, CharacterPieceStyle.leather));
		_pieces.Add(CharacterPiecePart.Arms,     new CharacterPiece(CharacterPiecePart.Arms,     gender, CharacterPieceStyle.platemail, Color.grey));
		_pieces.Add(CharacterPiecePart.Hands,    new CharacterPiece(CharacterPiecePart.Hands,    gender, CharacterPieceStyle.platemail, Color.grey));
		_pieces.Add(CharacterPiecePart.Waist,    new CharacterPiece(CharacterPiecePart.Waist,    gender, "leather"));
		_pieces.Add(CharacterPiecePart.Legs,     new CharacterPiece(CharacterPiecePart.Legs,     gender, CharacterPieceStyle.platemail, Color.grey));
		_pieces.Add(CharacterPiecePart.Feet,     new CharacterPiece(CharacterPiecePart.Feet,     gender, CharacterPieceStyle.platemail, Color.grey));
	}

	private void SetGender(CharacterPieceGender gender)
	{
		_pieces.Add(CharacterPiecePart.Body, new CharacterPiece(CharacterPiecePart.Body, gender, "light"));
		/*_pieces.Add(CharacterPiecePart.FacialHair, new CharacterPiece(CharacterPiecePart.FacialHair, gender, "mustache"));*/
		/*_pieces.Add(CharacterPiecePart.Hair, new CharacterPiece(CharacterPiecePart.Hair, gender, "mohawk"));*/
	}

	private void InitPieces()
	{
		/*//sprite keys are: {piece}_{gender}_{item}*/
		CharacterPieceGender gender = CharacterPieceGender.male;
		var                  style  = _entity.character.EquipmentTemplate;

		SetGender(gender);

		if (style == EquipmentTemplate.Plate)
		{
			SetStyle(gender, CharacterPieceStyle.platemail);
			SetWeapons(gender, true, true, false, false);
		}

		if (style == EquipmentTemplate.Mail)
		{
			SetStyle(gender, CharacterPieceStyle.chainmail);
			SetWeapons(gender, true, true, false, false);
		}

		if (style == EquipmentTemplate.Leather)
		{
			SetStyle(gender, CharacterPieceStyle.leather);
			SetWeapons(gender, false, false, false, true);
		}

		if (style == EquipmentTemplate.Cloth)
		{
			SetStyle(gender, CharacterPieceStyle.chainmail);
			SetWeapons(gender, false, false, true, false);
		}
	}

	private void InitClips()
	{
		/* import all piece clips for each animation */
		var animations = CharacterAnimations.Animations;
		foreach (var animation in animations)
		{
			_animationClips.Add(animation.Key, new Dictionary<CharacterPiecePart, Sprite[]>());
			var clip = _animationClips[animation.Key];

			var enabledPieces = _pieces.Where(p => p.Value.enabled);
			foreach (var piece in enabledPieces)
			{
				var sprites = new Sprite[animation.Value.FrameCount];
				for (var j = 0; j < animation.Value.FrameCount; j++)
				{
					var spriteKey = $"{piece.Value.selector}_{animation.Value.Action.ToSelectorString()}_{animation.Value.Heading.ToSelectorString()}_{j}";
					if (animation.Value.Action == CharacterAnimationAction.Hurt)
						spriteKey = $"{piece.Value.selector}_{CharacterAnimationAction.Hurt.ToSelectorString()}_{j}";

					sprites[j] = AtlasManager.CharacterInstance.GetSprite(spriteKey);
				}

				//add this piece animation to the full clip
				clip.Add(piece.Key, sprites);
			}
		}
	}

	private void InitRenderers()
	{
		//create go with sprite renderer for each piece
		foreach (var piece in _pieces.Keys)
		{
			//setup go
			var pieceName = Enum.GetName(typeof(CharacterPiecePart), piece);
			var go        = new GameObject(pieceName);
			go.transform.parent                      = transform;
			go.transform.position                    = transform.position;
			_spriteRenderers[piece]                  = go.AddComponent<SpriteRenderer>();
			_spriteRenderers[piece].sortingOrder     = (int) piece;
			_spriteRenderers[piece].sortingLayerName = "characters";
		}
	}

	private void Start()
	{
		InitRenderers();
	}

	private void Awake()
	{
		_pieces          = new Dictionary<CharacterPiecePart, CharacterPiece>();
		_animationClips  = new Dictionary<string, Dictionary<CharacterPiecePart, Sprite[]>>();
		_spriteRenderers = new Dictionary<CharacterPiecePart, SpriteRenderer>();

		InitPieces();
		InitClips();
	}

	public void Play(CharacterAnimation animation)
	{
		Play(animation.AnimationKey);
	}

	public void Play(string animationKey)
	{
		/* clip not found */
		if (!_animationClips.ContainsKey(animationKey) || _animationClips[animationKey].Count == 0)
		{
			Debug.Log($"trying to play animation that is not found: {animationKey}");
			return;
		}

		/* clip already playing (don't interupt current iteration) */
		if (_currentAnimationKey == animationKey)
			return;


		_currentAnimationKey = animationKey;
		if (_currentAnimationKey.Contains("wc"))
			_stopWhenFinished = false;
		else
			_stopWhenFinished = true;
		_currentFrameIndex    = 0;
		_currentClipElapsed   = 0f;
		_currentFrameElapsed  = 0f;
		_currentAnimationClip = _animationClips[animationKey];
		_currentFrameCount    = _currentAnimationClip[0].Length;
		if (_currentFrameCount > 0)
			_currentSingleFrameLimit = 1f / _currentFrameCount;
		else
			Stop();
	}

	public void Stop()
	{
		_currentAnimationKey     = null;
		_currentFrameIndex       = 0;
		_currentClipElapsed      = 0f;
		_currentFrameElapsed     = 0f;
		_currentAnimationClip    = null;
		_currentFrameCount       = 0;
		_currentSingleFrameLimit = 0f;
		_stopWhenFinished        = true;
	}

	void Update()
	{
		if (string.IsNullOrEmpty(_currentAnimationKey))
			return;

		//update timers
		_currentClipElapsed  += Time.deltaTime;
		_currentFrameElapsed += Time.deltaTime;

		//show next frame?
		if (_currentFrameElapsed >= _currentSingleFrameLimit)
		{
			_currentFrameIndex++;

			//replay?
			var animationComplete = (_currentFrameIndex >= _currentFrameCount - 1);

			if (_stopWhenFinished && animationComplete)
			{
				Stop();
				return;
			}

			if (animationComplete)
				_currentFrameIndex = 0;

			RenderAnimationFrame();

			_currentFrameElapsed = 0f;
		}
	}

	private void RenderAnimationFrame()
	{
		foreach (var piece in _pieces)
		{
			var renderer = _spriteRenderers[piece.Key];
			renderer.sprite       = _currentAnimationClip[piece.Key][_currentFrameIndex];
			renderer.sortingOrder = (int) (transform.position.y * -100 + (float) piece.Key);
			if (piece.Value.color != Color.clear)
				renderer.material.SetColor("_Color", piece.Value.color);
		}
	}
}

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

public enum CharacterAnimationHeading
{
	Up,
	Down,
	Left,
	Right
}

public static class CharacterAnimationDirectionExtensions
{
	public static string ToSelectorString(this CharacterAnimationHeading heading)
	{
		switch (heading)
		{
			case CharacterAnimationHeading.Up:
				return "t";
			case CharacterAnimationHeading.Down:
				return "d";
			case CharacterAnimationHeading.Left:
				return "l";
			case CharacterAnimationHeading.Right:
				return "r";
			default:
				throw new ArgumentOutOfRangeException(nameof(heading), heading, null);
		}
	}
}

public static class CharacterAnimations
{
	public static Dictionary<string, CharacterAnimation> Animations { get; }
	public static List<CharacterAnimationAction>         Actions    { get; }
	public static List<CharacterAnimationHeading>        Directions { get; }

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
		Directions = new List<CharacterAnimationHeading> {CharacterAnimationHeading.Up, CharacterAnimationHeading.Down, CharacterAnimationHeading.Left, CharacterAnimationHeading.Right};

		WalkUp   = new CharacterAnimation(CharacterAnimationAction.Walk,      CharacterAnimationHeading.Up, WalkFrameCount);
		ShootUp  = new CharacterAnimation(CharacterAnimationAction.Shoot,     CharacterAnimationHeading.Up, ShootFrameCount);
		SlashUp  = new CharacterAnimation(CharacterAnimationAction.Slash,     CharacterAnimationHeading.Up, SlashFrameCount);
		ThrustUp = new CharacterAnimation(CharacterAnimationAction.Thrust,    CharacterAnimationHeading.Up, ThrustFrameCount);
		CastUp   = new CharacterAnimation(CharacterAnimationAction.Spellcast, CharacterAnimationHeading.Up, CastFrameCount);
		IdleUp   = new CharacterAnimation(CharacterAnimationAction.Idle,      CharacterAnimationHeading.Up, IdleFrameCount);

		WalkDown   = new CharacterAnimation(CharacterAnimationAction.Walk,      CharacterAnimationHeading.Down, WalkFrameCount);
		ShootDown  = new CharacterAnimation(CharacterAnimationAction.Shoot,     CharacterAnimationHeading.Down, ShootFrameCount);
		SlashDown  = new CharacterAnimation(CharacterAnimationAction.Slash,     CharacterAnimationHeading.Down, SlashFrameCount);
		ThrustDown = new CharacterAnimation(CharacterAnimationAction.Thrust,    CharacterAnimationHeading.Down, ThrustFrameCount);
		CastDown   = new CharacterAnimation(CharacterAnimationAction.Spellcast, CharacterAnimationHeading.Down, CastFrameCount);
		IdleDown   = new CharacterAnimation(CharacterAnimationAction.Idle,      CharacterAnimationHeading.Down, IdleFrameCount);

		WalkRight   = new CharacterAnimation(CharacterAnimationAction.Walk,      CharacterAnimationHeading.Right, WalkFrameCount);
		ShootRight  = new CharacterAnimation(CharacterAnimationAction.Shoot,     CharacterAnimationHeading.Right, ShootFrameCount);
		SlashRight  = new CharacterAnimation(CharacterAnimationAction.Slash,     CharacterAnimationHeading.Right, SlashFrameCount);
		ThrustRight = new CharacterAnimation(CharacterAnimationAction.Thrust,    CharacterAnimationHeading.Right, ThrustFrameCount);
		CastRight   = new CharacterAnimation(CharacterAnimationAction.Spellcast, CharacterAnimationHeading.Right, CastFrameCount);
		IdleRight   = new CharacterAnimation(CharacterAnimationAction.Idle,      CharacterAnimationHeading.Right, IdleFrameCount);

		WalkLeft   = new CharacterAnimation(CharacterAnimationAction.Walk,      CharacterAnimationHeading.Left, WalkFrameCount);
		ShootLeft  = new CharacterAnimation(CharacterAnimationAction.Shoot,     CharacterAnimationHeading.Left, ShootFrameCount);
		SlashLeft  = new CharacterAnimation(CharacterAnimationAction.Slash,     CharacterAnimationHeading.Left, SlashFrameCount);
		ThrustLeft = new CharacterAnimation(CharacterAnimationAction.Thrust,    CharacterAnimationHeading.Left, ThrustFrameCount);
		CastLeft   = new CharacterAnimation(CharacterAnimationAction.Spellcast, CharacterAnimationHeading.Left, CastFrameCount);
		IdleLeft   = new CharacterAnimation(CharacterAnimationAction.Idle,      CharacterAnimationHeading.Left, IdleFrameCount);

		Hurt = new CharacterAnimation(CharacterAnimationAction.Hurt, CharacterAnimationHeading.Down, HurtFrameCount);

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

	private static int WalkFrameCount   => 9;
	private static int ShootFrameCount  => 13;
	private static int SlashFrameCount  => 6;
	private static int ThrustFrameCount => 8;
	private static int CastFrameCount   => 7;
	private static int HurtFrameCount   => 6;
	private static int IdleFrameCount   => 1;
}

public class CharacterAnimation
{
	public string                    AnimationKey => $"{Action.ToSelectorString()}_{Heading.ToSelectorString()}";
	public CharacterAnimationAction  Action       { get; set; }
	public CharacterAnimationHeading Heading      { get; set; }
	public int                       FrameCount   { get; set; }


	public CharacterAnimation(CharacterAnimationAction action, CharacterAnimationHeading heading, int frameCount)
	{
		Action     = action;
		Heading    = heading;
		FrameCount = frameCount;
	}
}
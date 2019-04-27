using System;
using System.Collections.Generic;
using System.Linq;
using Entitas.Unity;
using UnityEngine;

/// <summary>
/// sprites = actual resources (texture/sprite)
/// animations = set of sprites for an animation
/// renderers = swap sprites based in current animation
/// </summary>
public class CharacterPieceAnimatorView : View
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
	private CharacterHeading                               _heading;

	private GameEntity _entity => (GameEntity) GetComponent<EntityLink>()?.entity;

	/*private CharacterAnimator _characterAnimator;*/
	/*private SingleSpriteCharacterAnimator _characterAnimator;*/

	public override void Link(GameEntity entity)
	{
		if (gameObject.GetComponent<EntityLink>() == null)
			gameObject.Link(entity);

		entity.AddCombatActionListener(this);
		entity.AddCombatActionRemovedListener(this);
		entity.AddVelocityListener(this);
		entity.AddDefeatedListener(this);

		//parent
		transform.position   = _entity.position.value;
		transform.localScale = Vector3.one;

		/*_characterAnimator = gameObject.AddComponent<CharacterAnimator>();*/
		/*_characterAnimator = gameObject.AddComponent<SingleSpriteCharacterAnimator>();*/
	}

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
		/*var                  style  = _entity.character.EquipmentTemplate;*/
		var style = EquipmentTemplate.Plate;

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

	private void Play(string animationKey)
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

	private void Play(CharacterAnimationAction action)
	{
		Play(GetAnimationKeyFor(action, _heading));
	}

	private void Play(CharacterAnimationAction action, CharacterHeading heading)
	{
		Play(GetAnimationKeyFor(action, heading));
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

	public override void Update()
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

		FollowPlayer();
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

	private void FollowPlayer()
	{
		if (_entity.isPlayerControlled)
		{
			var     target = gameObject;
			Vector3 posNoZ = transform.position;
			posNoZ.z = target.transform.position.z;

			Vector3 targetDirection = (target.transform.position - posNoZ);

			var interpVelocity = targetDirection.magnitude * 5f;

			var targetPos = transform.position + (targetDirection.normalized * interpVelocity * Time.deltaTime);
			var offset    = new Vector3(0, 0, -10);

			Camera.main.transform.position = Vector3.Lerp(transform.position, targetPos + offset, 0.25f);
		}
	}

	public override void OnCombatAction(GameEntity entity, CombatAction action, float elapsed)
	{
		Play(action.action);
	}

	public override void OnCombatActionRemoved(GameEntity entity)
	{
		if (!entity.hasInput || entity.input.direction != Vector2.zero)
			return;

		Play(CharacterAnimationAction.Idle);
	}

	public override void OnVelocity(GameEntity entity, Vector2 value)
	{
		if (entity.hasCombatAction || entity.isDefeated)
			return;

		if (value == Vector2.zero)
			Play(CharacterAnimationAction.Idle, entity.heading);
		else
			Play(CharacterAnimationAction.Walk, entity.heading);
	}

	public override void OnDefeated(GameEntity entity)
	{
		if (!entity.isDefeated)
			return;

		Debug.Log(gameObject.name + " was defeated");
		Play(CharacterAnimationAction.Hurt, CharacterHeading.Down);
	}

	private void SetHeading(Vector2 velocity)
	{
		if (velocity.x > 0)
			_heading = CharacterHeading.Right;
		else if (velocity.x < 0)
			_heading = CharacterHeading.Left;
		else if (velocity.y > 0)
			_heading = CharacterHeading.Up;
		else if (velocity.y < 0)
			_heading = CharacterHeading.Down;
	}

	private string GetAnimationKeyFor(CharacterAnimationAction action, CharacterHeading heading)
	{
		return $"{action.ToSelectorString()}_{heading.ToSelectorString()}";
	}
}
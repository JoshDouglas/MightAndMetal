#region

using System;
using System.Collections.Generic;
using Entitas.Unity;
using UnityEngine;

#endregion

/// <summary>
///     sprites = actual resources (texture/sprite)
///     animations = set of sprites for an animation
///     renderers = swap sprites based in current animation
/// </summary>
public class CharacterAnimatorView : View
{
	/* the animation clips */
	private Dictionary<string, Sprite[]> _bodyClips;

	/* behavior */
	private SpriteRenderer _bodyRenderer;
	public  string         _currentAnimationKey;

	//todo: shield (for blocking)
	//todo: projectile (for ranged)
	private Sprite[]         _currentBodyAnimationClip;
	private float            _currentClipElapsed;
	private float            _currentClipElapsedLimit;
	private int              _currentFrameCount;
	private float            _currentFrameElapsed;
	private int              _currentFrameIndex;
	private float            _currentSingleFrameLimit;
	private Sprite[]         _currentTabardAnimationClip;
	private Sprite[]         _currentWeaponAnimationClip;
	private CharacterHeading _heading;
	private float            _idleTimerElapsed;
	private bool             _stopWhenFinished;

	private Dictionary<string, Sprite[]> _tabardClips;
	private SpriteRenderer               _tabardRenderer;
	private Dictionary<string, Sprite[]> _weaponClips;
	private SpriteRenderer               _weaponRenderer;
	private GameEntity                   _entity => (GameEntity) GetComponent<EntityLink>()?.entity;

	#region setup

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

		_bodyClips   = new Dictionary<string, Sprite[]>();
		_weaponClips = new Dictionary<string, Sprite[]>();
		_tabardClips = new Dictionary<string, Sprite[]>();

		InitClips();
		InitRenderers();
		enabled = true;

		Play(CharacterAnimationAction.Walk, CharacterHeading.Down);
	}

	private void InitClips()
	{
		var bodySelector = string.Empty;
		/*switch (_entity.character.EquipmentTemplate)
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
		}*/

		bodySelector = "templates_sword_shield";

		var weaponSelector = string.Empty;
		if (_entity.hasWeapon)
		{
			switch (_entity.weapon.weaponType)
			{
				case CharacterWeaponType.longsword:
					weaponSelector = "righthand_male_longsword";
					break;
				case CharacterWeaponType.rapier:
					weaponSelector = "righthand_male_rapier";
					break;
				case CharacterWeaponType.saber:
					weaponSelector = "righthand_male_saber";
					break;
				case CharacterWeaponType.mace:
					weaponSelector = "righthand_male_mace";
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		var tabardSelector = "chest_male_chain_tabard";

		/* import all piece clips for each animation */
		var animations = CharacterAnimations.Animations;
		foreach (var animation in animations)
		{
			var bodySprites   = new Sprite[animation.Value.FrameCount];
			var tabardSprites = new Sprite[animation.Value.FrameCount];
			var weaponSprites = new Sprite[animation.Value.FrameCount];
			for (var j = 0; j < animation.Value.FrameCount; j++)
			{
				var bodyKey   = $"{bodySelector}_{animation.Value.Action.ToSelectorString()}_{animation.Value.Heading.ToSelectorString()}_{j}";
				var tabardKey = $"{tabardSelector}_{animation.Value.Action.ToSelectorString()}_{animation.Value.Heading.ToSelectorString()}_{j}";

				if (animation.Value.Action == CharacterAnimationAction.Idle)
				{
					bodyKey   = $"{bodySelector}_{CharacterAnimationAction.Walk.ToSelectorString()}_{animation.Value.Heading.ToSelectorString()}_{j}";
					tabardKey = $"{tabardSelector}_{CharacterAnimationAction.Walk.ToSelectorString()}_{animation.Value.Heading.ToSelectorString()}_{j}";
				}

				if (animation.Value.Action == CharacterAnimationAction.Hurt)
				{
					bodyKey   = $"{bodySelector}_{CharacterAnimationAction.Hurt.ToSelectorString()}_{j}";
					tabardKey = $"{tabardSelector}_{CharacterAnimationAction.Hurt.ToSelectorString()}_{j}";
				}

				var weaponKey = string.Empty;
				if (animation.Value.Action == CharacterAnimationAction.Slash)
					weaponKey = $"{weaponSelector}_{animation.Value.Action.ToSelectorString()}_{animation.Value.Heading.ToSelectorString()}_{j}";

				if (!string.IsNullOrEmpty(bodyKey))
					bodySprites[j] = AtlasManager.CharacterTemplateInstance.GetSprite(bodyKey);
				if (!string.IsNullOrEmpty(tabardKey))
					tabardSprites[j] = AtlasManager.CharacterTemplateInstance.GetSprite(tabardKey);
				if (!string.IsNullOrEmpty(weaponKey))
					weaponSprites[j] = AtlasManager.CharacterTemplateInstance.GetSprite(weaponKey);
			}

			_bodyClips.Add(animation.Key, bodySprites);
			_tabardClips.Add(animation.Key, tabardSprites);
			_weaponClips.Add(animation.Key, weaponSprites);
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
		var tabardGo = new GameObject("tabard");
		tabardGo.transform.parent        = transform;
		tabardGo.transform.position      = transform.position;
		_tabardRenderer                  = tabardGo.AddComponent<SpriteRenderer>();
		_tabardRenderer.sortingLayerName = "characters";
		if (_entity.faction.name == "Ivalice")
			_tabardRenderer.material.SetColor("_Color", new Color(.8f, .116f, .140f));
		else
			_tabardRenderer.material.SetColor("_Color", new Color(.212f, .192f, .162f));
	}

	private void Start()
	{
		/*InitRenderers();
		enabled = true;
		Play(CharacterAnimationAction.Walk, CharacterHeading.Down);*/
	}

	private void Awake()
	{
		/*_bodyClips   = new Dictionary<string, Sprite[]>();
		_weaponClips = new Dictionary<string, Sprite[]>();
		_tabardClips = new Dictionary<string, Sprite[]>();
		InitClips();*/
	}

	#endregion

	#region rendering

	private void Play(CharacterAnimationAction action)
	{
		Play(GetAnimationKeyFor(action, _heading));
	}

	private void Play(CharacterAnimationAction action, CharacterHeading heading)
	{
		Play(GetAnimationKeyFor(action, heading));
	}

	private void Play(string animationKey)
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

		_currentAnimationKey        = animationKey;
		_stopWhenFinished           = !animationKey.Contains("wc");
		_currentFrameIndex          = 0;
		_currentClipElapsed         = 0f;
		_currentBodyAnimationClip   = _bodyClips[animationKey];
		_currentWeaponAnimationClip = _weaponClips[animationKey];
		_currentTabardAnimationClip = _tabardClips[animationKey];

		_currentFrameCount = _currentBodyAnimationClip.Length;

		if (_entity.hasCombatAction)
			_currentSingleFrameLimit = _entity.combatAction.action.duration / _currentFrameCount;
		else
			_currentSingleFrameLimit = .5f / _currentFrameCount;

		//render the first frame of the animation
		RenderAnimationFrame();
	}

	private void Stop()
	{
		_currentAnimationKey        = null;
		_currentBodyAnimationClip   = null;
		_currentWeaponAnimationClip = null;
		_currentTabardAnimationClip = null;
		_currentFrameIndex          = 0;
		_currentClipElapsed         = 0f;
		_currentFrameElapsed        = 0f;
		_currentFrameCount          = 0;
		_currentSingleFrameLimit    = 0f;
		_stopWhenFinished           = true;
	}

	public override void Update()
	{
		if (_entity == null)
			return;

		if (!_entity.isDefeated)
		{
			_weaponRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -1000) + 1;
			_tabardRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -1000) + 0;
			_bodyRenderer.sortingOrder   = Mathf.RoundToInt(transform.position.y * -1000) - 1;
		}

		if (string.IsNullOrEmpty(_currentAnimationKey))
			return;

		//update timers
		_currentClipElapsed  += Time.deltaTime;
		_currentFrameElapsed += Time.deltaTime;
		if (_entity.velocity.value == Vector2.zero || _entity.hasKnockback || _entity.hasCombatAction)
			_idleTimerElapsed += Time.deltaTime;
		else
			_idleTimerElapsed = 0f;

		if (_idleTimerElapsed >= .1f && !_entity.hasCombatAction && !_entity.isDefeated)
			Play(CharacterAnimationAction.Idle);

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

		/*FollowPlayer();*/
	}

	private void RenderAnimationFrame()
	{
		if (_currentFrameIndex > _currentFrameCount - 1)
			return;

		if (_bodyRenderer != null)
			_bodyRenderer.sprite = _currentBodyAnimationClip[_currentFrameIndex];
		if (_weaponRenderer != null)
			_weaponRenderer.sprite = _currentWeaponAnimationClip[_currentFrameIndex];
		if (_tabardRenderer != null)
			_tabardRenderer.sprite = _currentTabardAnimationClip[_currentFrameIndex];

		if (_entity.hasInvincible)
			_bodyRenderer.material.color = Color.red;
		else
			_bodyRenderer.material.color = Color.white;
	}

	/*private void FollowPlayer()
	{
		if (_entity != null && _entity.isPlayerControlled)
		{
			var target = gameObject;
			var posNoZ = transform.position;
			posNoZ.z = target.transform.position.z;

			var targetDirection = target.transform.position - posNoZ;

			var interpVelocity = targetDirection.magnitude * 5f;

			var targetPos = transform.position + targetDirection.normalized * interpVelocity * Time.deltaTime;
			var offset    = new Vector3(0, 0, -10);

			Camera.main.transform.position = Vector3.Lerp(transform.position, targetPos + offset, 0.25f);
		}
	}*/

	private string GetAnimationKeyFor(CharacterAnimationAction action, CharacterHeading heading)
	{
		return $"{action.ToSelectorString()}_{heading.ToSelectorString()}";
	}

	#endregion

	#region game events

	public override void OnCombatAction(GameEntity entity, CombatAction action, float elapsed)
	{
		Play(action.action);
	}

	public override void OnCombatActionRemoved(GameEntity entity)
	{
		/*if (!entity.hasInput || entity.input.direction != Vector2.zero)
			return;*/

		Play(CharacterAnimationAction.Idle);
	}

	public override void OnVelocity(GameEntity entity, Vector2 value)
	{
		if (value != Vector2.zero && !entity.hasKnockback)
			_heading = entity.heading;

		if (!entity.hasCombatAction && !entity.isDefeated && !entity.hasStunned)
			Play(CharacterAnimationAction.Walk, _heading);
	}

	public override void OnDefeated(GameEntity entity)
	{
		if (!entity.isDefeated)
			return;

		_weaponRenderer.sortingOrder = _weaponRenderer.sortingOrder - 1000;
		_tabardRenderer.sortingOrder = _tabardRenderer.sortingOrder - 1000;
		_bodyRenderer.sortingOrder   = _bodyRenderer.sortingOrder   - 1000;

		Play(CharacterAnimationAction.Hurt, CharacterHeading.Down);
	}

	#endregion
}
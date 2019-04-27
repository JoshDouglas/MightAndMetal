using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class WeaponComponent : IComponent
{
	public CharacterWeaponType                                       weaponType;
	public float                                                     duration;
	public bool                                                      isBlockable;
	public bool                                                      canReflect;
	public Dictionary<CharacterHeading, CombatColliderInfo> weaponColliders;
	public List<CombatEffect>                                        effects;

	//read-only
	public CharacterAnimationAction animationAction => weaponType.GetAction();

	public int frames =>
		animationAction == CharacterAnimationAction.Slash  ? CharacterAnimations.SlashFrameCount :
		animationAction == CharacterAnimationAction.Thrust ? CharacterAnimations.ThrustFrameCount : 0;

	public float singleFrameTime => duration / frames;

	public float colliderEnabledAt  => singleFrameTime * 2f;
	public float colliderDisabledAt => singleFrameTime * 6f;
}

public class CombatColliderInfo
{
	public Vector2 offset { get; set; }
	public Vector2 size   { get; set; }
}

public static class CharacterWeapon
{
	static readonly Dictionary<CharacterWeaponType, Dictionary<CharacterHeading, CombatColliderInfo>> _weaponColliders;

	static CharacterWeapon()
	{
		_weaponColliders = new Dictionary<CharacterWeaponType, Dictionary<CharacterHeading, CombatColliderInfo>>();

		var weaponTypes = Enum.GetValues(typeof(CharacterWeaponType));

		foreach (CharacterWeaponType wt in weaponTypes)
		{
			_weaponColliders[wt] = new Dictionary<CharacterHeading, CombatColliderInfo>();

			var headings = Enum.GetValues(typeof(CharacterHeading));

			foreach (CharacterHeading h in headings)
			{
				switch (wt)
				{
					case CharacterWeaponType.longsword:
						switch (h)
						{
							case CharacterHeading.Up:
								_weaponColliders[wt].Add(h, new CombatColliderInfo {offset = new Vector2(0f, .4f), size = new Vector2(2.5f, 1f)});
								break;
							case CharacterHeading.Down:
								_weaponColliders[wt].Add(h, new CombatColliderInfo {offset = new Vector2(0f, -1.1f), size = new Vector2(2.5f, 1f)});
								break;
							case CharacterHeading.Left:
								_weaponColliders[wt].Add(h, new CombatColliderInfo {offset = new Vector2(-1.5f, -.7f), size = new Vector2(2f, 1.5f)});
								break;
							case CharacterHeading.Right:
								_weaponColliders[wt].Add(h, new CombatColliderInfo {offset = new Vector2(1.5f, -.7f), size = new Vector2(2f, 1.5f)});
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}

						break;
					case CharacterWeaponType.rapier:
						switch (h)
						{
							case CharacterHeading.Up:
								_weaponColliders[wt].Add(h, new CombatColliderInfo {offset = new Vector2(), size = new Vector2()});
								break;
							case CharacterHeading.Down:
								_weaponColliders[wt].Add(h, new CombatColliderInfo {offset = new Vector2(), size = new Vector2()});
								break;
							case CharacterHeading.Left:
								_weaponColliders[wt].Add(h, new CombatColliderInfo {offset = new Vector2(), size = new Vector2()});
								break;
							case CharacterHeading.Right:
								_weaponColliders[wt].Add(h, new CombatColliderInfo {offset = new Vector2(), size = new Vector2()});
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}

						break;
					case CharacterWeaponType.saber:
						switch (h)
						{
							case CharacterHeading.Up:
								_weaponColliders[wt].Add(h, new CombatColliderInfo {offset = new Vector2(), size = new Vector2()});
								break;
							case CharacterHeading.Down:
								_weaponColliders[wt].Add(h, new CombatColliderInfo {offset = new Vector2(), size = new Vector2()});
								break;
							case CharacterHeading.Left:
								_weaponColliders[wt].Add(h, new CombatColliderInfo {offset = new Vector2(), size = new Vector2()});
								break;
							case CharacterHeading.Right:
								_weaponColliders[wt].Add(h, new CombatColliderInfo {offset = new Vector2(), size = new Vector2()});
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}

						break;
					case CharacterWeaponType.mace:
						switch (h)
						{
							case CharacterHeading.Up:
								_weaponColliders[wt].Add(h, new CombatColliderInfo {offset = new Vector2(), size = new Vector2()});
								break;
							case CharacterHeading.Down:
								_weaponColliders[wt].Add(h, new CombatColliderInfo {offset = new Vector2(), size = new Vector2()});
								break;
							case CharacterHeading.Left:
								_weaponColliders[wt].Add(h, new CombatColliderInfo {offset = new Vector2(), size = new Vector2()});
								break;
							case CharacterHeading.Right:
								_weaponColliders[wt].Add(h, new CombatColliderInfo {offset = new Vector2(), size = new Vector2()});
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}

						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
	}

	public static Dictionary<CharacterHeading, CombatColliderInfo> GetCollidersFor(CharacterWeaponType weaponType)
	{
		return _weaponColliders[weaponType];
	}
}

public enum CharacterWeaponType
{
	longsword,
	rapier,
	saber,
	mace
}

public static class CharacterWeaponTypeExtensions
{
	public static CharacterAnimationAction GetAction(this CharacterWeaponType weaponType)
	{
		/*
		 * todo: no spears, yet :)
		 */
		return CharacterAnimationAction.Slash;
	}
}

public partial class GameEntity
{
	public void AddWeapon(CharacterWeaponType weaponType)
	{
		var index     = GameComponentsLookup.Weapon;
		var component = (WeaponComponent) CreateComponent(index, typeof(WeaponComponent));

		component.weaponType      = weaponType;
		component.duration  = 0.5f;
		component.isBlockable     = true;
		component.canReflect      = false;
		component.weaponColliders = CharacterWeapon.GetCollidersFor(weaponType);
		component.effects         = new List<CombatEffect> {CombatEffects.Get(CombatEffectName.slash)};

		AddComponent(index, component);
	}
}
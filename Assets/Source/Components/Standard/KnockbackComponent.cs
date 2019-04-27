using Entitas;
using UnityEngine;

public class KnockbackComponent : IComponent
{
	public float duration;
	public float elapsed;
	public Vector2 direction;
}

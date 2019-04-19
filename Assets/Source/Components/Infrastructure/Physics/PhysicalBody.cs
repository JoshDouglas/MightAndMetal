using Entitas.Unity;
using Entitas.VisualDebugging.Unity;
using UnityEngine;

public class PhysicalBody : MonoBehaviour, IPhysicalBody, IActingRemovedListener, IActingListener, IVelocityListener, IVelocityRemovedListener
{
	protected BoxCollider2D             _collider;
	
	public virtual void Link(GameEntity entity)
	{
		if (!entity.hasPosition || !entity.hasScale)
			gameObject.DestroyGameObject();

		if (gameObject.GetComponent<EntityLink>() == null)
			gameObject.Link(entity);
		
		entity.AddActingListener(this);
		entity.AddActingRemovedListener(this);
		entity.AddVelocityListener(this);
		entity.AddVelocityRemovedListener(this);

		transform.localPosition = entity.position.value;
		transform.localScale    = entity.scale.value;
		
		_collider        = gameObject.AddComponent<BoxCollider2D>();
		/*_collider.size   = new Vector2(1f, 0.3f);
		_collider.offset = new Vector2(0f, -0.8f);*/

	}
	
	private void OnCollisionEnter2D(Collision2D other)
	{
		Debug.Log($"{name} collided with {other.gameObject.name}");
	}

	public virtual void Update()
	{
	}

	public virtual void OnActingRemoved(GameEntity entity)
	{
	}

	public virtual void OnActing(GameEntity entity, float duration, float elapsed, CombatAbility combatAbility)
	{
	}

	public virtual void OnVelocity(GameEntity entity, Vector2 value)
	{
	}

	public virtual void OnVelocityRemoved(GameEntity entity)
	{
	}
}

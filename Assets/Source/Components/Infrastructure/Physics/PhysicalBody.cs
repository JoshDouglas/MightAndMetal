using Entitas.Unity;
using Entitas.VisualDebugging.Unity;
using UnityEngine;

public class PhysicalBody : MonoBehaviour, IPhysicalBody, ICombatActionListener, ICombatActionRemovedListener, IVelocityListener, IDefeatedListener
{
	public virtual void Link(GameEntity entity)
	{
		if (!entity.hasPosition)
			gameObject.DestroyGameObject();

		if (gameObject.GetComponent<EntityLink>() == null)
			gameObject.Link(entity);
		
		entity.AddCombatActionListener(this);
		entity.AddCombatActionRemovedListener(this);
		entity.AddVelocityListener(this);

		transform.localPosition = entity.position.value;
		transform.localScale    = Vector3.one;
		
		/*_collider        = gameObject.AddComponent<BoxCollider2D>();*/
		/*_collider.size   = new Vector2(1f, 0.3f);
		_collider.offset = new Vector2(0f, -0.8f);*/

	}
	
	private void OnCollisionEnter2D(Collision2D other)
	{
		/*Debug.Log($"{name} collided with {other.gameObject.name}");*/
	}

	public virtual void Update()
	{
	}

	public virtual void OnCombatAction(GameEntity entity, CombatAction action, float elapsed)
	{ }

	public virtual void OnCombatActionRemoved(GameEntity entity)
	{ }


	public virtual void OnDefeated(GameEntity entity)
	{ }

	public virtual void OnVelocity(GameEntity entity, Vector2 value) { }
}

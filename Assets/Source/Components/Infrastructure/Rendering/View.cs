using System.Collections;
using System.Collections.Generic;
using Entitas.Unity;
using Entitas.VisualDebugging.Unity;
using UnityEngine;

public class View : MonoBehaviour, IView, IActingRemovedListener, IDefeatedListener, IActingListener, IVelocityListener, IVelocityRemovedListener
{
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
		entity.AddDefeatedListener(this);

		transform.localPosition = entity.position.value;
		transform.localScale    = entity.scale.value;
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

	public virtual void OnDefeated(GameEntity entity)
	{
	}
}
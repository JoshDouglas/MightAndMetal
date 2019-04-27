using System.Collections;
using System.Collections.Generic;
using Entitas.Unity;
using Entitas.VisualDebugging.Unity;
using UnityEngine;

public class View : MonoBehaviour, IView, ICombatActionListener, ICombatActionRemovedListener, IVelocityListener, IDefeatedListener
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
		entity.AddDefeatedListener(this);

		transform.localPosition = entity.position.value;
		transform.localScale    = Vector3.one;
	}

	public virtual void Update()
	{
	}

	public virtual void OnDefeated(GameEntity entity)
	{ }

	public virtual void OnCombatAction(GameEntity entity, CombatAction action, float elapsed)
	{ }

	public virtual void OnCombatActionRemoved(GameEntity entity)
	{ }

	public virtual void OnVelocity(GameEntity entity, Vector2 value)
	{ }
}
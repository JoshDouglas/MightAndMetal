using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEmitter : MonoBehaviour
{
	public string targetTag;

	private void OnCollisionEnter2D(Collision2D other)
	{
		/*var rb = gameObject.GetComponent<Rigidbody2D>();
		rb.velocity = Vector2.zero;*/
		Debug.Log($"{name} collided with {other.gameObject.name}");
	}

	private void OnCollisionStay2D(Collision2D other)
	{
		/*throw new System.NotImplementedException();*/
	}

	private void OnCollisionExit2D(Collision2D other)
	{
		/*throw new System.NotImplementedException();*/
	}
	
	private void OnTriggerEnter2D(Collider2D other)
	{
		/*ProcessTrigger(other);*/
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		/*ProcessTrigger(other);*/
	}


	/*private void ProcessTrigger(Collider2D other)
	{
		if (other.gameObject.CompareTag(targetTag))
		{
			Debug.Log("triggered");

			if (other.gameObject.tag == "Warrior")
			{
				//what distances?
				var thisCollider = gameObject.GetComponent<BoxCollider2D>();
				var deltaX = thisCollider.transform.position.x - other.transform.position.x;
				var deltaY = thisCollider.transform.position.y - other.transform.position.y;

				//which directions?
				var left = deltaX < 0;
				var up = deltaY > 0;

				//what direction
				Vector2 direction;
				if (Math.Abs(deltaX) > Math.Abs(deltaY))
					direction = left ? Vector2.left : Vector2.right;
				else
					direction = up ? Vector2.up : Vector2.down;

				var entity = (GameEntity) gameObject.GetEntityLink().entity;
				var otherEntity = (GameEntity) other.gameObject.GetEntityLink().entity;
				if (entity.hasCollisions)
					entity.collisions.collisions.Add(new Collision {direction = direction, other = otherEntity});

				/*var collider = gameObject.GetComponent<BoxCollider2D>();
				var entity = (GameEntity) gameObject.GetEntityLink().entity;
				Vector3 contactPoint = collider.contacts[0];
				Vector3 center = collider.bounds.center;

				bool right = contactPoint.x > center.x;
				bool top = contactPoint.y > center.y;

				selfEntity.AddCollision(selfEntity, otherEntity);
				otherEntity.AddCollision(selfEntity, otherEntity);#1#
			}


			/*
			Contexts.sharedInstance.game.CreateEntity(
				.AddCollision(link.entity, targetLink.entity);#1#
		}
	}*/

	/*private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.CompareTag(targetTag))
		{
			var selfRb = GetComponent<Rigidbody2D>();
			var otherRb = other.gameObject.GetComponent<Rigidbody2D>();
			var link = gameObject.GetEntityLink();
			var targetLink = other.gameObject.GetEntityLink();
			
			Contexts.sharedInstance.game.CreateEntity()
				.AddCollision(link.entity, targetLink.entity);
		}
	}

	private void OnCollisionExit2D(Collision other)
	{
		if (other.gameObject.CompareTag(targetTag))
		{
			var selfRb = GetComponent<Rigidbody2D>();
			var otherRb = other.gameObject.GetComponent<Rigidbody2D>();
			selfRb.isKinematic = false;
			otherRb.isKinematic = false;
		}
	}*/
}


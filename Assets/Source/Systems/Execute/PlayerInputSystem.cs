#region

using Entitas;
using UnityEngine;

#endregion

public class PlayerInputSystem : IExecuteSystem
{
	private readonly Contexts _contexts;
	private          float    _elapsedTime;

	public PlayerInputSystem(Contexts contexts)
	{
		_contexts = contexts;
	}

	public void Execute()
	{
		var playerEntities = _contexts.game.GetGroup(GameMatcher.PlayerControlled);
		foreach (var entity in playerEntities)
		{
			if (!entity.hasInput)
				continue;

			var input = entity.input;

			var horizontal = 0;
			var vertical   = 0;

			//touch
			var touchButtonSize = 100;
			var movePosition    = new Vector2(80, Screen.height - 180);
			var attackPosition  = new Vector2(Screen.width      - 180, Screen.height - 180);

			foreach (var touch in Input.touches)
				if (touch.phase == TouchPhase.Began)
				{
					var position = touch.position;

					//attack
					if (position.x >= attackPosition.x && position.x <= attackPosition.x + touchButtonSize && position.y <= attackPosition.y + touchButtonSize && position.y <= attackPosition.y + touchButtonSize)
						input.actionButton1 = true;

					//movement
					if (position.x >= movePosition.x && position.x <= movePosition.x + touchButtonSize && position.y <= movePosition.y + touchButtonSize && position.y <= movePosition.y + touchButtonSize)
					{
						if (position.x <= movePosition.x + touchButtonSize / 2)
							horizontal = -1;
						else
							horizontal = 1;

						if (position.y <= movePosition.y + touchButtonSize / 2)
							vertical = -1;
						else
							vertical = 1;
					}
				}


			//TODO: config based
			/*input.up = Input.GetKey(KeyCode.E);
			input.down = Input.GetKey(KeyCode.D);
			input.left = Input.GetKey(KeyCode.S);
			input.right = Input.GetKey(KeyCode.F);*/
			var usingKinesis = true;
			if (usingKinesis)
			{
				if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.UpArrow))
					vertical = 1;
				if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.DownArrow))
					vertical = -1;
				if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.LeftArrow))
					horizontal = -1;
				if (Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.RightArrow))
					horizontal = 1;
			}
			else
			{
				if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
					vertical += 1;
				if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
					vertical += -1;
				if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
					horizontal += -1;
				if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
					horizontal += 1;
			}

			//force single direction
			/*if (vertical != 0 && horizontal != 0)
				horizontal = 0;*/

			input.direction = new Vector2(horizontal, vertical);

			//todo: set direction based on mouse/touch direction (mouse pos is pixels, player pos is world... therin lies the issue)
			/*if (Input.GetMouseButton(0))
			{
				var mousePosition = (Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition);
				var distance      = mousePosition - entity.position.value;
				
				if (distance.x > distance.y)
				{
					if (distance.x > 0)
						input.direction = Vector2.right;
					else
						input.direction = Vector2.left;
				}
				else
				{
					if (distance.y > 0)
						input.direction = Vector2.up;
					else
						input.direction = Vector2.down;
				}
			}*/

			input.actionButton1  = Input.GetKey(KeyCode.Alpha1) || Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space);
			input.actionButton2  = Input.GetKey(KeyCode.Alpha2);
			input.actionButton3  = Input.GetKey(KeyCode.Alpha3);
			input.actionButton4  = Input.GetKey(KeyCode.Alpha4);
			input.actionButton5  = Input.GetKey(KeyCode.Alpha5);
			input.actionButton6  = Input.GetKey(KeyCode.Alpha6);
			input.actionButton7  = Input.GetKey(KeyCode.Alpha7);
			input.actionButton8  = Input.GetKey(KeyCode.Alpha8);
			input.actionButton9  = Input.GetKey(KeyCode.Alpha9);
			input.itemButton1    = Input.GetKey(KeyCode.Alpha1) && Input.GetKey(KeyCode.LeftControl);
			input.itemButton2    = Input.GetKey(KeyCode.Alpha2) && Input.GetKey(KeyCode.LeftControl);
			input.itemButton3    = Input.GetKey(KeyCode.Alpha3) && Input.GetKey(KeyCode.LeftControl);
			input.itemButton4    = Input.GetKey(KeyCode.Alpha4) && Input.GetKey(KeyCode.LeftControl);
			input.specialButton1 = Input.GetKey(KeyCode.Alpha1) && Input.GetKey(KeyCode.LeftAlt);
			input.specialButton2 = Input.GetKey(KeyCode.Alpha2) && Input.GetKey(KeyCode.LeftAlt);
			input.sprint         = Input.GetKey(KeyCode.LeftShift);
			input.block          = Input.GetMouseButton(0);

			//todo: move to combat system to account for resources?
			input.direction *= 1.5f;
			/*if (input.sprint)
				input.direction *= 1.5f;
			else
				input.direction *= 1f;*/
		}
	}
}
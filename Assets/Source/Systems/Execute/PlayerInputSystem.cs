using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class PlayerInputSystem : IExecuteSystem
{
	readonly Contexts _contexts;
	private float _elapsedTime;

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

			//TODO: config based
			/*input.up = Input.GetKey(KeyCode.E);
			input.down = Input.GetKey(KeyCode.D);
			input.left = Input.GetKey(KeyCode.S);
			input.right = Input.GetKey(KeyCode.F);*/
			var horizontal = 0;
			var vertical = 0;
			if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
				vertical += 1;
			if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
				vertical += -1;
			if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
				horizontal += -1;
			if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
				horizontal += 1;
			input.direction = new Vector2(horizontal, vertical);
			input.actionButton1 = Input.GetKey(KeyCode.Alpha1) || Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space);
			input.actionButton2 = Input.GetKey(KeyCode.Alpha2);
			input.actionButton3 = Input.GetKey(KeyCode.Alpha3);
			input.actionButton4 = Input.GetKey(KeyCode.Alpha4);
			input.actionButton5 = Input.GetKey(KeyCode.Alpha5);
			input.actionButton6 = Input.GetKey(KeyCode.Alpha6);
			input.actionButton7 = Input.GetKey(KeyCode.Alpha7);
			input.actionButton8 = Input.GetKey(KeyCode.Alpha8);
			input.actionButton9 = Input.GetKey(KeyCode.Alpha9);
			input.itemButton1 = Input.GetKey(KeyCode.Alpha1) && Input.GetKey(KeyCode.LeftControl);
			input.itemButton2 = Input.GetKey(KeyCode.Alpha2) && Input.GetKey(KeyCode.LeftControl);
			input.itemButton3 = Input.GetKey(KeyCode.Alpha3) && Input.GetKey(KeyCode.LeftControl);
			input.itemButton4 = Input.GetKey(KeyCode.Alpha4) && Input.GetKey(KeyCode.LeftControl);
			input.specialButton1 = Input.GetKey(KeyCode.Alpha1) && Input.GetKey(KeyCode.LeftAlt);
			input.specialButton2 = Input.GetKey(KeyCode.Alpha2) && Input.GetKey(KeyCode.LeftAlt);
			input.sprint = Input.GetKey(KeyCode.LeftShift);
			input.block = Input.GetMouseButton(0);

			//todo: move to combat system to account for resources?
			if (input.sprint)
				input.direction *= 1.5f;
			else
				input.direction *= 1f;
		}
	}
}


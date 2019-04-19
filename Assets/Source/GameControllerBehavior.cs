using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerBehavior : MonoBehaviour
{
	GameController _gameController;

	void Awake() => _gameController = new GameController(Contexts.sharedInstance);
	void Start() => _gameController.Initialize();
	void Update() => _gameController.Execute();
}

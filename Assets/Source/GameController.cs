#region

using System;
using Entitas;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Random = System.Random;

#endregion

public class GameController
{
	public readonly Contexts _contexts;
	public readonly Systems _systems;

	public GameController(Contexts contexts)
    {
        var random = new Random(DateTime.UtcNow.Millisecond);
        UnityEngine.Random.InitState(random.Next());
        Rand.game = new Rand(random.Next());

        _systems = new GameSystems(contexts);
		_contexts = contexts;
		/*var cameraEntity = contexts.game.CreateEntity();
		cameraEntity.AddCamera(Camera.main.orthographicSize, Camera.main.transform.position);*/
	}

	public void Initialize()
    {
        //match monitor hz
        QualitySettings.vSyncCount = 1;
        _systems.Initialize();
    }

	public void Execute()
    {
        _systems.Execute();
        _systems.Cleanup();
        
    }
}


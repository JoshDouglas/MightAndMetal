using System;
using Entitas;
using UnityEngine;

public class GameController
{
    readonly Systems _systems;

    public GameController(Contexts contexts)
    {
        var random = new System.Random(DateTime.UtcNow.Millisecond);
        UnityEngine.Random.InitState(random.Next());
        Rand.game = new Rand(random.Next());

        _systems = new GameSystems(contexts);
        /*var cameraEntity = contexts.game.CreateEntity();
        cameraEntity.AddCamera(Camera.main.orthographicSize, Camera.main.transform.position);*/
    }

    public void Initialize()
    {
        Application.targetFrameRate = 60;
        _systems.Initialize();
    }

    public void Execute()
    {
        _systems.Execute();
        _systems.Cleanup();
    }
}


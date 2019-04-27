#region

using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

public class GameControllerBehavior : MonoBehaviour
{
	private GameController _gameController;

	private void Awake()
	{
		_gameController = new GameController(Contexts.sharedInstance);
	}

	private void Start()
	{
		_gameController.Initialize();
	}

	private void Update()
	{
		
		//todo: fix later
        if (Input.GetKey(KeyCode.F1))
        {
            _gameController._systems.DeactivateReactiveSystems();
            var environmentParent = GameObject.Find("Environment")?.transform ?? new GameObject("Environment").transform;
            var characterParent   = GameObject.Find("Characters")?.transform  ?? new GameObject("Characters").transform;
            Destroy(environmentParent);
            Destroy(characterParent);
            _gameController._contexts.Reset();
            SceneManager.LoadScene("Main");
        }
        
		_gameController.Execute();
	}
}

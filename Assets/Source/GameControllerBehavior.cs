#region

using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

public class GameControllerBehavior : MonoBehaviour
{
	private Texture2D      _dpad;
	private Texture2D      _attack;
	private GameController _gameController;

	private void Awake()
	{
		/*_dpad = AtlasManager.CharacterTemplateInstance.GetSprite("dpad");*/
		_dpad           = Resources.Load<Texture2D>("Battleground/dpad");
		_attack           = Resources.Load<Texture2D>("Battleground/attack");
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

		FollowPlayer();


		_gameController.Execute();
	}

	private void FollowPlayer()
	{
		var player = GameObject.Find("player");
		if (player != null)
		{
			var target = player;
			var posNoZ = player.transform.position;
			posNoZ.z = target.transform.position.z;

			var targetDirection = target.transform.position - posNoZ;

			var interpVelocity = targetDirection.magnitude * 5f;

			var targetPos = player.transform.position + targetDirection.normalized * interpVelocity * Time.deltaTime;
			var offset    = new Vector3(0, 0, -10);

			Camera.main.transform.position = Vector3.Lerp(player.transform.position, targetPos + offset, 0.25f);
		}
	}

	private void OnGUI()
	{
		GUI.DrawTexture(new Rect(80, Screen.height - 180, 100, 100), _dpad, ScaleMode.ScaleToFit, true);
		GUI.DrawTexture(new Rect(Screen.width - 180, Screen.height - 180, 100, 100), _attack, ScaleMode.ScaleToFit, true);
	}
}
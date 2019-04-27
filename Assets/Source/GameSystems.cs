public sealed class GameSystems : Feature
{
	public GameSystems(Contexts contexts)
	{
		// Init
		Add(new GameInitSystem(contexts));

		// Input
		Add(new PlayerInputSystem(contexts));
		Add(new MouthbreathingAISystem(contexts));

		//Update
		Add(new CombatSystem(contexts));
		Add(new PhysicsSystem(contexts));
		Add(new ViewSystem(contexts));
		Add(new CombatCleanupSystem(contexts));

		// Events (Generated)
		Add(new GameEventSystems(contexts));
		/*Add(new GameStateEventSystems(contexts));*/
	}
}
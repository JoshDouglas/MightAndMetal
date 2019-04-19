public sealed class GameSystems : Feature
{
    public GameSystems(Contexts contexts)
    {
        
        // Init
        Add(new GameInitSystem(contexts));
        /*Add(new WarriorAtlasSetupSystem());
        Add(new WarriorAnimationSetupSystem());*/
        /*Add(new BoardSystem(contexts));
        Add(new BattleSystem(contexts));*/
        /*Add(new BattleSetupSystem(contexts));*/

        // Input
        Add(new PlayerInputSystem(contexts));
        Add(new MouthbreathingAISystem(contexts));
        /*Add(new PlayerInputSystem(contexts));
        Add(new SwordsmanInputSystem(contexts));
        Add(new ArcherInputSystem(contexts));
        Add(new ClericInputSystem(contexts));
        Add(new WarriorInputProcessingSystem(contexts));
        Add(new ProcessCollisionSystem(contexts));*/
        /*Add(new PlayerInputSystem(contexts));
        Add(new SwordsmanInputSystem(contexts));*/

        //Update
        Add(new CombatSystem(contexts));
        Add(new PhysicsSystem(contexts));
        Add(new ViewSystem(contexts));
        /*Add(new WarriorMovementSystem(contexts));
        //TODO: combat system
        Add(new WarriorAbilitySystem(contexts));*/
        /*Add(new WarriorMovementSystem(contexts));
        Add(new WarriorAbilitySystem(contexts));*/

        /*Add(new WarriorChooseStrategySystem(contexts));
        Add(new WarriorCalculateStrategySystem(contexts));
        Add(new WarriorMoveSystem(contexts));*/

        // View
        /*Add(new AddViewSystem(contexts));
        Add(new AddWarriorViewSystem(contexts));
        Add(new RenderWarriorSystem(contexts));*/

        // Events (Generated)
        Add(new GameEventSystems(contexts));
        /*Add(new GameStateEventSystems(contexts));*/
    }
}


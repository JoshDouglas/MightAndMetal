using Entitas;

public class CombatCleanupSystem : ICleanupSystem
{
	private readonly Contexts _contexts;
	
	public CombatCleanupSystem(Contexts contexts)
	{
		_contexts = contexts;
	}
	
	public void Cleanup()
	{
		var characters = _contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.CombatEvents)).GetEntities();
		foreach (var entity in characters)
		{
			entity.RemoveCombatEvents();
		}
	}
}

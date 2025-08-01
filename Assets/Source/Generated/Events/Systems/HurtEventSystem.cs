//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.EventSystemGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed class HurtEventSystem : Entitas.ReactiveSystem<GameEntity> {

    readonly System.Collections.Generic.List<IHurtListener> _listenerBuffer;

    public HurtEventSystem(Contexts contexts) : base(contexts.game) {
        _listenerBuffer = new System.Collections.Generic.List<IHurtListener>();
    }

    protected override Entitas.ICollector<GameEntity> GetTrigger(Entitas.IContext<GameEntity> context) {
        return Entitas.CollectorContextExtension.CreateCollector(
            context, Entitas.TriggerOnEventMatcherExtension.Added(GameMatcher.Hurt)
        );
    }

    protected override bool Filter(GameEntity entity) {
        return entity.hasHurt && entity.hasHurtListener;
    }

    protected override void Execute(System.Collections.Generic.List<GameEntity> entities) {
        foreach (var e in entities) {
            var component = e.hurt;
            _listenerBuffer.Clear();
            _listenerBuffer.AddRange(e.hurtListener.value);
            foreach (var listener in _listenerBuffer) {
                listener.OnHurt(e, component.duration, component.elapsed, component.weaponCollisions, component.projectileCollisions);
            }
        }
    }
}

using Rayark.Mast;
using UnityEngine;

public interface ICardPropertyEntity
{
    CardProperty Property { get; }
    
    int Eval(IGameplayStatusWatcher gameWatcher);
    void UpdateByTiming(IGameplayStatusWatcher gameWatcher, UpdateTiming timing);
    void UpdateIntent(IGameplayStatusWatcher gameWatcher, IIntentAction intent);
    void UpdateResult(IGameplayStatusWatcher gameWatcher, IResultAction result);
}

public abstract class CardPropertyEntity : ICardPropertyEntity
{
    public abstract CardProperty Property { get; }

    public virtual int Eval(IGameplayStatusWatcher gameWatcher)
    {
        return 0;
    }

    public virtual void UpdateByTiming(IGameplayStatusWatcher gameWatcher, UpdateTiming timing) {}
    public virtual void UpdateIntent(IGameplayStatusWatcher gameWatcher, IIntentAction intent) {}
    public virtual void UpdateResult(IGameplayStatusWatcher gameWatcher, IResultAction result) {}
}

public class PreservedPropertyEntity : CardPropertyEntity
{
    public override CardProperty Property => CardProperty.Preserved;

    public PreservedPropertyEntity() { }
}

public class InitialPriorityPropertyEntity : CardPropertyEntity
{
    public override CardProperty Property => CardProperty.InitialPriority;

    public InitialPriorityPropertyEntity() { }
}   

public class ConsumablePropertyEntity : CardPropertyEntity
{
    public override CardProperty Property => CardProperty.Consumable;

    public ConsumablePropertyEntity()
    {
    }
}

public class DisposePropertyEntity : CardPropertyEntity
{
    public override CardProperty Property => CardProperty.Dispose;
    public DisposePropertyEntity()
    {
    }
}

public class AutoDisposePropertyEntity : CardPropertyEntity
{
    public override CardProperty Property => CardProperty.AutoDispose;

    public AutoDisposePropertyEntity()
    {
    }
}

public class SealedPropertyEntity : CardPropertyEntity
{
    public override CardProperty Property => CardProperty.Sealed;

    public SealedPropertyEntity()
    {
    }
}
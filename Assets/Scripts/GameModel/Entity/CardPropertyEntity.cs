using Rayark.Mast;
using UnityEngine;

public interface ICardPropertyEntity
{
    CardProperty Property { get; }
    ICardPropertyUseCountEntity UseCount { get; }
    ICardPropertyValue Value { get; }
    
    void UpdateTiming(GameContextManager contextManager, CardTiming timing);
}

public abstract class CardPropertyEntity : ICardPropertyEntity
{
    public abstract CardProperty Property { get; }
    public ICardPropertyUseCountEntity UseCount { get; protected set; }
    public ICardPropertyValue Value { get; protected set; }

    public CardPropertyEntity(ICardPropertyUseCountEntity lifetime, ICardPropertyValue value)
    {
        UseCount = lifetime;
        Value = value;
    }

    public virtual void UpdateTiming(GameContextManager contextManager, CardTiming timing)
    {
        UseCount.UpdateTiming(contextManager, timing);
    }
}

public class PreservedPropertyEntity : CardPropertyEntity
{
    public override CardProperty Property => CardProperty.Preserved;

    public PreservedPropertyEntity(ICardPropertyUseCountEntity lifetime) : base(lifetime, new NoneValue())
    {
    }
}

public class InitialPriorityPropertyEntity : CardPropertyEntity
{
    public override CardProperty Property => CardProperty.InitialPriority;

    public InitialPriorityPropertyEntity(ICardPropertyUseCountEntity lifetime) : base(lifetime, new NoneValue())
    {
    }
}   

public class ConsumablePropertyEntity : CardPropertyEntity
{
    public override CardProperty Property => CardProperty.Consumable;

    public ConsumablePropertyEntity(ICardPropertyUseCountEntity lifetime) : base(lifetime, new NoneValue())
    {
    }
}

public class DisposePropertyEntity : CardPropertyEntity
{
    public override CardProperty Property => CardProperty.Dispose;
    public DisposePropertyEntity(ICardPropertyUseCountEntity lifetime) : base(lifetime, new NoneValue())
    {
    }
}

public class AutoDisposePropertyEntity : CardPropertyEntity
{
    public override CardProperty Property => CardProperty.AutoDispose;

    public AutoDisposePropertyEntity(ICardPropertyUseCountEntity lifetime) : base(lifetime, new NoneValue())
    {
    }
}

public class SealedPropertyEntity : CardPropertyEntity
{
    public override CardProperty Property => CardProperty.Sealed;

    public SealedPropertyEntity(ICardPropertyUseCountEntity lifetime) : base(lifetime, new NoneValue())
    {
    }
}
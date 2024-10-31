using Rayark.Mast;
using UnityEngine;

public interface ICardPropertyEntity
{
    CardProperty Property { get; }
    ICardPropertyLifetimeEntity Lifetime { get; }
    ICardPropertyValue Value { get; }
}

public abstract class CardPropertyEntity : ICardPropertyEntity
{
    public abstract CardProperty Property { get; }
    public ICardPropertyLifetimeEntity Lifetime { get; protected set; }
    public ICardPropertyValue Value { get; protected set; }

    public CardPropertyEntity(ICardPropertyLifetimeEntity lifetime, ICardPropertyValue value)
    {
        Lifetime = lifetime;
        Value = value;
    }
}

public class PreservedPropertyEntity : CardPropertyEntity
{
    public override CardProperty Property => CardProperty.Preserved;

    public PreservedPropertyEntity(ICardPropertyLifetimeEntity lifetime) : base(lifetime, new NoneValue())
    {
    }
}

public class InitialPriorityPropertyEntity : CardPropertyEntity
{
    public override CardProperty Property => CardProperty.InitialPriority;

    public InitialPriorityPropertyEntity(ICardPropertyLifetimeEntity lifetime) : base(lifetime, new NoneValue())
    {
    }
}   

public class ConsumablePropertyEntity : CardPropertyEntity
{
    public override CardProperty Property => CardProperty.Consumable;

    public ConsumablePropertyEntity(ICardPropertyLifetimeEntity lifetime) : base(lifetime, new NoneValue())
    {
    }
}

public class DisposePropertyEntity : CardPropertyEntity
{
    public override CardProperty Property => CardProperty.Dispose;
    public DisposePropertyEntity(ICardPropertyLifetimeEntity lifetime) : base(lifetime, new NoneValue())
    {
    }
}

public class AutoDisposePropertyEntity : CardPropertyEntity
{
    public override CardProperty Property => CardProperty.AutoDispose;

    public AutoDisposePropertyEntity(ICardPropertyLifetimeEntity lifetime) : base(lifetime, new NoneValue())
    {
    }
}

public class SealedPropertyEntity : CardPropertyEntity
{
    public override CardProperty Property => CardProperty.Sealed;

    public SealedPropertyEntity(ICardPropertyLifetimeEntity lifetime) : base(lifetime, new NoneValue())
    {
    }
}
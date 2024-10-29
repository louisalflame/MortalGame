using Rayark.Mast;
using UnityEngine;

public interface ICardPropertyEntity
{
    CardProperty Property { get; }
    ICardPropertyLifetimeEntity Lifetime { get; }
    ICardPropertyValue Value { get; }
}


public class PreservedPropertyEntity : ICardPropertyEntity
{
    public CardProperty Property => CardProperty.Preserved;
    public ICardPropertyLifetimeEntity Lifetime { get; private set; }
    public ICardPropertyValue Value { get; private set; }

    public PreservedPropertyEntity(ICardPropertyLifetimeEntity lifetime)
    {
        Lifetime = lifetime;
        Value = new NoneValue();
    }
}

public class InitialPriorityPropertyEntity : ICardPropertyEntity
{
    public CardProperty Property => CardProperty.InitialPriority;
    public ICardPropertyLifetimeEntity Lifetime { get; private set; }
    public ICardPropertyValue Value { get; private set; }

    public InitialPriorityPropertyEntity(ICardPropertyLifetimeEntity lifetime)
    {
        Lifetime = lifetime;
        Value = new NoneValue();
    }
}   

public class ConsumablePropertyEntity : ICardPropertyEntity
{
    public CardProperty Property => CardProperty.Consumable;
    public ICardPropertyLifetimeEntity Lifetime { get; private set; }
    public ICardPropertyValue Value { get; private set; }

    public ConsumablePropertyEntity(ICardPropertyLifetimeEntity lifetime)
    {
        Lifetime = lifetime;
        Value = new NoneValue();
    }
}

public class DisposePropertyEntity : ICardPropertyEntity
{
    public CardProperty Property => CardProperty.Dispose;
    public ICardPropertyLifetimeEntity Lifetime { get; private set; }
    public ICardPropertyValue Value { get; private set; }

    public DisposePropertyEntity(ICardPropertyLifetimeEntity lifetime)
    {
        Lifetime = lifetime;
        Value = new NoneValue();
    }
}

public class AutoDisposePropertyEntity : ICardPropertyEntity
{
    public CardProperty Property => CardProperty.AutoDispose;
    public ICardPropertyLifetimeEntity Lifetime { get; private set; }
    public ICardPropertyValue Value { get; private set; }

    public AutoDisposePropertyEntity(ICardPropertyLifetimeEntity lifetime)
    {
        Lifetime = lifetime;
        Value = new NoneValue();
    }
}

public class SealedPropertyEntity : ICardPropertyEntity
{
    public CardProperty Property => CardProperty.Sealed;
    public ICardPropertyLifetimeEntity Lifetime { get; private set; }
    public ICardPropertyValue Value { get; private set; }

    public SealedPropertyEntity(ICardPropertyLifetimeEntity lifetime)
    {
        Lifetime = lifetime;
        Value = new NoneValue();
    }
}
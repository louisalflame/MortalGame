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

    public PreservedPropertyEntity(ICardPropertyLifetimeEntity lifetime, ICardPropertyValue value)
    {
        Lifetime = lifetime;
        Value = value;
    }
}
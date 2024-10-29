using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ICardPropertyData
{
    ICardPropertyEntity CreateEntity();
}

public class PreservedPropertyData : ICardPropertyData
{
    public ICardPropertyLifetimeData Lifetime;

    public ICardPropertyEntity CreateEntity()
    {
        return new PreservedPropertyEntity(Lifetime.CreateEntity());
    }
}

public class InitialPriorityPropertyData : ICardPropertyData
{
    public ICardPropertyLifetimeData Lifetime;

    public ICardPropertyEntity CreateEntity()
    {
        return new InitialPriorityPropertyEntity(Lifetime.CreateEntity());
    }
}

public class ConsumablePropertyData : ICardPropertyData
{
    public ICardPropertyLifetimeData Lifetime;

    public ICardPropertyEntity CreateEntity()
    {
        return new ConsumablePropertyEntity(Lifetime.CreateEntity());
    }
}

public class DisposePropertyData : ICardPropertyData
{
    public ICardPropertyLifetimeData Lifetime;

    public ICardPropertyEntity CreateEntity()
    {
        return new DisposePropertyEntity(Lifetime.CreateEntity());
    }
}

public class AutoDisposePropertyData : ICardPropertyData
{
    public ICardPropertyLifetimeData Lifetime;

    public ICardPropertyEntity CreateEntity()
    {
        return new AutoDisposePropertyEntity(Lifetime.CreateEntity());
    }
}

public class SealedPropertyData : ICardPropertyData
{
    public ICardPropertyLifetimeData Lifetime;

    public ICardPropertyEntity CreateEntity()
    {
        return new SealedPropertyEntity(Lifetime.CreateEntity());
    }
}

public static class CardPropertyDataExtensions
{
    public static bool HasProperty(this CardEntity card, CardProperty property)
    { 
        return card.Properties.Any(p => p.Property == property);
    }
}
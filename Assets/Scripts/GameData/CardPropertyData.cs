using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ICardPropertyData
{
    ICardPropertyEntity CreateEntity();
}

public class PreservedPropertyData : ICardPropertyData
{
    public ICardPropertyUseCountData UseCount;

    public ICardPropertyEntity CreateEntity()
    {
        return new PreservedPropertyEntity(UseCount.CreateEntity());
    }
}

public class InitialPriorityPropertyData : ICardPropertyData
{
    public ICardPropertyUseCountData UseCount;

    public ICardPropertyEntity CreateEntity()
    {
        return new InitialPriorityPropertyEntity(UseCount.CreateEntity());
    }
}

public class ConsumablePropertyData : ICardPropertyData
{
    public ICardPropertyUseCountData UseCount;

    public ICardPropertyEntity CreateEntity()
    {
        return new ConsumablePropertyEntity(UseCount.CreateEntity());
    }
}

public class DisposePropertyData : ICardPropertyData
{
    public ICardPropertyUseCountData UseCount;

    public ICardPropertyEntity CreateEntity()
    {
        return new DisposePropertyEntity(UseCount.CreateEntity());
    }
}

public class AutoDisposePropertyData : ICardPropertyData
{
    public ICardPropertyUseCountData UseCount;

    public ICardPropertyEntity CreateEntity()
    {
        return new AutoDisposePropertyEntity(UseCount.CreateEntity());
    }
}

public class SealedPropertyData : ICardPropertyData
{
    public ICardPropertyUseCountData UseCount;

    public ICardPropertyEntity CreateEntity()
    {
        return new SealedPropertyEntity(UseCount.CreateEntity());
    }
}

public static class CardPropertyDataExtensions
{
    public static bool HasProperty(this ICardEntity card, CardProperty property)
    { 
        return card.AllProperties.Any(p => p.Property == property);
    }
}
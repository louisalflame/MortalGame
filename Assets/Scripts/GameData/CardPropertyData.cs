using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ICardPropertyData
{
    ICardPropertyEntity CreateEntity();
}

public class PreservedPropertyData : ICardPropertyData
{
    public ICardPropertyEntity CreateEntity()
    {
        return new PreservedPropertyEntity();
    }
}

public class InitialPriorityPropertyData : ICardPropertyData
{
    public ICardPropertyEntity CreateEntity()
    {
        return new InitialPriorityPropertyEntity();
    }
}

public class ConsumablePropertyData : ICardPropertyData
{
    public ICardPropertyEntity CreateEntity()
    {
        return new ConsumablePropertyEntity();
    }
}

public class DisposePropertyData : ICardPropertyData
{
    public ICardPropertyEntity CreateEntity()
    {
        return new DisposePropertyEntity();
    }
}

public class AutoDisposePropertyData : ICardPropertyData
{
    public ICardPropertyEntity CreateEntity()
    {
        return new AutoDisposePropertyEntity();
    }
}

public class SealedPropertyData : ICardPropertyData
{
    public ICardPropertyEntity CreateEntity()
    {
        return new SealedPropertyEntity();
    }
}

public static class CardPropertyDataExtensions
{
    public static bool IsConsumable(this ICardEntity card)
    {
        return card.HasProperty(CardProperty.Consumable);
    }
    public static bool IsDisposable(this ICardEntity card)
    {
        return card.HasProperty(CardProperty.Dispose) || card.HasProperty(CardProperty.AutoDispose);
    }

    public static bool HasProperty(this ICardEntity card, CardProperty property)
    { 
        return card.AllProperties.Any(p => p.Property == property);
    }
}
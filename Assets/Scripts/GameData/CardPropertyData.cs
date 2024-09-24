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
    public ICardPropertyValue Value;

    public ICardPropertyEntity CreateEntity()
    {
        return new PreservedPropertyEntity(Lifetime.CreateEntity(), Value);
    }
}

public static class CardPropertyDataExtensions
{
    public static bool HasProperty(this CardEntity card, CardProperty property)
    { 
        return card.Properties.Any(p => p.Property == property);
    }
}
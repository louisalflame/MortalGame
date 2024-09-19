using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardPropertyData
{
    public ICardPropertyLifetimeData Lifetime;
    public ICardPropertyValue Value;

    public CardPropertyEntity CreateEntity()
    {
        return new CardPropertyEntity
        {
            Lifetime = Lifetime.CreateEntity(),
            Value = Value
        };
    }
}

public static class CardPropertyDataExtensions
{
    public static Dictionary<CardProperty, List<CardPropertyEntity>> CreateCardProperties(this CardInstance instance)
    {
        var properties = new Dictionary<CardProperty, List<CardPropertyEntity>>();
        foreach (var pair in instance.PropertyDatas)
        {
            properties.Add(pair.Key, pair.Value.Select(data => data.CreateEntity()).ToList());
        }
        return properties;
    }

    public static bool HasProperty(this CardEntity card, CardProperty property)
    { 
        return card.Properties.ContainsKey(property);
    }
}
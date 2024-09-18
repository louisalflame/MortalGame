using UnityEngine;

public class CardPropertyData
{
    public ICardPropertyLifetime Lifetime;
    public AlgorithmType Algorithm;
    public int InitValue;

    public CardPropertyEntity CreateEntity()
    {
        return new CardPropertyEntity
        {
            Lifetime = Lifetime,
            Algorithm = Algorithm,
            Value = InitValue
        };
    }
}

public static class CardPropertyDataExtensions
{
    public static bool HasProperty(this CardEntity card, CardProperty property)
    { 
        return card.Properties.ContainsKey(property);
    }
}
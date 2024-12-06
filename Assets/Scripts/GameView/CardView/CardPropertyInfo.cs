using UnityEngine;

public class CardPropertyInfo
{
    public CardProperty Property { get; private set; }

    public CardPropertyValueInfo valueInfo { get; private set; }
    public CardPropertyLifetimeInfo lifetimeInfo { get; private set; }

    public string TitleKey { get; private set; }
    public string InfoKey { get; private set; }

    public CardPropertyInfo(ICardPropertyEntity propertyEntity)     
    {
        Property = propertyEntity.Property;
        valueInfo = new CardPropertyValueInfo(propertyEntity.Value);
        lifetimeInfo = new CardPropertyLifetimeInfo(propertyEntity.Lifetime);
    }
}

public class CardPropertyValueInfo
{
    public int Value { get; private set; }
    public bool HasValue { get; private set; }
    public CardPropertyValueInfo(ICardPropertyValue propertyValue)
    {
        switch (propertyValue)
        {
            case ArithmeticValue arithmeticValue:
                Value = arithmeticValue.Value;
                HasValue = true;
                break;
            case NoneValue noneValue:
                Value = 0;
                HasValue = false;
                break;
        }
    }
}

public class CardPropertyLifetimeInfo
{
    public int Lifetime { get; private set; }
    public bool HasValue { get; private set; }

    public CardPropertyLifetimeInfo(ICardPropertyLifetimeEntity lifetime)
    {
        switch (lifetime)
        {
            case CardPropertyAlwaysEntity alwaysEntity:
                Lifetime = -1;
                HasValue = false;
                break;
            case CardPropertyTurnCountEntity turnCountEntity:
                Lifetime = turnCountEntity.TurnCount;
                HasValue = true;
                break;
        }
    }
}

using UnityEngine;

public interface ICardPropertyLifetimeData
{
    ICardPropertyLifetimeEntity CreateEntity();
}

public class CardPropertyAlwaysData : ICardPropertyLifetimeData
{
    public ICardPropertyLifetimeEntity CreateEntity()
    {
        return new CardPropertyAlwaysEntity();
    }
}

public class CardPropertyTurnCountData : ICardPropertyLifetimeData
{
    public int TurnCount;

    public ICardPropertyLifetimeEntity CreateEntity()
    {
        return new CardPropertyTurnCountEntity
        {
            TurnCount = TurnCount
        };
    }
}

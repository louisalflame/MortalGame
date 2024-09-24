using UnityEngine;

public interface ICardPropertyLifetimeEntity
{
    void UpdateTiming(GameContextManager contextManager, CardTiming cardTiming);
    bool IsExpired { get; }
}
public class CardPropertyAlwaysEntity : ICardPropertyLifetimeEntity
{
    public bool IsExpired => false;
    public void UpdateTiming(GameContextManager contextManager, CardTiming cardTiming) { }
}

public class CardPropertyTurnCountEntity : ICardPropertyLifetimeEntity
{
    public int TurnCount;
    public bool IsExpired => TurnCount <= 0;

    public void UpdateTiming(GameContextManager contextManager, CardTiming cardTiming)
    {
        if (cardTiming == CardTiming.TurnEnd)
        {
            --TurnCount;
        }
    }
}
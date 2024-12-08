using UnityEngine;

public interface ICardStatusLifeTimeEntity
{
    bool IsExpired();
    void UpdateTiming(GameContextManager contextManager, CardTiming timing);
}

public class CardStatusAlwaysLifeTimeEntity : ICardStatusLifeTimeEntity
{
    public bool IsExpired()
    {
        return false;
    }

    public void UpdateTiming(GameContextManager contextManager, CardTiming timing)
    {
    }
}

public class CardStatusTurnLifeTimeEntity : ICardStatusLifeTimeEntity
{
    private int _turn;

    public CardStatusTurnLifeTimeEntity(int turn)
    {
        _turn = turn;
    }

    public bool IsExpired()
    {
        return _turn <= 0;
    }

    public void UpdateTiming(GameContextManager contextManager, CardTiming timing)
    {
        if (timing == CardTiming.TurnEnd)
        {
            _turn--;
        }
    }
}
using UnityEngine;

public interface ICardStatusLifeTimeEntity
{
    bool IsExpired();
    void UpdateTiming(IGameplayStatusWatcher gameWatcher, GameTiming timing);
}

public class CardStatusAlwaysLifeTimeEntity : ICardStatusLifeTimeEntity
{
    public bool IsExpired()
    {
        return false;
    }

    public void UpdateTiming(IGameplayStatusWatcher gameWatcher, GameTiming timing)
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

    public void UpdateTiming(IGameplayStatusWatcher gameWatcher, GameTiming timing)
    {
        if (timing == GameTiming.TurnEnd)
        {
            _turn--;
        }
    }
}
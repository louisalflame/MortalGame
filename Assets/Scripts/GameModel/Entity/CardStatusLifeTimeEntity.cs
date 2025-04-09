using UnityEngine;

public interface ICardStatusLifeTimeEntity
{
    bool IsExpired();
    void Update(IGameplayStatusWatcher gameWatcher);
}

public class AlwaysLifeTimeCardStatusEntity : ICardStatusLifeTimeEntity
{
    public bool IsExpired()
    {
        return false;
    }

    public void Update(IGameplayStatusWatcher gameWatcher)
    {
    }
}

public class TurnLifeTimeCardStatusEntity : ICardStatusLifeTimeEntity
{
    private int _turn;

    public TurnLifeTimeCardStatusEntity(int turn)
    {
        _turn = turn;
    }

    public bool IsExpired()
    {
        return _turn <= 0;
    }

    public void Update(IGameplayStatusWatcher gameWatcher)
    {
        if (gameWatcher.GameContext.GameTiming == GameTiming.TurnEnd)
        {
            _turn--;
        }
    }
}
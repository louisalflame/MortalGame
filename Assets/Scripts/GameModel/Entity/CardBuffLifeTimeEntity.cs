using UnityEngine;

public interface ICardBuffLifeTimeEntity
{
    bool IsExpired();
    void Update(IGameplayStatusWatcher gameWatcher);
}

public class AlwaysLifeTimeCardBuffEntity : ICardBuffLifeTimeEntity
{
    public bool IsExpired()
    {
        return false;
    }

    public void Update(IGameplayStatusWatcher gameWatcher)
    {
    }
}

public class TurnLifeTimeCardBuffEntity : ICardBuffLifeTimeEntity
{
    private int _turn;

    public TurnLifeTimeCardBuffEntity(int turn)
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
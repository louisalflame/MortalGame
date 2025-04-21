using UnityEngine;

public interface ICharacterBuffLifeTimeEntity
{
    bool IsExpired();
    void Update(IGameplayStatusWatcher gameWatcher);
}

public class AlwaysLifeTimeCharacterBuffEntity : ICharacterBuffLifeTimeEntity
{
    public bool IsExpired()
    {
        return false;
    }

    public void Update(IGameplayStatusWatcher gameWatcher)
    {
    }
}

public class TurnLifeTimeCharacterBuffEntity : ICharacterBuffLifeTimeEntity
{
    private int _turn;

    public TurnLifeTimeCharacterBuffEntity(int turn)
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
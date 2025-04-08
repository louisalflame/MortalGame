using UnityEngine;

public interface IPlayerBuffLifeTimeEntity
{
    bool IsExpired();
    void UpdateTiming(IGameplayStatusWatcher gameWatcher, GameTiming timing);
}

public class PlayerBuffAlwaysLifeTimeEntity : IPlayerBuffLifeTimeEntity
{
    public bool IsExpired()
    {
        return false;
    }

    public void UpdateTiming(IGameplayStatusWatcher gameWatcher, GameTiming timing)
    {
    }
}

public class PlayerBuffTurnLifeTimeEntity : IPlayerBuffLifeTimeEntity
{
    private int _turn;

    public PlayerBuffTurnLifeTimeEntity(int turn)
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
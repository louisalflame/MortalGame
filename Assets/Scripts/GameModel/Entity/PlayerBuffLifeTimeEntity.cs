using UnityEngine;

public interface IPlayerBuffLifeTimeEntity
{
    bool IsExpired();
    void UpdateByTiming(IGameplayStatusWatcher gameWatcher, UpdateTiming timing);
    void UpdateIntent(IGameplayStatusWatcher gameWatcher, IIntentAction intent);
    void UpdateResult(IGameplayStatusWatcher gameWatcher, IResultAction result);
}

public class AlwaysLifeTimePlayerBuffEntity : IPlayerBuffLifeTimeEntity
{
    public bool IsExpired()
    {
        return false;
    }

    public void UpdateByTiming(IGameplayStatusWatcher gameWatcher, UpdateTiming timing) { }
    public void UpdateIntent(IGameplayStatusWatcher gameWatcher, IIntentAction intent) { }
    public void UpdateResult(IGameplayStatusWatcher gameWatcher, IResultAction result) { }
}

public class TurnLifeTimePlayerBuffEntity : IPlayerBuffLifeTimeEntity
{
    private int _turn;

    public TurnLifeTimePlayerBuffEntity(int turn)
    {
        _turn = turn;
    }

    public bool IsExpired()
    {
        return _turn <= 0;
    }

    public void UpdateByTiming(IGameplayStatusWatcher gameWatcher, UpdateTiming timing)
    {
        if (timing == UpdateTiming.TurnEnd)
        {
            _turn--;
        }
    }
    public void UpdateIntent(IGameplayStatusWatcher gameWatcher, IIntentAction intent) { }
    public void UpdateResult(IGameplayStatusWatcher gameWatcher, IResultAction result) { }
}
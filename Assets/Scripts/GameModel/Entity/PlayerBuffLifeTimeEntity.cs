using UnityEngine;

public interface IPlayerBuffLifeTimeEntity
{
    bool IsExpired();
    void UpdateByTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing);
    void UpdateIntent(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IIntentAction intent);
    void UpdateResult(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IResultAction result);
}

public class AlwaysLifeTimePlayerBuffEntity : IPlayerBuffLifeTimeEntity
{
    public bool IsExpired()
    {
        return false;
    }

    public void UpdateByTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing) { }
    public void UpdateIntent(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IIntentAction intent) { }
    public void UpdateResult(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IResultAction result) { }
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

    public void UpdateByTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing)
    {
        if (timing == UpdateTiming.TurnEnd)
        {
            _turn--;
        }
    }
    public void UpdateIntent(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IIntentAction intent) { }
    public void UpdateResult(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IResultAction result) { }
}
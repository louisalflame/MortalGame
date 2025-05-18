using UnityEngine;

public interface ICharacterBuffLifeTimeEntity
{
    bool IsExpired();
    void UpdateTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing);
    void UpdateIntent(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IIntentAction intent);
    void UpdateResult(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IResultAction result);
}

public class AlwaysLifeTimeCharacterBuffEntity : ICharacterBuffLifeTimeEntity
{
    public bool IsExpired()
    {
        return false;
    }

    public void UpdateTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing) { }

    public void UpdateIntent(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IIntentAction intent) { }

    public void UpdateResult(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IResultAction result) { }
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

    public void UpdateTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing) 
    { 
        if (timing == global::UpdateTiming.TurnEnd)
        {
            _turn--;
        }
    }

    public void UpdateIntent(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IIntentAction intent) { }

    public void UpdateResult(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IResultAction result) { }
}
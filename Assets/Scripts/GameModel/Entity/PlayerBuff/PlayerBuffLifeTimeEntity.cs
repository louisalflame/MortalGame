using UnityEngine;

public interface IPlayerBuffLifeTimeEntity
{
    bool IsExpired();
    bool Update(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit);
}

public class AlwaysLifeTimePlayerBuffEntity : IPlayerBuffLifeTimeEntity
{
    public bool IsExpired()
    {
        return false;
    }

    public bool Update(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        return false;
    }
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

    public bool Update(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        if (actionUnit is UpdateTimingAction timingAction &&
            timingAction.Timing == GameTiming.TurnEnd)
        {
            _turn--;
            return true;
        }
        return false;
    }
}
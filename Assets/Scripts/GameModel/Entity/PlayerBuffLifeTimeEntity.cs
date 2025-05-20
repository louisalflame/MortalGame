using UnityEngine;

public interface IPlayerBuffLifeTimeEntity
{
    bool IsExpired();
    void Update(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit);
}

public class AlwaysLifeTimePlayerBuffEntity : IPlayerBuffLifeTimeEntity
{
    public bool IsExpired()
    {
        return false;
    }

    public void Update(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit) { }
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

    public void Update(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        if (actionUnit is UpdateTimingAction timingAction &&
            timingAction.Timing == UpdateTiming.TurnEnd)
        {
            _turn--;
        }
    }
}
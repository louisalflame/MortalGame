using UnityEngine;

public interface ICharacterBuffLifeTimeEntity
{
    bool IsExpired();
    bool Update(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit);
}

public class AlwaysLifeTimeCharacterBuffEntity : ICharacterBuffLifeTimeEntity
{
    public bool IsExpired()
    {
        return false;
    }

    public bool Update(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
        => false;
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

    public bool Update(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        if (actionUnit is UpdateTimingAction timingAction &&
            timingAction.Timing == UpdateTiming.TurnEnd)
        {
            _turn--;
            return true;
        }
        return false;
    }
}

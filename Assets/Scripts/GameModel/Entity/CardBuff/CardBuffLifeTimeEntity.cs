using System.Linq;
using UnityEngine;

public interface ICardBuffLifeTimeEntity
{
    bool IsExpired();
    bool Update(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit);
}

public class AlwaysLifeTimeCardBuffEntity : ICardBuffLifeTimeEntity
{
    public bool IsExpired()
    {
        return false;
    }

    public bool Update(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
        => false;
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

public class HandCardLifeTimeCardBuffEntity : ICardBuffLifeTimeEntity
{
    private bool _expired;
    public HandCardLifeTimeCardBuffEntity()
    { 
        _expired = false;
    }

    public bool IsExpired()
    {
        return _expired;
    }

    public bool Update(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        if (!_CheckInHandCard(gameWatcher))
        {
            _expired = true;
            return true;
        }
        return false;
    }

    private bool _CheckInHandCard(IGameplayStatusWatcher gameWatcher)
    {
        foreach (var card in gameWatcher.GameStatus.Ally.CardManager.HandCard.Cards
            .Concat(gameWatcher.GameStatus.Enemy.CardManager.HandCard.Cards))
        {
            foreach (var buff in card.BuffManager.Buffs)
            {
                if (buff.LifeTime == this)
                    return true;
            }
        }
        return false;
    }
}
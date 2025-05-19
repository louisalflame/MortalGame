using System.Linq;
using UnityEngine;

public interface ICardBuffLifeTimeEntity
{
    bool IsExpired();
    void UpdateByTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing);
    void UpdateIntent(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IIntentAction intent);
    void UpdateResult(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IResultAction result);
}

public class AlwaysLifeTimeCardBuffEntity : ICardBuffLifeTimeEntity
{
    public bool IsExpired()
    {
        return false;
    }

    public void UpdateByTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing) { }

    public void UpdateIntent(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IIntentAction intent) { }

    public void UpdateResult(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IResultAction result) { }
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

    public void UpdateByTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing)
    { 
        if (!_CheckInHandCard(gameWatcher))
        {
            _expired = true;
        }
    }
    public void UpdateIntent(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IIntentAction intent)
    {        
        if (!_CheckInHandCard(gameWatcher))
        {
            _expired = true;
        }
    }
    public void UpdateResult(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IResultAction result)
    {
        if (!_CheckInHandCard(gameWatcher))
        {
            _expired = true;
        }
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
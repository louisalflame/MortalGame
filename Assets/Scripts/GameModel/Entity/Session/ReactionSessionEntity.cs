using System;
using System.Collections.Generic;
using System.Diagnostics;
using Optional;


public interface IReactionSessionEntity
{
    bool IsSessionValueUpdated { get; }
    Option<bool> BooleanValue { get; }
    Option<int> IntegerValue { get; }

    void Update(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit);
}

public class ReactionSessionEntity : IReactionSessionEntity
{
    private readonly SessionLifeTime _lifeTime;
    private readonly ISessionValueEntity _baseEntity;
    private Option<ISessionValueEntity> _currentValue;

    public bool IsSessionValueUpdated => _currentValue.HasValue;
    public Option<bool> BooleanValue => _currentValue.Match(
        value =>
            value is SessionBooleanEntity booleanEntity ?
                booleanEntity.Value.Some() :
                Option.None<bool>(),
        () => Option.None<bool>());
    public Option<int> IntegerValue => _currentValue.Match(
        value =>
            value is SessionIntegerEntity integerEntity ?
                integerEntity.Value.Some() :
                Option.None<int>(),
        () => Option.None<int>());

    public ReactionSessionEntity(ISessionValueEntity entity, SessionLifeTime lifeTime)
    {
        _baseEntity = entity;
        _currentValue = Option.None<ISessionValueEntity>();
        _lifeTime = lifeTime;

        if (lifeTime != SessionLifeTime.PlayCard)
        {
            _Reset();
        }
    }

    public void Update(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        if (actionUnit is UpdateTimingAction timingAction)
        { 
            switch (_lifeTime)
            {
                case SessionLifeTime.WholeTurn:
                    if (timingAction.Timing == UpdateTiming.TurnStart) _Reset();
                    else if (timingAction.Timing == UpdateTiming.TurnEnd) _Clear();
                    break;
                case SessionLifeTime.PlayCard:
                    if (timingAction.Timing == UpdateTiming.PlayCardStart) _Reset();
                    else if (timingAction.Timing == UpdateTiming.PlayCardEnd) _Clear();
                    break;
            }
        }

        _currentValue.MatchSome(value =>
            value.Update(gameWatcher, trigger, actionUnit));
    }

    private void _Reset()
    {
        _currentValue = _baseEntity.Clone().Some();
    }
    private void _Clear()
    {
        _currentValue = Option.None<ISessionValueEntity>();
    }
}
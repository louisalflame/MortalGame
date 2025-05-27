using System;
using System.Collections.Generic;
using System.Diagnostics;
using Optional;


public interface IReactionSessionEntity
{
    bool IsSessionValueUpdated(string key);
    Option<bool> GetSessionBoolean(string key);
    Option<int> GetSessionInteger(string key);

    void Update(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit);
}

public abstract class ReactionSessionEntity : IReactionSessionEntity
{
    protected readonly IReadOnlyDictionary<string, ISessionValueEntity> _baseEntities;
    protected readonly Dictionary<string, ISessionValueEntity> _values;

    public ReactionSessionEntity(Dictionary<string, ISessionValueEntity> values)
    {
        _baseEntities = values;
        _values = new Dictionary<string, ISessionValueEntity>();
    }

    public bool IsSessionValueUpdated(string key)
    {
        return
            _values.TryGetValue(key, out var valueEntity) &&
            valueEntity.IsUpdated;
    }
    public Option<bool> GetSessionBoolean(string key)
    {
        if (_values.TryGetValue(key, out var valueEntity) &&
            valueEntity is SessionBooleanEntity booleanEntity)
        {
            return booleanEntity.Value;
        }
        return Option.None<bool>();
    }
    public Option<int> GetSessionInteger(string key)
    {
        if (_values.TryGetValue(key, out var valueEntity) &&
            valueEntity is SessionIntegerEntity integerEntity)
        {
            return integerEntity.Value;
        }
        return Option.None<int>();
    }

    public abstract void Update(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit);

    protected virtual void _Update(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        foreach (var kvp in _values)
        {
            kvp.Value.Update(gameWatcher, trigger, actionUnit);
        }
    }

    protected void _Reset()
    {
        foreach (var kvp in _baseEntities)
        {
            _values[kvp.Key] = kvp.Value.Clone();
        }
    }
    protected void _Clear()
    {
        _values.Clear();
    }
}

public class WholeGameSessionEntity : ReactionSessionEntity
{
    public WholeGameSessionEntity(Dictionary<string, ISessionValueEntity> values) : base(values) 
    {
        _Reset();
    }

    public override void Update(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        _Update(gameWatcher, trigger, actionUnit);
    }
}

public class WholeTurnSessionEntity : ReactionSessionEntity
{
    public WholeTurnSessionEntity(Dictionary<string, ISessionValueEntity> values) : base(values)
    { 
        _Reset();
    }

    public override void Update(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        if (actionUnit is UpdateTimingAction timingAction)
        {
            if (timingAction.Timing == UpdateTiming.TurnStart)
            {
                _Reset();
            }
            else if (timingAction.Timing == UpdateTiming.TurnEnd)
            {
                _Clear();
            }
        }

        _Update(gameWatcher, trigger, actionUnit);
    }
}

public class ExectueTurnSessionEntity : ReactionSessionEntity
{
    public ExectueTurnSessionEntity(Dictionary<string, ISessionValueEntity> values) : base(values)
    { 
        _Reset();
    }

    public override void Update(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        if (actionUnit is UpdateTimingAction timingAction)
        {
            if (timingAction.Timing == UpdateTiming.ExecuteStart)
            {
                _Reset();
            }
            else if (timingAction.Timing == UpdateTiming.ExecuteEnd)
            {
                _Clear();
            }
        }

        _Update(gameWatcher, trigger, actionUnit);
    }
}

public class PlayCardSessionEntity : ReactionSessionEntity
{
    public PlayCardSessionEntity(Dictionary<string, ISessionValueEntity> values) : base(values) { }    

    public override void Update(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        if (actionUnit is UpdateTimingAction timingAction)
        {
            if (timingAction.Timing == UpdateTiming.PlayCardStart)
            {
                _Reset();
            }
            else if (timingAction.Timing == UpdateTiming.PlayCardEnd)
            {
                _Clear();
            }
        }

        _Update(gameWatcher, trigger, actionUnit);
    }
}

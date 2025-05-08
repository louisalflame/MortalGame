using System;
using System.Collections.Generic;
using System.Diagnostics;
using Optional;


public interface IReactionSessionEntity
{
    Option<bool> GetSessionBoolean(string key);
    Option<int> GetSessionInteger(string key);

    void UpdateTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing);
    void UpdateIntent(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IIntentAction intent);
    void UpdateResult(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IResultAction result);
}

public abstract class ReactionSessionEntity : IReactionSessionEntity
{
    protected Dictionary<string, ISessionValueEntity> _baseEntities;
    protected Dictionary<string, ISessionValueEntity> _values;

    public ReactionSessionEntity(Dictionary<string, ISessionValueEntity> values)
    {
        _baseEntities = values;
        _values = new Dictionary<string, ISessionValueEntity>();
    }

    public Option<bool> GetSessionBoolean(string key)
    {
        if (_values.TryGetValue(key, out var valueEntity) &&
            valueEntity is SessionBooleanEntity booleanEntity)
        {
            return booleanEntity.Value.SomeNotNull();
        }
        return Option.None<bool>();
    }
    public Option<int> GetSessionInteger(string key)
    {
        if (_values.TryGetValue(key, out var valueEntity) &&
            valueEntity is SessionIntegerEntity integerEntity)
        {
            return integerEntity.Value.SomeNotNull();
        }
        return Option.None<int>();
    }

    public abstract void UpdateTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing);
    public abstract void UpdateIntent(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IIntentAction intent);
    public abstract void UpdateResult(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IResultAction result);

    protected virtual void _UpdateByTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing)
    {
        foreach (var kvp in _values)
        {
            kvp.Value.UpdateByTiming(gameWatcher, trigger, timing);
        }
    }
    protected virtual void _UpdateIntent(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IIntentAction intent)
    {
        foreach (var kvp in _values)
        {
            kvp.Value.UpdateIntent(gameWatcher, trigger, intent);
        }
    }
    protected virtual void _UpdateResult(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IResultAction result)
    {
        foreach (var kvp in _values)
        {
            kvp.Value.UpdateResult(gameWatcher, trigger, result);
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

    public override void UpdateTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing)
    {
        _UpdateByTiming(gameWatcher, trigger, timing);
    }
    public override void UpdateIntent(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IIntentAction intent)
    {
        _UpdateIntent(gameWatcher, trigger, intent);
    }
    public override void UpdateResult(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IResultAction result)
    {
        _UpdateResult(gameWatcher, trigger, result);
    }
}

public class WholeTurnSessionEntity : ReactionSessionEntity
{
    public WholeTurnSessionEntity(Dictionary<string, ISessionValueEntity> values) : base(values) { }

    public override void UpdateTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing)
    {
        if (timing == global::UpdateTiming.TurnStart)
        {
            _Reset();
        }
        else if (timing == global::UpdateTiming.TurnEnd)
        {
            _Clear();
        }

        _UpdateByTiming(gameWatcher, trigger, timing);
    }
    public override void UpdateIntent(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IIntentAction intent)
    {
        _UpdateIntent(gameWatcher, trigger, intent);
    }
    public override void UpdateResult(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IResultAction result)
    {
        _UpdateResult(gameWatcher, trigger, result);
    }
}

public class ExectueTurnSessionEntity : ReactionSessionEntity
{
    public ExectueTurnSessionEntity(Dictionary<string, ISessionValueEntity> values) : base(values) { }

    public override void UpdateTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing)
    {
        if (timing == global::UpdateTiming.ExecuteStart)
        {
            _Reset();
        }
        else if (timing == global::UpdateTiming.ExecuteEnd)
        {
            _Clear();
        }

        _UpdateByTiming(gameWatcher, trigger, timing);
    }
    public override void UpdateIntent(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IIntentAction intent)
    {
        _UpdateIntent(gameWatcher, trigger, intent);
    }
    public override void UpdateResult(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IResultAction result)
    {
        _UpdateResult(gameWatcher, trigger, result);
    }
}

public class PlayCardSessionEntity : ReactionSessionEntity
{
    public PlayCardSessionEntity(Dictionary<string, ISessionValueEntity> values) : base(values) { }    

    public override void UpdateTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing)
    {
        if (timing == global::UpdateTiming.PlayCardStart)
        {
            _Reset();
        }
        else if (timing == global::UpdateTiming.PlayCardEnd)
        {
            _Clear();
        }

        _UpdateByTiming(gameWatcher, trigger, timing);
    }
    public override void UpdateIntent(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IIntentAction intent)
    {
        _UpdateIntent(gameWatcher, trigger, intent);
    }
    public override void UpdateResult(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IResultAction result)
    {
        _UpdateResult(gameWatcher, trigger, result);
    }
}

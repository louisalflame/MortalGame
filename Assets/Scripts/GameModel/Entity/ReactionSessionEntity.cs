using System;
using System.Collections.Generic;
using System.Diagnostics;


public interface IReactionSessionEntity
{
    string Id { get; }
    void UpdateTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing);
    void UpdateIntent(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IIntentAction intent);
    void UpdateResult(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IResultAction result);
}

public abstract class ReactionSessionEntity : IReactionSessionEntity
{
    private string _id;
    public string Id => _id;

    protected Dictionary<string, ISessionValueEntity> _baseEntities;
    protected Dictionary<string, ISessionValueEntity> _values;

    public ReactionSessionEntity(string id, Dictionary<string, ISessionValueEntity> values)
    {
        _id = id;
        _baseEntities = values;
        _values = new Dictionary<string, ISessionValueEntity>();
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
    public WholeGameSessionEntity(string id, Dictionary<string, ISessionValueEntity> values) : base(id, values) 
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
    public WholeTurnSessionEntity(string id, Dictionary<string, ISessionValueEntity> values) : base(id, values) { }

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
    public ExectueTurnSessionEntity(string id, Dictionary<string, ISessionValueEntity> values) : base(id, values) { }

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
    public PlayCardSessionEntity(string id, Dictionary<string, ISessionValueEntity> values) : base(id, values) { }    

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

using System;
using System.Collections.Generic;


public interface IReactionSessionEntity
{
    string Id { get; }
    void UpdateByTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing);
    void UpdateIntent(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IIntentAction intent);
    void UpdateResult(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IResultAction result);
}

public abstract class ReactionSessionEntity : IReactionSessionEntity
{
    private string _id;
    public string Id => _id;

    public Dictionary<string, ISessionValueEntity> Values;

    public ReactionSessionEntity(string id, Dictionary<string, ISessionValueEntity> values)
    {
        _id = id;
        Values = values;
    }

    public abstract void UpdateByTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing);
    public abstract void UpdateIntent(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IIntentAction intent);
    public abstract void UpdateResult(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IResultAction result);

    protected virtual void _UpdateByTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing)
    {
        foreach (var kvp in Values)
        {
            kvp.Value.UpdateByTiming(gameWatcher, trigger, timing);
        }
    }
    protected virtual void _UpdateIntent(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IIntentAction intent)
    {
        foreach (var kvp in Values)
        {
            kvp.Value.UpdateIntent(gameWatcher, trigger, intent);
        }
    }
    protected virtual void _UpdateResult(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IResultAction result)
    {
        foreach (var kvp in Values)
        {
            kvp.Value.UpdateResult(gameWatcher, trigger, result);
        }
    }
}

public class WholeGameSessionEntity : ReactionSessionEntity
{
    public WholeGameSessionEntity(string id, Dictionary<string, ISessionValueEntity> values) : base(id, values) { }

    public override void UpdateByTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing)
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

    public override void UpdateByTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing)
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

public class ExectueTurnSessionEntity : ReactionSessionEntity
{
    public ExectueTurnSessionEntity(string id, Dictionary<string, ISessionValueEntity> values) : base(id, values) { }

    public override void UpdateByTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing)
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

public class PlayCardSessionEntity : ReactionSessionEntity
{
    public PlayCardSessionEntity(string id, Dictionary<string, ISessionValueEntity> values) : base(id, values) { }

    public override void UpdateByTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing)
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

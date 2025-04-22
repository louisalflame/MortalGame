using System;
using System.Collections.Generic;


public interface IReactionSessionEntity
{
    string Id { get; }
    void UpdateByTiming(IGameplayStatusWatcher gameWatcher, UpdateTiming timing);
    void UpdateIntent(IGameplayStatusWatcher gameWatcher, IIntentAction intent);
    void UpdateResult(IGameplayStatusWatcher gameWatcher, IResultAction result);
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

    public abstract void UpdateByTiming(IGameplayStatusWatcher gameWatcher, UpdateTiming timing);
    public abstract void UpdateIntent(IGameplayStatusWatcher gameWatcher, IIntentAction intent);
    public abstract void UpdateResult(IGameplayStatusWatcher gameWatcher, IResultAction result);

    protected virtual void _UpdateByTiming(IGameplayStatusWatcher gameWatcher, UpdateTiming timing)
    {
        foreach (var kvp in Values)
        {
            kvp.Value.UpdateByTiming(gameWatcher, timing);
        }
    }
    protected virtual void _UpdateIntent(IGameplayStatusWatcher gameWatcher, IIntentAction intent)
    {
        foreach (var kvp in Values)
        {
            kvp.Value.UpdateIntent(gameWatcher, intent);
        }
    }
    protected virtual void _UpdateResult(IGameplayStatusWatcher gameWatcher, IResultAction result)
    {
        foreach (var kvp in Values)
        {
            kvp.Value.UpdateResult(gameWatcher, result);
        }
    }
}

public class WholeGameSessionEntity : ReactionSessionEntity
{
    public WholeGameSessionEntity(string id, Dictionary<string, ISessionValueEntity> values) : base(id, values) { }

    public override void UpdateByTiming(IGameplayStatusWatcher gameWatcher, UpdateTiming timing)
    {
        _UpdateByTiming(gameWatcher, timing);
    }
    public override void UpdateIntent(IGameplayStatusWatcher gameWatcher, IIntentAction intent)
    {
        _UpdateIntent(gameWatcher, intent);
    }
    public override void UpdateResult(IGameplayStatusWatcher gameWatcher, IResultAction result)
    {
        _UpdateResult(gameWatcher, result);
    }
}

public class WholeTurnSessionEntity : ReactionSessionEntity
{
    public WholeTurnSessionEntity(string id, Dictionary<string, ISessionValueEntity> values) : base(id, values) { }

    public override void UpdateByTiming(IGameplayStatusWatcher gameWatcher, UpdateTiming timing)
    {
        _UpdateByTiming(gameWatcher, timing);
    }
    public override void UpdateIntent(IGameplayStatusWatcher gameWatcher, IIntentAction intent)
    {
        _UpdateIntent(gameWatcher, intent);
    }
    public override void UpdateResult(IGameplayStatusWatcher gameWatcher, IResultAction result)
    {
        _UpdateResult(gameWatcher, result);
    }
}

public class ExectueTurnSessionEntity : ReactionSessionEntity
{
    public ExectueTurnSessionEntity(string id, Dictionary<string, ISessionValueEntity> values) : base(id, values) { }

    public override void UpdateByTiming(IGameplayStatusWatcher gameWatcher, UpdateTiming timing)
    {
        _UpdateByTiming(gameWatcher, timing);
    }
    public override void UpdateIntent(IGameplayStatusWatcher gameWatcher, IIntentAction intent)
    {
        _UpdateIntent(gameWatcher, intent);
    }
    public override void UpdateResult(IGameplayStatusWatcher gameWatcher, IResultAction result)
    {
        _UpdateResult(gameWatcher, result);
    }
}

public class CardSessionEntity : ReactionSessionEntity
{
    public CardSessionEntity(string id, Dictionary<string, ISessionValueEntity> values) : base(id, values) { }

    public override void UpdateByTiming(IGameplayStatusWatcher gameWatcher, UpdateTiming timing)
    {
        _UpdateByTiming(gameWatcher, timing);
    }
    public override void UpdateIntent(IGameplayStatusWatcher gameWatcher, IIntentAction intent)
    {
        _UpdateIntent(gameWatcher, intent);
    }
    public override void UpdateResult(IGameplayStatusWatcher gameWatcher, IResultAction result)
    {
        _UpdateResult(gameWatcher, result);
    }
}

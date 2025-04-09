using System;
using System.Collections.Generic;


public interface IReactionSessionEntity
{
    string Id { get; }
    void Update(IGameplayStatusWatcher gameWatcher, IReactionSessionData sessionData);
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

    public abstract void Update(IGameplayStatusWatcher gameWatcher, IReactionSessionData sessionData);

    public void _Update(IGameplayStatusWatcher gameWatcher, IReactionSessionData sessionData)
    {
        foreach (var kvp in Values)
        {
            if (sessionData.SessionValueTable.TryGetValue(kvp.Key, out var sessionValueData))
            {
                kvp.Value.Update(gameWatcher, sessionValueData);
            }
        }
    }
}

public class WholeGameSessionEntity : ReactionSessionEntity
{
    public WholeGameSessionEntity(string id, Dictionary<string, ISessionValueEntity> values) : base(id, values) { }

    public override void Update(IGameplayStatusWatcher gameWatcher, IReactionSessionData sessionData)
    {
        // Implement the logic for updating the whole game session entity
        _Update(gameWatcher, sessionData);
    }
}

public class WholeTurnSessionEntity : ReactionSessionEntity
{
    public WholeTurnSessionEntity(string id, Dictionary<string, ISessionValueEntity> values) : base(id, values) { }

    public override void Update(IGameplayStatusWatcher gameWatcher, IReactionSessionData sessionData)
    {
        // Implement the logic for updating the whole turn session entity
        _Update(gameWatcher, sessionData);
    }
}

public class ExectueTurnSessionEntity : ReactionSessionEntity
{
    public ExectueTurnSessionEntity(string id, Dictionary<string, ISessionValueEntity> values) : base(id, values) { }

    public override void Update(IGameplayStatusWatcher gameWatcher, IReactionSessionData sessionData)
    {
        // Implement the logic for updating the execute turn session entity
        _Update(gameWatcher, sessionData);
    }
}

public class CardSessionEntity : ReactionSessionEntity
{
    public CardSessionEntity(string id, Dictionary<string, ISessionValueEntity> values) : base(id, values) { }

    public override void Update(IGameplayStatusWatcher gameWatcher, IReactionSessionData sessionData)
    {
        // Implement the logic for updating the card session entity
        _Update(gameWatcher, sessionData);
    }
}

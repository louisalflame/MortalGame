using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IReactionSessionData
{
    string Id { get; }
    Dictionary<string, ISessionValueData> SessionValueTable { get; }
    IReactionSessionEntity CreateEntity(IGameplayStatusWatcher gameWatcher);
}

public abstract class ReactionSessionData
{
    public string Id { get; set; }
    
    [ShowInInspector]
    protected Dictionary<string, ISessionValueData> _values = new();

    public Dictionary<string, ISessionValueData> SessionValueTable => _values;
}

[Serializable]
public class WholeGameSession : ReactionSessionData, IReactionSessionData
{
    public IReactionSessionEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new WholeGameSessionEntity(
            Id, 
            _values.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.GetEntity(gameWatcher)));
    }
}

[Serializable]
public class WholeTurnSession : ReactionSessionData, IReactionSessionData
{
    public IReactionSessionEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new WholeTurnSessionEntity(
            Id, 
            _values.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.GetEntity(gameWatcher)));
    }
}

[Serializable]
public class ExectueTurnSession : ReactionSessionData, IReactionSessionData
{
    public IReactionSessionEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new WholeTurnSessionEntity(
            Id, 
            _values.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.GetEntity(gameWatcher)));
    }
}

[Serializable]
public class CardSession : ReactionSessionData, IReactionSessionData
{
    public IReactionSessionEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new WholeTurnSessionEntity(
            Id, 
            _values.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.GetEntity(gameWatcher)));
    }
}

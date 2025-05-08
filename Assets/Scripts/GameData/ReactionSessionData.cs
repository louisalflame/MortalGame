using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IReactionSessionData
{
    Dictionary<string, ISessionValueData> SessionValueTable { get; }
    IReactionSessionEntity CreateEntity(IGameplayStatusWatcher gameWatcher);
}

[Serializable]
public abstract class ReactionSessionData
{    
    [ShowInInspector]
    [SerializeField]
    protected Dictionary<string, ISessionValueData> _values = new();
    public Dictionary<string, ISessionValueData> SessionValueTable => _values;
}

[Serializable]
public class WholeGameSession : ReactionSessionData, IReactionSessionData
{
    public IReactionSessionEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new WholeGameSessionEntity(
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
        return new ExectueTurnSessionEntity(
            _values.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.GetEntity(gameWatcher)));
    }
}

[Serializable]
public class PlayCardSession : ReactionSessionData, IReactionSessionData
{
    public IReactionSessionEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new PlayCardSessionEntity(
            _values.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.GetEntity(gameWatcher)));
    }
}

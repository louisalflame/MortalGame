using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IReactionSessionData
{
    Dictionary<string, ISessionValueData> SessionValueTable { get; }

    IReactionSessionEntity GetEntity(IGameplayStatusWatcher gameWatcher);
}

public abstract class ReactionSessionData
{
    public string Id;
    
    public Dictionary<string, ISessionValueData> Values;

    public Dictionary<string, ISessionValueData> SessionValueTable => Values;
}

[Serializable]
public class WholeGameSession : ReactionSessionData, IReactionSessionData
{
    public IReactionSessionEntity GetEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new WholeGameSessionEntity(
            Id, 
            Values.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.GetEntity(gameWatcher)));
    }
}

[Serializable]
public class WholeTurnSession : ReactionSessionData, IReactionSessionData
{
    public IReactionSessionEntity GetEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new WholeTurnSessionEntity(
            Id, 
            Values.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.GetEntity(gameWatcher)));
    }
}

[Serializable]
public class ExectueTurnSession : ReactionSessionData, IReactionSessionData
{
    public IReactionSessionEntity GetEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new WholeTurnSessionEntity(
            Id, 
            Values.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.GetEntity(gameWatcher)));
    }
}

[Serializable]
public class CardSession : ReactionSessionData, IReactionSessionData
{
    public IReactionSessionEntity GetEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new WholeTurnSessionEntity(
            Id, 
            Values.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.GetEntity(gameWatcher)));
    }
}

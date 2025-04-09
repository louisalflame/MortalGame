using System;
using System.Collections.Generic;
using System.Linq;
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
    
    public Dictionary<string, ISessionValueData> Values;

    public Dictionary<string, ISessionValueData> SessionValueTable => Values;
}

[Serializable]
public class WholeGameSession : ReactionSessionData, IReactionSessionData
{
    public IReactionSessionEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
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
    public IReactionSessionEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
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
    public IReactionSessionEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
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
    public IReactionSessionEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new WholeTurnSessionEntity(
            Id, 
            Values.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.GetEntity(gameWatcher)));
    }
}

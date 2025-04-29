using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using Unity.VisualScripting;
using UnityEngine;

public interface IPlayerBuffEntity
{
    string Id { get; }
    Guid Identity { get; }
    int Level { get; }
    Option<IPlayerEntity> Caster { get; }
    IReadOnlyCollection<IPlayerBuffPropertyEntity> Properties { get; }
    IPlayerBuffLifeTimeEntity LifeTime { get; }
    IReadOnlyCollection<IReactionSessionEntity> ReactionSessions { get; }

    void AddLevel(int level);
    PlayerBuffInfo ToInfo();
}

public class PlayerBuffEntity : IPlayerBuffEntity
{
    private string _id;
    private Guid _identity;
    private int _level;
    private Option<IPlayerEntity> _owner;
    private Option<IPlayerEntity> _caster;
    private List<IPlayerBuffPropertyEntity> _properties;
    private IPlayerBuffLifeTimeEntity _lifeTime;
    private List<IReactionSessionEntity> _reactionSessions;

    public string Id => _id;
    public Guid Identity => _identity;
    public int Level => _level;
    public Option<IPlayerEntity> Caster => _caster;
    public IReadOnlyCollection<IPlayerBuffPropertyEntity> Properties => _properties;
    public IPlayerBuffLifeTimeEntity LifeTime => _lifeTime;
    public IReadOnlyCollection<IReactionSessionEntity> ReactionSessions => _reactionSessions;

    public bool IsDummy => this == DummyBuff;
    public static IPlayerBuffEntity DummyBuff = new DummyPlayerBuff();

    public PlayerBuffEntity(
        string id,
        Guid identity,
        int level,
        Option<IPlayerEntity> caster,
        IEnumerable<IPlayerBuffPropertyEntity> properties,
        IPlayerBuffLifeTimeEntity lifeTime,
        IEnumerable<IReactionSessionEntity> reactionSessions) 
    {
        _id = id;
        _identity = identity;
        _level = level;
        _caster = caster;
        _properties = properties.ToList();
        _lifeTime = lifeTime;
        _reactionSessions = reactionSessions.ToList();
    }

    public void AddLevel(int level)
    {
        _level += level;
    }
    
    public PlayerBuffInfo ToInfo()
    {
        return new PlayerBuffInfo() {
            Id = Id,
            Identity = Identity,
            Level = Level
        };
    } 
}

public class DummyPlayerBuff : PlayerBuffEntity
{
    public DummyPlayerBuff() : base(
        string.Empty,
        Guid.Empty,
        1,
        Option.None<IPlayerEntity>(),
        Enumerable.Empty<IPlayerBuffPropertyEntity>(),
        new AlwaysLifeTimePlayerBuffEntity(),
        Enumerable.Empty<IReactionSessionEntity>())
    {
    }
}

public static class PlayerBuffEntityExtensions
{
    public static Option<IPlayerEntity> Owner(this IPlayerBuffEntity playerBuffEntity, IGameplayStatusWatcher watcher)
    {
        if (watcher.GameStatus.Ally.BuffManager.Buffs.Contains(playerBuffEntity))
            return (watcher.GameStatus.Ally as IPlayerEntity).Some();
        if (watcher.GameStatus.Enemy.BuffManager.Buffs.Contains(playerBuffEntity))
            return (watcher.GameStatus.Enemy as IPlayerEntity).Some();
        return Option.None<IPlayerEntity>();
    }
}
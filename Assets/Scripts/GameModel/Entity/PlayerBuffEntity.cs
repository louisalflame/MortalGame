using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public interface IPlayerBuffEntity
{
    string Id { get; }
    Guid Identity { get; }
    int Level { get; }
    IPlayerEntity Owner { get; }
    IPlayerEntity Caster { get; }
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
    private IPlayerEntity _owner;
    private IPlayerEntity _caster;
    private List<IPlayerBuffPropertyEntity> _properties;
    private IPlayerBuffLifeTimeEntity _lifeTime;
    private List<IReactionSessionEntity> _reactionSessions;

    public string Id => _id;
    public Guid Identity => _identity;
    public int Level => _level;
    public IPlayerEntity Owner => _owner;
    public IPlayerEntity Caster => _caster;
    public IReadOnlyCollection<IPlayerBuffPropertyEntity> Properties => _properties;
    public IPlayerBuffLifeTimeEntity LifeTime => _lifeTime;
    public IReadOnlyCollection<IReactionSessionEntity> ReactionSessions => _reactionSessions;

    public bool IsDummy => this == DummyBuff;
    public static IPlayerBuffEntity DummyBuff = new DummyPlayerBuff();

    public PlayerBuffEntity(
        string id,
        Guid identity,
        int level,
        IPlayerEntity owner,
        IPlayerEntity caster,
        IEnumerable<IPlayerBuffPropertyEntity> properties,
        IPlayerBuffLifeTimeEntity lifeTime,
        IEnumerable<IReactionSessionEntity> reactionSessions) 
    {
        _id = id;
        _identity = identity;
        _level = level;
        _owner = owner;
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
        PlayerEntity.DummyPlayer,
        PlayerEntity.DummyPlayer,
        Enumerable.Empty<IPlayerBuffPropertyEntity>(),
        new AlwaysLifeTimePlayerBuffEntity(),
        Enumerable.Empty<IReactionSessionEntity>())
    {
    }
}
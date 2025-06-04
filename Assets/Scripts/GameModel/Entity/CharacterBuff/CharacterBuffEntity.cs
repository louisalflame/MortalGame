using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using Unity.VisualScripting;
using UnityEngine;

public interface ICharacterBuffEntity
{
    string Id { get; }
    Guid Identity { get; }
    int Level { get; }
    Option<IPlayerEntity> Caster { get; }
    IReadOnlyCollection<ICharacterBuffPropertyEntity> Properties { get; }
    ICharacterBuffLifeTimeEntity LifeTime { get; }
    IReadOnlyDictionary<string, IReactionSessionEntity> ReactionSessions { get; }

    bool IsExpired();
    void AddLevel(int level);
    CharacterBuffInfo ToInfo();
}

public class CharacterBuffEntity : ICharacterBuffEntity
{
    private readonly string _id;
    private readonly Guid _identity;
    private int _level;
    private readonly Option<IPlayerEntity> _caster;
    private readonly IReadOnlyList<ICharacterBuffPropertyEntity> _properties;
    private readonly ICharacterBuffLifeTimeEntity _lifeTime;
    private readonly IReadOnlyDictionary<string, IReactionSessionEntity> _reactionSessions;

    public string Id => _id;
    public Guid Identity => _identity;
    public int Level => _level;
    public Option<IPlayerEntity> Caster => _caster;
    public IReadOnlyCollection<ICharacterBuffPropertyEntity> Properties => _properties;
    public ICharacterBuffLifeTimeEntity LifeTime => _lifeTime;
    public IReadOnlyDictionary<string, IReactionSessionEntity> ReactionSessions => _reactionSessions;

    public bool IsDummy => this == DummyBuff;
    public static ICharacterBuffEntity DummyBuff = new DummyCharacterBuff();

    public CharacterBuffEntity(
        string id,
        Guid identity,
        int level,
        Option<IPlayerEntity> caster,
        IEnumerable<ICharacterBuffPropertyEntity> properties,
        ICharacterBuffLifeTimeEntity lifeTime,
        IReadOnlyDictionary<string, IReactionSessionEntity> reactionSessions) 
    {
        _id = id;
        _identity = identity;
        _level = level;
        _caster = caster;
        _properties = properties.ToList();
        _lifeTime = lifeTime;
        _reactionSessions = reactionSessions;
    }

    public bool IsExpired()
    {
        return _lifeTime.IsExpired();
    }

    public void AddLevel(int level)
    {
        _level += level;
    }
    
    public CharacterBuffInfo ToInfo()
    {
        return new CharacterBuffInfo(
            Id,
            Identity,
            Level,
            ReactionSessions
                .Where(kvp => kvp.Value.IntegerValue.HasValue)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.IntegerValue.ValueOr(0)));
    } 
}

public class DummyCharacterBuff : CharacterBuffEntity
{
    public DummyCharacterBuff() : base(
        string.Empty,
        Guid.Empty,
        1,
        Option.None<IPlayerEntity>(),
        Enumerable.Empty<ICharacterBuffPropertyEntity>(),
        new AlwaysLifeTimeCharacterBuffEntity(),
        new Dictionary<string, IReactionSessionEntity>())
    {
    }
}
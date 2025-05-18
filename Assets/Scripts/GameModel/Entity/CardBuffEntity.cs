using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using UnityEngine;

public interface ICardBuffEntity
{
    string CardBuffDataID { get; }
    Guid Identity { get; }
    int Level { get; }
    Option<IPlayerEntity> Caster { get; }
    IReadOnlyCollection<ICardBuffPropertyEntity> Properties { get; }    
    ICardBuffLifeTimeEntity LifeTime { get; }
    IReadOnlyCollection<IReactionSessionEntity> ReactionSessions { get; }
    
    bool IsExpired();
}

public class CardBuffEntity : ICardBuffEntity
{
    private readonly string _cardBuffDataId;
    private readonly Guid _identity;
    private int _level;
    private readonly Option<IPlayerEntity> _caster;
    private readonly IReadOnlyList<ICardBuffPropertyEntity> _properties;
    private readonly ICardBuffLifeTimeEntity _lifeTime;
    private readonly IReadOnlyList<IReactionSessionEntity> _reactionSessions;

    public string CardBuffDataID => _cardBuffDataId;
    public Guid Identity => _identity;
    public int Level => _level;
    public Option<IPlayerEntity> Caster => _caster;
    public IReadOnlyCollection<ICardBuffPropertyEntity> Properties => _properties;
    public ICardBuffLifeTimeEntity LifeTime => _lifeTime;
    public IReadOnlyCollection<IReactionSessionEntity> ReactionSessions => _reactionSessions;

    public CardBuffEntity(
        string cardBuffDataID,
        Guid identity,
        int level,
        Option<IPlayerEntity> caster,
        IEnumerable<ICardBuffPropertyEntity> properties,
        ICardBuffLifeTimeEntity lifeTime,
        IEnumerable<IReactionSessionEntity> reactionSessions)
    {
        _cardBuffDataId = cardBuffDataID;
        _identity = identity;
        _level = level;
        _caster = caster;
        _properties = properties.ToList();
        _lifeTime = lifeTime;
        _reactionSessions = reactionSessions.ToList();
    }

    public bool IsExpired()
    {
        return LifeTime.IsExpired();
    }
}

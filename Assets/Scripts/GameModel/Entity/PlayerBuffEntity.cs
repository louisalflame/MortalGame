using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBuffEntity
{
    public string Id;
    
    public Guid Identity;

    public int Level;

    public IPlayerEntity Owner;
    public IPlayerEntity Caster;

    public IReadOnlyDictionary<BuffTiming, IPlayerBuffEffect[]> Effects;

    public PlayerBuffEntity(
        string id,
        Guid identity,
        int level,
        IPlayerEntity owner,
        IPlayerEntity caster,
        Dictionary<BuffTiming, IPlayerBuffEffect[]> effects) 
    {
        Id = id;
        Identity = identity;
        Level = level;
        Owner = owner;
        Caster = caster;
        Effects = effects;
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

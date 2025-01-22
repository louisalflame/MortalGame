using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BuffEntity
{
    public string Id;
    
    public Guid Identity;

    public int Level;

    public IPlayerEntity Owner;
    public IPlayerEntity Caster;

    public IReadOnlyDictionary<BuffTiming, IBuffEffect[]> Effects;

    public BuffEntity(
        string id,
        Guid identity,
        int level,
        IPlayerEntity owner,
        IPlayerEntity caster,
        Dictionary<BuffTiming, IBuffEffect[]> effects) 
    {
        Id = id;
        Identity = identity;
        Level = level;
        Owner = owner;
        Caster = caster;
        Effects = effects;
    }

    public BuffInfo ToInfo()
    {
        return new BuffInfo() {
            Id = Id,
            Identity = Identity,
            Level = Level
        };
    } 
}

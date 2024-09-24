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

    public PlayerEntity Owner;
    public PlayerEntity Caster;

    public IReadOnlyDictionary<BuffTiming, IBuffEffect[]> Effects;

    public BuffEntity(
        string id,
        Guid identity,
        int level,
        PlayerEntity owner,
        PlayerEntity caster,
        BuffData buffData) 
    {
        Id = id;
        Identity = identity;
        Level = level;
        Owner = owner;
        Caster = caster;

        Effects = buffData.Effects.ToDictionary(
            pair => pair.Key,
            pair => pair.Value.ToArray());
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

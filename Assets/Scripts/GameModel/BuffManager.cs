using System;
using System.Collections.Generic;
using UnityEngine;

public interface IBuffManager
{
    IReadOnlyCollection<BuffEntity> Buffs { get; }
    BuffEntity AddBuff(GameContext gameContext, string buffId, int level);
}

public class BuffManager : IBuffManager
{
    private GameContextManager _gameContextManager;
    private List<BuffEntity> _buffs;

    public IReadOnlyCollection<BuffEntity> Buffs => _buffs;

    public BuffManager()
    {
        _buffs = new List<BuffEntity>();
    }

    public BuffEntity AddBuff(GameContext gameContext, string buffId, int level)
    {
        var buff = new BuffEntity(buffId, Guid.NewGuid().ToString(), level, gameContext.Caster, gameContext.EffectTarget);
        _buffs.Add(buff);
        return buff;
    }
}

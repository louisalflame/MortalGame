using System;
using System.Collections.Generic;
using UnityEngine;

public interface IBuffManager
{
    IReadOnlyCollection<BuffEntity> Buffs { get; }
    bool AddBuff(BuffLibrary buffLibrary, GameContext gameContext, string buffId, int level, out BuffEntity resultBuff);
    bool RemoveBuff(BuffLibrary buffLibrary, GameContext gameContext, string buffId, out BuffEntity resultBuff);
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

    public bool AddBuff(BuffLibrary buffLibrary, GameContext gameContext, string buffId, int level, out BuffEntity resultBuff)
    {
        foreach (var existBuff in _buffs)
        {
            if (existBuff.Id == buffId)
            {
                existBuff.Level += level;
                resultBuff = existBuff;
                return false;
            }
        }

        resultBuff = new BuffEntity(buffId, Guid.NewGuid().ToString(), level, gameContext.Caster, gameContext.EffectTarget);
        _buffs.Add(resultBuff);
        return true;
    }
    public bool RemoveBuff(BuffLibrary buffLibrary, GameContext gameContext, string buffId, out BuffEntity resultBuff)
    {
        foreach (var existBuff in _buffs)
        {
            if (existBuff.Id == buffId)
            {
                _buffs.Remove(existBuff);
                resultBuff = existBuff;
                return true;
            }
        }

        resultBuff = null;
        return false;   
    }
}

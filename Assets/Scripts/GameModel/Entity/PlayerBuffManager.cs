using System;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerBuffManager
{
    IReadOnlyCollection<PlayerBuffEntity> Buffs { get; }
    bool AddBuff(PlayerBuffLibrary buffLibrary, GameContext gameContext, string buffId, int level, out PlayerBuffEntity resultBuff);
    bool RemoveBuff(PlayerBuffLibrary buffLibrary, GameContext gameContext, string buffId, out PlayerBuffEntity resultBuff);
}

public class PlayerBuffManager : IPlayerBuffManager
{
    private GameContextManager _gameContextManager;
    private List<PlayerBuffEntity> _buffs;

    public IReadOnlyCollection<PlayerBuffEntity> Buffs => _buffs;

    public PlayerBuffManager()
    {
        _buffs = new List<PlayerBuffEntity>();
    }

    public bool AddBuff(PlayerBuffLibrary buffLibrary, GameContext gameContext, string buffId, int level, out PlayerBuffEntity resultBuff)
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

        var buffEffects = buffLibrary.GetBuffEffects(buffId);
        resultBuff = new PlayerBuffEntity(
            buffId, 
            Guid.NewGuid(), 
            level, 
            gameContext.EffectTargetPlayer,
            gameContext.CardCaster,
            buffEffects);
        _buffs.Add(resultBuff);
        return true;
    }
    
    public bool RemoveBuff(PlayerBuffLibrary buffLibrary, GameContext gameContext, string buffId, out PlayerBuffEntity resultBuff)
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

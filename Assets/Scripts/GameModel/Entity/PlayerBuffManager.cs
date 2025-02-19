using System;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerBuffManager
{
    IReadOnlyCollection<IPlayerBuffEntity> Buffs { get; }
    bool AddBuff(PlayerBuffLibrary buffLibrary, GameContext gameContext, string buffId, int level, out IPlayerBuffEntity resultBuff);
    bool RemoveBuff(PlayerBuffLibrary buffLibrary, GameContext gameContext, string buffId, out IPlayerBuffEntity resultBuff);
}

public class PlayerBuffManager : IPlayerBuffManager
{
    private GameContextManager _gameContextManager;
    private List<IPlayerBuffEntity> _buffs;

    public IReadOnlyCollection<IPlayerBuffEntity> Buffs => _buffs;

    public PlayerBuffManager()
    {
        _buffs = new List<IPlayerBuffEntity>();
    }

    public bool AddBuff(PlayerBuffLibrary buffLibrary, GameContext gameContext, string buffId, int level, out IPlayerBuffEntity resultBuff)
    {
        foreach (var existBuff in _buffs)
        {
            if (existBuff.Id == buffId)
            {
                existBuff.AddLevel(level);
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
    
    public bool RemoveBuff(PlayerBuffLibrary buffLibrary, GameContext gameContext, string buffId, out IPlayerBuffEntity resultBuff)
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

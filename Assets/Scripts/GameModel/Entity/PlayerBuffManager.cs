using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IPlayerBuffManager
{
    IReadOnlyCollection<IPlayerBuffEntity> Buffs { get; }
    bool AddBuff(PlayerBuffLibrary buffLibrary, IGameplayStatusWatcher gameWatcher, string buffId, int level, out IPlayerBuffEntity resultBuff);
    bool RemoveBuff(PlayerBuffLibrary buffLibrary, IGameplayStatusWatcher gameWatcher, string buffId, out IPlayerBuffEntity resultBuff);
}

public class PlayerBuffManager : IPlayerBuffManager
{
    private List<IPlayerBuffEntity> _buffs;

    public IReadOnlyCollection<IPlayerBuffEntity> Buffs => _buffs;

    public PlayerBuffManager()
    {
        _buffs = new List<IPlayerBuffEntity>();
    }

    public bool AddBuff(PlayerBuffLibrary buffLibrary, IGameplayStatusWatcher gameWatcher, string buffId, int level, out IPlayerBuffEntity resultBuff)
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

        resultBuff = new PlayerBuffEntity(
            buffId, 
            Guid.NewGuid(), 
            level, 
            gameWatcher.GameContext.EffectTargetPlayer,
            gameWatcher.GameContext.CardCaster,
            buffLibrary.GetBuffProperties(buffId)
                .Select(p => p.CreateEntity(gameWatcher)),
            buffLibrary.GetBuffLifeTime(buffId)
                .CreateEntity(gameWatcher),
            buffLibrary.GetBuffSessions(buffId)
                .Select(s => s.CreateEntity(gameWatcher)));
        _buffs.Add(resultBuff);
        return true;
    }
    
    public bool RemoveBuff(PlayerBuffLibrary buffLibrary, IGameplayStatusWatcher gameWatcher, string buffId, out IPlayerBuffEntity resultBuff)
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

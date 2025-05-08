using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using UnityEngine;

public interface IPlayerBuffManager
{
    IReadOnlyCollection<IPlayerBuffEntity> Buffs { get; }
    bool AddBuff(
        PlayerBuffLibrary buffLibrary, 
        IGameplayStatusWatcher gameWatcher, 
        IActionSource actionSource,
        string buffId, 
        int level, 
        out IPlayerBuffEntity resultBuff);
    bool RemoveBuff(
        PlayerBuffLibrary buffLibrary, 
        IGameplayStatusWatcher gameWatcher, 
        IActionSource actionSource,
        string buffId, 
        out IPlayerBuffEntity resultBuff);

    void UpdateTiming(
        IGameplayStatusWatcher gameWatcher,
        UpdateTiming timing);
    void UpdateIntent(
        IGameplayStatusWatcher gameWatcher,
        IIntentAction intent);
    void UpdateResult(
        IGameplayStatusWatcher gameWatcher,
        IResultAction result);
}

public class PlayerBuffManager : IPlayerBuffManager
{
    private List<IPlayerBuffEntity> _buffs;

    public IReadOnlyCollection<IPlayerBuffEntity> Buffs => _buffs;

    public PlayerBuffManager()
    {
        _buffs = new List<IPlayerBuffEntity>();
    }

    public bool AddBuff(
        PlayerBuffLibrary buffLibrary, 
        IGameplayStatusWatcher gameWatcher, 
        IActionSource actionSource,
        string buffId, 
        int level, 
        out IPlayerBuffEntity resultBuff)
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

        var caster = actionSource switch
        {
            CardSource cardSource => cardSource.Card.Owner(gameWatcher),
            PlayerBuffSource playerBuffSource => playerBuffSource.Buff.Caster,
            _ => Option.None<IPlayerEntity>()
        };

        resultBuff = new PlayerBuffEntity(
            buffId, 
            Guid.NewGuid(), 
            level,
            caster,
            buffLibrary.GetBuffProperties(buffId)
                .Select(p => p.CreateEntity(gameWatcher)),
            buffLibrary.GetBuffLifeTime(buffId)
                .CreateEntity(gameWatcher),
            buffLibrary.GetBuffSessions(buffId)
                .Select(s => s.CreateEntity(gameWatcher)));
        _buffs.Add(resultBuff);
        return true;
    }
    
    public bool RemoveBuff(
        PlayerBuffLibrary buffLibrary, 
        IGameplayStatusWatcher gameWatcher, 
        IActionSource actionSource,
        string buffId, 
        out IPlayerBuffEntity resultBuff)
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

    public void UpdateTiming(
        IGameplayStatusWatcher gameWatcher,
        UpdateTiming timing)
    {
        foreach (var buff in _buffs)
        {
            var triggerBuff = new PlayerBuffTrigger(buff);

            foreach(var session in buff.ReactionSessions)
            {
                session.UpdateTiming(gameWatcher, triggerBuff, timing);
            }
            
            buff.LifeTime.UpdateByTiming(gameWatcher, triggerBuff, timing);
        }
    }
    
    public void UpdateIntent(
        IGameplayStatusWatcher gameWatcher,
        IIntentAction intent)
    {
        foreach (var buff in _buffs)
        {
            var triggerBuff = new PlayerBuffTrigger(buff);

            foreach(var session in buff.ReactionSessions)
            {
                session.UpdateIntent(gameWatcher, triggerBuff, intent);
            }

            buff.LifeTime.UpdateIntent(gameWatcher, triggerBuff, intent);
        }
    }

    public void UpdateResult(
        IGameplayStatusWatcher gameWatcher,
        IResultAction result)
    {
        foreach (var buff in _buffs)
        {
            var triggerBuff = new PlayerBuffTrigger(buff);

            foreach(var session in buff.ReactionSessions)
            {
                session.UpdateResult(gameWatcher, triggerBuff, result);
            }

            buff.LifeTime.UpdateResult(gameWatcher, triggerBuff, result);
        }
    }
}

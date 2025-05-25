using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using UnityEngine;

public interface IPlayerBuffManager
{
    IReadOnlyCollection<IPlayerBuffEntity> Buffs { get; }
    AddPlayerBuffResult AddBuff(
        PlayerBuffLibrary buffLibrary, 
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource triggerSource,
        IActionUnit actionUnit,
        string buffId, 
        int level);
    RemovePlayerBuffResult RemoveBuff(
        PlayerBuffLibrary buffLibrary, 
        IGameplayStatusWatcher gameWatcher, 
        IActionUnit actionUnit,
        string buffId);

    void Update(IGameplayStatusWatcher gameWatcher, IActionUnit actionUnit);
}

public class PlayerBuffManager : IPlayerBuffManager
{
    private List<IPlayerBuffEntity> _buffs;

    public IReadOnlyCollection<IPlayerBuffEntity> Buffs => _buffs;

    public PlayerBuffManager()
    {
        _buffs = new List<IPlayerBuffEntity>();
    }

    public AddPlayerBuffResult AddBuff(
        PlayerBuffLibrary buffLibrary, 
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource triggerSource,
        IActionUnit actionUnit,
        string buffId, 
        int level)
    {
        foreach (var existBuff in _buffs)
        {
            if (existBuff.PlayerBuffDataId == buffId)
            {
                existBuff.AddLevel(level);
                return new AddPlayerBuffResult
                {
                    IsNewBuff = false,
                    Buff = existBuff,
                    DeltaLevel = level
                };
            }
        }

        var caster = actionUnit switch
        {
            CardPlaySource cardSource => cardSource.Card.Owner(gameWatcher.GameStatus),
            PlayerBuffSource playerBuffSource => playerBuffSource.Buff.Caster,
            _ => Option.None<IPlayerEntity>()
        };

        var resultBuff = new PlayerBuffEntity(
            buffId, 
            Guid.NewGuid(), 
            level,
            caster,
            buffLibrary.GetBuffProperties(buffId)
                .Select(p => p.CreateEntity(gameWatcher, triggerSource)),
            buffLibrary.GetBuffLifeTime(buffId)
                .CreateEntity(gameWatcher, triggerSource, actionUnit),
            buffLibrary.GetBuffSessions(buffId)
                .Select(s => s.CreateEntity(gameWatcher, triggerSource)));
        _buffs.Add(resultBuff);
        return new AddPlayerBuffResult
        {
            IsNewBuff = true,
            Buff = resultBuff,
            DeltaLevel = level
        };
    }
    
    public RemovePlayerBuffResult RemoveBuff(
        PlayerBuffLibrary buffLibrary, 
        IGameplayStatusWatcher gameWatcher,
        IActionUnit actionUnit,
        string buffId)
    {
        foreach (var existBuff in _buffs)
        {
            if (existBuff.PlayerBuffDataId == buffId)
            {
                _buffs.Remove(existBuff);
                return new RemovePlayerBuffResult
                {
                    Buff = existBuff.SomeNotNull()
                };
            }
        }

        return new RemovePlayerBuffResult
        {
            Buff = Option.None<IPlayerBuffEntity>()
        };   
    }

    public void Update(IGameplayStatusWatcher gameWatcher, IActionUnit actionUnit)
    {
        foreach (var buff in _buffs.ToList())
        {
            var triggerBuff = new PlayerBuffTrigger(buff);
            foreach (var session in buff.ReactionSessions)
            {
                session.Update(gameWatcher, triggerBuff, actionUnit);
            }

            buff.LifeTime.Update(gameWatcher, triggerBuff, actionUnit);
            if (buff.IsExpired())
            {
                _buffs.Remove(buff);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using UnityEngine;

public interface IPlayerBuffManager
{
    IReadOnlyCollection<IPlayerBuffEntity> Buffs { get; }
    AddPlayerBuffResult AddBuff(
        TriggerContext triggerContext,
        string buffId, 
        int level);
    RemovePlayerBuffResult RemoveBuff(
        TriggerContext triggerContext,
        string buffId);

    IEnumerable<IPlayerBuffEntity> Update(TriggerContext triggerContext);
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
        TriggerContext triggerContext,
        string buffId, 
        int level)
    {
        foreach (var existBuff in _buffs)
        {
            if (existBuff.PlayerBuffDataId == buffId)
            {
                existBuff.AddLevel(level);
                return new AddPlayerBuffResult(
                    IsNewBuff: false,
                    Buff: existBuff,
                    DeltaLevel: level
                );
            }
        }

        var caster = triggerContext.Action.Source switch
        {
            PlayerBuffSource playerBuffSource => playerBuffSource.Buff.Caster,
            CardPlaySource cardPlaySource => cardPlaySource.Card.Owner(triggerContext.Model),
            _ => Option.None<IPlayerEntity>()
        };

        var buffLibrary = triggerContext.Model.ContextManager.PlayerBuffLibrary;
        var resultBuff = new PlayerBuffEntity(
            buffId, 
            Guid.NewGuid(), 
            level,
            caster,
            buffLibrary.GetBuffProperties(buffId)
                .Select(p => p.CreateEntity(triggerContext)),
            buffLibrary.GetBuffLifeTime(buffId)
                .CreateEntity(triggerContext),
            buffLibrary.GetBuffSessions(buffId)
                .ToDictionary(
                    kvp => kvp.Key, 
                    kvp => kvp.Value.CreateEntity(triggerContext)));
        _buffs.Add(resultBuff);
        return new AddPlayerBuffResult(
            IsNewBuff: true,
            Buff: resultBuff,
            DeltaLevel: level
        );
    }
    
    public RemovePlayerBuffResult RemoveBuff(
        TriggerContext triggerContext,
        string buffId)
    {
        foreach (var existBuff in _buffs)
        {
            if (existBuff.PlayerBuffDataId == buffId)
            {
                _buffs.Remove(existBuff);
                return new RemovePlayerBuffResult(
                    Buffs: new List<IPlayerBuffEntity> { existBuff }
                );
            }
        }

        return new RemovePlayerBuffResult(
            Buffs: Array.Empty<IPlayerBuffEntity>()
        );   
    }
    // TODO
    public RemovePlayerBuffResult RemoveExpiredBuff(
        TriggerContext triggerContext)
    {
        var expiredBuffs = new List<IPlayerBuffEntity>();
        foreach (var existBuff in _buffs)
        {
            if (existBuff.IsExpired())
            {
                expiredBuffs.Add(existBuff);
                _buffs.Remove(existBuff);
            }
        }

        return new RemovePlayerBuffResult(
            Buffs: expiredBuffs
        );
    }

    public IEnumerable<IPlayerBuffEntity> Update(TriggerContext triggerContext)
    {
        foreach (var buff in _buffs.ToList())
        {
            var isUpdated = false;
            var triggerBuff = new PlayerBuffTrigger(buff);
            var updateBuffTriggerContext = triggerContext with { Triggered = triggerBuff };

            foreach (var session in buff.ReactionSessions.Values)
            {
                isUpdated |= session.Update(updateBuffTriggerContext);
            }

            isUpdated |= buff.LifeTime.Update(updateBuffTriggerContext);

            if (isUpdated)
            {
                yield return buff;
            }
        }
    }
}

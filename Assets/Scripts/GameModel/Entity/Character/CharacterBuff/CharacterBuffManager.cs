using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using UnityEngine;

public interface ICharacterBuffManager
{
    IReadOnlyCollection<ICharacterBuffEntity> Buffs { get; }
    bool AddBuff(
        TriggerContext triggerContext,
        string buffId,
        int level,
        out ICharacterBuffEntity resultBuff);
    bool RemoveBuff(
        TriggerContext triggerContext,
        string buffId,
        out ICharacterBuffEntity resultBuff);
    
    IEnumerable<ICharacterBuffEntity> Update(TriggerContext triggerContext);
}

public class CharacterBuffManager : ICharacterBuffManager
{
    private List<ICharacterBuffEntity> _buffs;

    public IReadOnlyCollection<ICharacterBuffEntity> Buffs => _buffs;

    public CharacterBuffManager()
    {
        _buffs = new List<ICharacterBuffEntity>();
    }

    public bool AddBuff(
        TriggerContext triggerContext,
        string buffId,
        int level,
        out ICharacterBuffEntity resultBuff)
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

        var owner = triggerContext.Action switch
        {
            IActionTargetUnit actionTargetUnit => actionTargetUnit.Target switch
            {
                CharacterTarget characterTarget => Option.Some(characterTarget.Character),
                _ => Option.None<ICharacterEntity>()
            },
            _ => Option.None<ICharacterEntity>()
        };
        var caster = triggerContext.Action.Source switch
        {
            CardPlaySource cardSource => cardSource.Card.Owner(triggerContext.Model),
            PlayerBuffSource playerBuffSource => playerBuffSource.Buff.Caster,
            _ => Option.None<IPlayerEntity>()
        };

        var buffLibrary = triggerContext.Model.ContextManager.CharacterBuffLibrary;
        resultBuff = new CharacterBuffEntity(
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
                    session => session.Key,
                    session => session.Value.CreateEntity(triggerContext)));
        _buffs.Add(resultBuff);
        return true;
    }

    public bool RemoveBuff(
        TriggerContext triggerContext,
        string buffId,
        out ICharacterBuffEntity resultBuff)
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

    public IEnumerable<ICharacterBuffEntity> Update(TriggerContext triggerContext)
    {
        foreach (var buff in _buffs.ToList())
        {
            var isUpdated = false;
            var triggeredBuff = new CharacterBuffTrigger(buff);
            var updateCharacterBuffContext = triggerContext with { Triggered = triggeredBuff };
            foreach (var session in buff.ReactionSessions.Values)
            {
                isUpdated |= session.Update(updateCharacterBuffContext);
            }

            isUpdated |= buff.LifeTime.Update(updateCharacterBuffContext);

            if (isUpdated)
            { 
                yield return buff;
            }
        }
    }
}

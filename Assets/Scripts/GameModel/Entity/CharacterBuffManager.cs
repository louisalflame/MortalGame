using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using UnityEngine;

public interface ICharacterBuffManager
{
    IReadOnlyCollection<ICharacterBuffEntity> Buffs { get; }
    bool AddBuff(
        CharacterBuffLibrary buffLibrary,
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource triggerSource,
        IActionSource actionSource,
        IActionTarget actionTarget,
        string buffId,
        int level,
        out ICharacterBuffEntity resultBuff);
    bool RemoveBuff(
        CharacterBuffLibrary buffLibrary,
        IGameplayStatusWatcher gameWatcher,
        IActionSource actionSource,
        IActionTarget actionTarget,
        string buffId,
        out ICharacterBuffEntity resultBuff);
    
    void UpdateTiming(IGameplayStatusWatcher gameWatcher, UpdateTiming timing);
    void UpdateIntent(IGameplayStatusWatcher gameWatcher, IIntentAction intent);
    void UpdateResult(IGameplayStatusWatcher gameWatcher, IResultAction result);
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
        CharacterBuffLibrary buffLibrary,
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource triggerSource,
        IActionSource actionSource,
        IActionTarget actionTarget,
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

        var owner = actionTarget switch
        {
            CharacterTarget characterTarget => Option.Some(characterTarget.Character),
            _ => Option.None<ICharacterEntity>()
        };
        var caster = actionSource switch
        {
            CardPlaySource cardSource => cardSource.Card.Owner(gameWatcher),
            PlayerBuffSource playerBuffSource => playerBuffSource.Buff.Caster,
            _ => Option.None<IPlayerEntity>()
        };

        resultBuff = new CharacterBuffEntity(
            buffId,
            Guid.NewGuid(),
            level,
            caster,
            buffLibrary.GetBuffProperties(buffId)
                .Select(p => p.CreateEntity(gameWatcher, triggerSource)),
            buffLibrary.GetBuffLifeTime(buffId)
                .CreateEntity(gameWatcher, triggerSource),
            buffLibrary.GetBuffSessions(buffId)
                .Select(s => s.CreateEntity(gameWatcher, triggerSource)));
        _buffs.Add(resultBuff);
        return true;
    }

    public bool RemoveBuff(
        CharacterBuffLibrary buffLibrary,
        IGameplayStatusWatcher gameWatcher,
        IActionSource actionSource,
        IActionTarget actionTarget,
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

    public void UpdateTiming(IGameplayStatusWatcher gameWatcher, UpdateTiming timing)
    {
        foreach (var buff in _buffs.ToList())
        {
            var triggeredBuff = new CharacterBuffTrigger(buff);
            foreach (var session in buff.ReactionSessions)
            {
                session.UpdateTiming(gameWatcher, triggeredBuff, timing);
            }

            buff.LifeTime.UpdateTiming(gameWatcher, triggeredBuff, timing);
            if (buff.IsExpired())
            {
                _buffs.Remove(buff);
            }
        }
    }

    public void UpdateIntent(IGameplayStatusWatcher gameWatcher, IIntentAction intent)
    {
        foreach (var buff in _buffs.ToList())
        {
            var triggeredBuff = new CharacterBuffTrigger(buff);
            foreach (var session in buff.ReactionSessions)
            {
                session.UpdateIntent(gameWatcher, triggeredBuff, intent);
            }

            buff.LifeTime.UpdateIntent(gameWatcher, triggeredBuff, intent);
            if (buff.IsExpired())
            {
                _buffs.Remove(buff);
            }
        }
    }
    
    public void UpdateResult(IGameplayStatusWatcher gameWatcher, IResultAction result)
    {
        foreach (var buff in _buffs.ToList())
        {
            var triggeredBuff = new CharacterBuffTrigger(buff);
            foreach (var session in buff.ReactionSessions)
            {
                session.UpdateResult(gameWatcher, triggeredBuff, result);
            }

            buff.LifeTime.UpdateResult(gameWatcher, triggeredBuff, result);
            if (buff.IsExpired())
            {
                _buffs.Remove(buff);
            }
        }
    }
}

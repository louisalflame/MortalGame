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
    
    void Update(IGameplayStatusWatcher gameWatcher, IActionUnit actionUnit);
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

    public void Update(IGameplayStatusWatcher gameWatcher, IActionUnit actionUnit)
    {
        foreach (var buff in _buffs.ToList())
        {
            var triggeredBuff = new CharacterBuffTrigger(buff);
            foreach (var session in buff.ReactionSessions)
            {
                session.Update(gameWatcher, triggeredBuff, actionUnit);
            }

            buff.LifeTime.Update(gameWatcher, triggeredBuff, actionUnit);
            if (buff.IsExpired())
            {
                _buffs.Remove(buff);
            }
        }
    }
}

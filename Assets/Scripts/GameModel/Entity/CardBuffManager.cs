using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using UnityEngine;

public interface ICardBuffManager
{
    IReadOnlyCollection<ICardBuffEntity> Buffs { get; }
    bool AddBuff(
        CardBuffLibrary buffLibrary,
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource triggerSource,
        IActionSource actionSource,
        string buffId,
        int level,
        out ICardBuffEntity resultBuff);
    bool RemoveBuff(
        CardBuffLibrary buffLibrary,
        IGameplayStatusWatcher gameWatcher,
        IActionSource actionSource,
        string buffId,
        out ICardBuffEntity resultBuff);

    void Update(IGameplayStatusWatcher gameWatcher, IActionUnit actionUnit);
}

public class CardBuffManager : ICardBuffManager
{
    private readonly List<ICardBuffEntity> _buffs;

    public IReadOnlyCollection<ICardBuffEntity> Buffs => _buffs;

    public CardBuffManager()
    {
        _buffs = new List<ICardBuffEntity>();
    }

    public bool AddBuff(
        CardBuffLibrary cardBuffLibrary,
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource triggerSource,
        IActionSource actionSource,
        string buffId,
        int level,
        out ICardBuffEntity resultBuff)
    {
        foreach (var existBuff in _buffs)
        {
            if (existBuff.CardBuffDataID == buffId)
            {
                existBuff.AddLevel(level);
                resultBuff = existBuff;
                return false;
            }
        }

        var caster = actionSource switch
        {
            CardPlaySource cardSource => cardSource.Card.Owner(gameWatcher),
            PlayerBuffSource playerBuffSource => playerBuffSource.Buff.Caster,
            _ => Option.None<IPlayerEntity>()
        };

        resultBuff = new CardBuffEntity(
            buffId,
            new Guid(),
            level,
            caster,
            cardBuffLibrary.GetCardBuffProperties(buffId)
                .Select(p => p.CreateEntity(gameWatcher, triggerSource)),
            cardBuffLibrary.GetCardBuffLifeTime(buffId)
                .CreateEntity(gameWatcher, triggerSource),
            cardBuffLibrary.GetCardBuffSessions(buffId)
                .Select(s => s.CreateEntity(gameWatcher, triggerSource)));

        _buffs.Add(resultBuff);
        return true;
    }

    public bool RemoveBuff(
        CardBuffLibrary buffLibrary,
        IGameplayStatusWatcher gameWatcher,
        IActionSource actionSource,
        string buffId,
        out ICardBuffEntity resultBuff)
    {
        foreach (var existBuff in _buffs)
        {
            if (existBuff.CardBuffDataID == buffId)
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
            var triggerBuff = new CardBuffTrigger(buff);
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
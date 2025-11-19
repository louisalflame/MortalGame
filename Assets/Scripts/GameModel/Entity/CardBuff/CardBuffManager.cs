using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using UnityEngine;

public interface ICardBuffManager
{
    IReadOnlyCollection<ICardBuffEntity> Buffs { get; }
    AddCardBuffResult AddBuff(
        CardBuffLibrary buffLibrary,
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource triggerSource,
        IActionUnit actionUnit,
        string buffId,
        int level);
    RemoveCardBuffResult RemoveBuff(
        CardBuffLibrary buffLibrary,
        IGameplayStatusWatcher gameWatcher,
        IActionUnit actionUnit,
        string buffId);

    bool Update(IGameplayStatusWatcher gameWatcher, IActionUnit actionUnit);
}

public class CardBuffManager : ICardBuffManager
{
    private readonly List<ICardBuffEntity> _buffs;

    public IReadOnlyCollection<ICardBuffEntity> Buffs => _buffs;

    public CardBuffManager(IEnumerable<ICardBuffEntity> buffs)
    {
        _buffs = new List<ICardBuffEntity>(buffs);
    }

    public AddCardBuffResult AddBuff(
        CardBuffLibrary cardBuffLibrary,
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource triggerSource,
        IActionUnit actionUnit,
        string buffId,
        int level)
    {
        foreach (var existBuff in _buffs)
        {
            if (existBuff.CardBuffDataID == buffId)
            {
                existBuff.AddLevel(level);
                return new AddCardBuffResult(
                    IsNewBuff: false,
                    Buff: existBuff,
                    DeltaLevel: level);
            }
        }

        var caster = actionUnit switch
        {
            CardPlaySource cardSource => cardSource.Card.Owner(gameWatcher),
            PlayerBuffSource playerBuffSource => playerBuffSource.Buff.Caster,
            _ => Option.None<IPlayerEntity>()
        };

        var resultBuff = CardBuffEntity.CreateFromData(
            buffId,
            level,
            caster,
            gameWatcher,
            triggerSource,
            actionUnit,
            cardBuffLibrary);

        _buffs.Add(resultBuff);
        return new AddCardBuffResult(
            IsNewBuff: true,
            Buff: resultBuff,
            DeltaLevel: level);
    }

    public RemoveCardBuffResult RemoveBuff(
        CardBuffLibrary buffLibrary,
        IGameplayStatusWatcher gameWatcher,
        IActionUnit actionUnit,
        string buffId)
    {
        foreach (var existBuff in _buffs)
        {
            if (existBuff.CardBuffDataID == buffId)
            {
                _buffs.Remove(existBuff);
                return new RemoveCardBuffResult(
                    Buff: new List<ICardBuffEntity>() { existBuff });
            }
        }

        return new RemoveCardBuffResult(
            Buff: Array.Empty<ICardBuffEntity>());
    }

    public bool Update(IGameplayStatusWatcher gameWatcher, IActionUnit actionUnit)
    {
        var isUpdated = false;
        foreach (var buff in _buffs.ToList())
        {
            var triggerBuff = new CardBuffTrigger(buff);
            foreach (var session in buff.ReactionSessions.Values)
            {
                isUpdated |= session.Update(gameWatcher, triggerBuff, actionUnit);
            }

            isUpdated |= buff.LifeTime.Update(gameWatcher, triggerBuff, actionUnit);
        }
        return isUpdated;
    }    
}
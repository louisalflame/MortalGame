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
                return new AddCardBuffResult
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

        var resultBuff = new CardBuffEntity(
            buffId,
            new Guid(),
            level,
            caster,
            cardBuffLibrary.GetCardBuffProperties(buffId)
                .Select(p => p.CreateEntity(gameWatcher, triggerSource)),
            cardBuffLibrary.GetCardBuffLifeTime(buffId)
                .CreateEntity(gameWatcher, triggerSource, actionUnit),
            cardBuffLibrary.GetCardBuffSessions(buffId)
                .ToDictionary(
                    session => session.Key,
                    session => session.Value.CreateEntity(gameWatcher, triggerSource, actionUnit)));

        _buffs.Add(resultBuff);
        return new AddCardBuffResult
        {
            IsNewBuff = true,
            Buff = resultBuff,
            DeltaLevel = level
        };
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
                return new RemoveCardBuffResult
                {
                    Buff = existBuff.Some()
                };
            }
        }

        return new RemoveCardBuffResult
        {
            Buff = Option.None<ICardBuffEntity>()
        };
    }

    public void Update(IGameplayStatusWatcher gameWatcher, IActionUnit actionUnit)
    {
        foreach (var buff in _buffs.ToList())
        {
            var triggerBuff = new CardBuffTrigger(buff);
            foreach (var session in buff.ReactionSessions.Values)
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
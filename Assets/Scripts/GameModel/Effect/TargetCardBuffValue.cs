using System;
using Optional;
using UnityEngine;

public interface ITargetCardBuffValue
{
    Option<ICardBuffEntity> Eval(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource trigger,
        IActionUnit actionUnit);
}

[Serializable]
public class NoneCardBuff : ITargetCardBuffValue
{
    public Option<ICardBuffEntity> Eval(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource trigger,
        IActionUnit actionUnit)
    {
        return Option.None<ICardBuffEntity>();
    }
}

[Serializable]
public class TriggeredCardBuff : ITargetCardBuffValue
{
    public Option<ICardBuffEntity> Eval(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource trigger,
        IActionUnit actionUnit)
    {
        return trigger switch
        {
            CardBuffTrigger cardBuff => cardBuff.Buff.SomeNotNull(),
            _ => Option.None<ICardBuffEntity>()
        };
    }
}
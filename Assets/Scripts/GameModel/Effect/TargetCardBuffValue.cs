using System;
using Optional;
using UnityEngine;

public interface ITargetCardBuffValue
{
    Option<ICardBuffEntity> Eval(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource trigger);
}

[Serializable]
public class NoneCardBuff : ITargetCardBuffValue
{
    public Option<ICardBuffEntity> Eval(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource trigger)
    {
        return Option.None<ICardBuffEntity>();
    }
}

[Serializable]
public class TriggeredCardBuff : ITargetCardBuffValue
{
    public Option<ICardBuffEntity> Eval(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource trigger)
    {
        return trigger switch
        {
            CardBuffTrigger cardBuff => cardBuff.Buff.SomeNotNull(),
            _ => Option.None<ICardBuffEntity>()
        };
    }
}
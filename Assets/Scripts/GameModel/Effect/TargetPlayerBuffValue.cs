using System;
using System.Collections.Generic;
using Optional;
using UnityEngine;

public interface ITargetPlayerBuffValue
{
    Option<IPlayerBuffEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger);
}

[Serializable]
public class NoneBuff : ITargetPlayerBuffValue
{
    public Option<IPlayerBuffEntity> Eval(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource trigger)
    {
        return Option.None<IPlayerBuffEntity>();
    }
}
[Serializable]
public class TriggeredPlayerBuff : ITargetPlayerBuffValue
{
    public Option<IPlayerBuffEntity> Eval(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource trigger)
    {
        return trigger switch
        {
            PlayerBuffTrigger playerBuffTrigger => playerBuffTrigger.Buff.Some(),
            _ => Option.None<IPlayerBuffEntity>()
        };
    }
}

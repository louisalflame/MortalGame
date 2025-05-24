using System;
using System.Collections.Generic;
using Optional;
using UnityEngine;

public interface ITargetPlayerBuffValue
{
    Option<IPlayerBuffEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger,
        IActionUnit actionUnit);
}

[Serializable]
public class NoneBuff : ITargetPlayerBuffValue
{
    public Option<IPlayerBuffEntity> Eval(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource trigger,
        IActionUnit actionUnit)
    {
        return Option.None<IPlayerBuffEntity>();
    }
}
[Serializable]
public class TriggeredPlayerBuff : ITargetPlayerBuffValue
{
    public Option<IPlayerBuffEntity> Eval(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource trigger,
        IActionUnit actionUnit)
    {
        return trigger switch
        {
            PlayerBuffTrigger playerBuffTrigger => playerBuffTrigger.Buff.Some(),
            _ => Option.None<IPlayerBuffEntity>()
        };
    }
}

using System;
using System.Collections.Generic;
using Optional;
using UnityEngine;

public static class TargetPlayerBuffValueExtensions
{
    public static Option<IPlayerBuffEntity> Eval(
        this ITargetPlayerBuffValue targetPlayerBuffValue, 
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger = null,
        IActionSource source = null)
    {
        return targetPlayerBuffValue.Eval(gameWatcher, trigger.SomeNotNull(), source.SomeNotNull());
    }
}

public interface ITargetPlayerBuffValue
{
    Option<IPlayerBuffEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        Option<ITriggerSource> trigger,
        Option<IActionSource> source);
}

[Serializable]
public class NoneBuff : ITargetPlayerBuffValue
{
    public Option<IPlayerBuffEntity> Eval(
        IGameplayStatusWatcher gameWatcher,
        Option<ITriggerSource> trigger,
        Option<IActionSource> source)
    {
        return Option.None<IPlayerBuffEntity>();
    }
}
[Serializable]
public class TriggeredPlayerBuff : ITargetPlayerBuffValue
{
    public Option<IPlayerBuffEntity> Eval(
        IGameplayStatusWatcher gameWatcher,
        Option<ITriggerSource> trigger,
        Option<IActionSource> source)
    {
        return trigger.Match(
            t => {
                return t switch
                {
                    PlayerBuffTrigger playerBuffTrigger => playerBuffTrigger.Buff.Some(),
                    _ => Option.None<IPlayerBuffEntity>()
                };
            },
            () => Option.None<IPlayerBuffEntity>());
    }
}

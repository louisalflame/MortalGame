using System;
using System.Collections.Generic;
using Optional;
using UnityEngine;

public interface ITargetPlayerBuffValue
{
    Option<IPlayerBuffEntity> Eval(IGameplayStatusWatcher gameWatcher, IActionSource source);
}

[Serializable]
public class NoneBuff : ITargetPlayerBuffValue
{
    public Option<IPlayerBuffEntity> Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return Option.None<IPlayerBuffEntity>();
    }
}
[Serializable]
public class TriggeredBuff : ITargetPlayerBuffValue
{
    public Option<IPlayerBuffEntity> Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return gameWatcher.GameContext.TriggeredBuff.Some();
    }
}

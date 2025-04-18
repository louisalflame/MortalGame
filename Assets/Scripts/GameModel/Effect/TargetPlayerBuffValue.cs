using System;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetPlayerBuffValue
{
    IPlayerBuffEntity Eval(IGameplayStatusWatcher gameWatcher, IActionSource source);
}

[Serializable]
public class NoneBuff : ITargetPlayerBuffValue
{
    public IPlayerBuffEntity Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return PlayerBuffEntity.DummyBuff;
    }
}
[Serializable]
public class TriggeredBuff : ITargetPlayerBuffValue
{
    public IPlayerBuffEntity Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return gameWatcher.GameContext.TriggeredBuff;
    }
}

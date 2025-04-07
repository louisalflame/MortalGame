using System;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetPlayerBuffValue
{
    IPlayerBuffEntity Eval(IGameplayStatusWatcher gameWatcher);
}

[Serializable]
public class NoneBuff : ITargetPlayerBuffValue
{
    public IPlayerBuffEntity Eval(IGameplayStatusWatcher gameWatcher)
    {
        return PlayerBuffEntity.DummyBuff;
    }
}
[Serializable]
public class TriggeredBuff : ITargetPlayerBuffValue
{
    public IPlayerBuffEntity Eval(IGameplayStatusWatcher gameWatcher)
    {
        return gameWatcher.GameContext.TriggeredBuff;
    }
}

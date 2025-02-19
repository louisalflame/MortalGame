using System;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetPlayerBuffValue
{
    IPlayerBuffEntity Eval(GameStatus gameStatus, GameContext context);
}

[Serializable]
public class NoneBuff : ITargetPlayerBuffValue
{
    public IPlayerBuffEntity Eval(GameStatus gameStatus, GameContext context)
    {
        return PlayerBuffEntity.DummyBuff;
    }
}
[Serializable]
public class TriggeredBuff : ITargetPlayerBuffValue
{
    public IPlayerBuffEntity Eval(GameStatus gameStatus, GameContext context)
    {
        return context.TriggeredBuff;
    }
}

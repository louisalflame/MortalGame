using System;
using UnityEngine;

public enum TargetType
{
    Player = 1,
    Card
}

public interface ITargetPlayerValue
{
    PlayerEntity Eval(GameStatus gameStatus, GameContext context);
}

[Serializable]
public class NonePlayer : ITargetPlayerValue
{
    public PlayerEntity Eval(GameStatus gameStatus, GameContext context)
    {
        return PlayerEntity.DummyPlayer;
    }
}

[Serializable]
public class ThisPlayer : ITargetPlayerValue
{
    public PlayerEntity Eval(GameStatus gameStatus, GameContext context)
    {
        return context.Caster;
    }
}

[Serializable]
public class OppositePlayer : ITargetPlayerValue
{
    public PlayerEntity Eval(GameStatus gameStatus, GameContext context)
    {
        return context.Caster == gameStatus.Ally ? gameStatus.Enemy : gameStatus.Ally;
    }
}
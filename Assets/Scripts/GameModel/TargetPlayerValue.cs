using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public enum TargetType
{
    Player = 1,
    Card
}

public interface ITargetPlayerValue
{
    IReadOnlyCollection<PlayerEntity> Eval(GameStatus gameStatus, GameContext context);
}

[Serializable]
public class NonePlayer : ITargetPlayerValue
{
    public IReadOnlyCollection<PlayerEntity> Eval(GameStatus gameStatus, GameContext context)
    {
        return new PlayerEntity[0];
    }
}

[Serializable]
public class ThisPlayer : ITargetPlayerValue
{
    public IReadOnlyCollection<PlayerEntity> Eval(GameStatus gameStatus, GameContext context)
    {
        return new PlayerEntity[] { context.Caster };
    }
}

[Serializable]
public class OppositePlayer : ITargetPlayerValue
{
    public IReadOnlyCollection<PlayerEntity> Eval(GameStatus gameStatus, GameContext context)
    {
        return new PlayerEntity[] { context.Caster == gameStatus.Ally ? gameStatus.Enemy : gameStatus.Ally };
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

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
public class ThisExecutePlayer : ITargetPlayerValue
{
    public IReadOnlyCollection<PlayerEntity> Eval(GameStatus gameStatus, GameContext context)
    {
        return new PlayerEntity[] { context.ExecutePlayer };
    }
}
[Serializable]
public class OppositePlayers : ITargetPlayerValue
{
    public ITargetPlayerValue Reference;

    public IReadOnlyCollection<PlayerEntity> Eval(GameStatus gameStatus, GameContext context)
    {
        var references = Reference.Eval(gameStatus, context);
        return references.Select<PlayerEntity, PlayerEntity>(reference => reference == gameStatus.Ally ? gameStatus.Enemy : gameStatus.Ally).ToArray();
    }
}

[Serializable]
public class CardCaster : ITargetPlayerValue
{
    public IReadOnlyCollection<PlayerEntity> Eval(GameStatus gameStatus, GameContext context)
    {
        return new PlayerEntity[] { context.CardCaster };
    }
}

[Serializable]
public class ThisBuffOwner : ITargetPlayerValue
{
    public IReadOnlyCollection<PlayerEntity> Eval(GameStatus gameStatus, GameContext context)
    {
        return new PlayerEntity[] { context.UsingBuff.Owner };
    }
}
[Serializable]
public class ThisBuffCaster : ITargetPlayerValue
{
    public IReadOnlyCollection<PlayerEntity> Eval(GameStatus gameStatus, GameContext context)
    {
        return new PlayerEntity[] { context.UsingBuff.Caster };
    }
}
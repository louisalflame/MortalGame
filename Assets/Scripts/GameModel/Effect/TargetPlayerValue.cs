using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public interface ITargetPlayerValue
{
    IPlayerEntity Eval(GameStatus gameStatus, GameContext context);
}
[Serializable]
public class NonePlayer : ITargetPlayerValue
{
    public IPlayerEntity Eval(GameStatus gameStatus, GameContext context)
    {
        return PlayerEntity.DummyPlayer;
    }
}
[Serializable]
public class ThisExecutePlayer : ITargetPlayerValue
{
    public IPlayerEntity Eval(GameStatus gameStatus, GameContext context)
    {
        return context.ExecutePlayer;
    }
}
[Serializable]
public class CardCaster : ITargetPlayerValue
{
    public IPlayerEntity Eval(GameStatus gameStatus, GameContext context)
    {
        return context.CardCaster;
    }
}
[Serializable]
public class ThisBuffOwner : ITargetPlayerValue
{
    public IPlayerEntity Eval(GameStatus gameStatus, GameContext context)
    {
        return context.UsingBuff.Owner;
    }
}
[Serializable]
public class ThisBuffCaster : ITargetPlayerValue
{
    public IPlayerEntity Eval(GameStatus gameStatus, GameContext context)
    {
        return context.UsingBuff.Caster;
    }
}

public interface ITargetPlayerCollectionValue
{
    IReadOnlyCollection<IPlayerEntity> Eval(GameStatus gameStatus, GameContext context);
}
[Serializable]
public class NonePlayers : ITargetPlayerCollectionValue
{
    public IReadOnlyCollection<IPlayerEntity> Eval(GameStatus gameStatus, GameContext context)
    {
        return new PlayerEntity[0];
    }
}
[Serializable]
public class SinglePlayerCollection : ITargetPlayerCollectionValue
{
    public ITargetPlayerValue Target;

    public IReadOnlyCollection<IPlayerEntity> Eval(GameStatus gameStatus, GameContext context)
    {
        return new [] { Target.Eval(gameStatus, context) };
    }
}
[Serializable]
public class OppositePlayers : ITargetPlayerCollectionValue
{
    public ITargetPlayerCollectionValue Reference;

    public IReadOnlyCollection<IPlayerEntity> Eval(GameStatus gameStatus, GameContext context)
    {
        var references = Reference.Eval(gameStatus, context);
        return references
            .Select<IPlayerEntity, IPlayerEntity>(
                reference => reference == gameStatus.Ally ? 
                    gameStatus.Enemy : 
                    gameStatus.Ally)
            .ToArray();
    }
}
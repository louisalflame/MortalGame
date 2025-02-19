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
public class OppositePlayer : ITargetPlayerValue
{    
    public ITargetPlayerValue Reference;

    public IPlayerEntity Eval(GameStatus gameStatus, GameContext context)
    {
        var reference = Reference.Eval(gameStatus, context);
        return
            reference.Faction == Faction.Ally ? gameStatus.Enemy : 
            reference.Faction == Faction.Enemy ? gameStatus.Ally : 
            PlayerEntity.DummyPlayer;
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
public class OwnerOfPlayerBuff : ITargetPlayerValue
{
    public ITargetPlayerBuffValue PlayerBuff;

    public IPlayerEntity Eval(GameStatus gameStatus, GameContext context)
    {
        var playerBuff = PlayerBuff.Eval(gameStatus, context);
        return playerBuff.Owner;
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
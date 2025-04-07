using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public interface ITargetPlayerValue
{
    IPlayerEntity Eval(IGameplayStatusWatcher gameWatcher);
}
[Serializable]
public class NonePlayer : ITargetPlayerValue
{
    public IPlayerEntity Eval(IGameplayStatusWatcher gameWatcher)
    {
        return PlayerEntity.DummyPlayer;
    }
}
[Serializable]
public class ThisExecutePlayer : ITargetPlayerValue
{
    public IPlayerEntity Eval(IGameplayStatusWatcher gameWatcher)
    {
        return gameWatcher.GameContext.ExecutePlayer;
    }
}
[Serializable]
public class OppositePlayer : ITargetPlayerValue
{    
    public ITargetPlayerValue Reference;

    public IPlayerEntity Eval(IGameplayStatusWatcher gameWatcher)
    {
        var reference = Reference.Eval(gameWatcher);
        return
            reference.Faction == Faction.Ally ? gameWatcher.GameStatus.Enemy : 
            reference.Faction == Faction.Enemy ? gameWatcher.GameStatus.Ally : 
            PlayerEntity.DummyPlayer;
    }
}
[Serializable]
public class CardCaster : ITargetPlayerValue
{
    public IPlayerEntity Eval(IGameplayStatusWatcher gameWatcher)
    {
        return gameWatcher.GameContext.CardCaster;
    }
}
[Serializable]
public class OwnerOfPlayerBuff : ITargetPlayerValue
{
    public ITargetPlayerBuffValue PlayerBuff;

    public IPlayerEntity Eval(IGameplayStatusWatcher gameWatcher)
    {
        var playerBuff = PlayerBuff.Eval(gameWatcher);
        return playerBuff.Owner;
    }
}


public interface ITargetPlayerCollectionValue
{
    IReadOnlyCollection<IPlayerEntity> Eval(IGameplayStatusWatcher gameWatcher);
}
[Serializable]
public class NonePlayers : ITargetPlayerCollectionValue
{
    public IReadOnlyCollection<IPlayerEntity> Eval(IGameplayStatusWatcher gameWatcher)
    {
        return new PlayerEntity[0];
    }
}
[Serializable]
public class SinglePlayerCollection : ITargetPlayerCollectionValue
{
    public ITargetPlayerValue Target;

    public IReadOnlyCollection<IPlayerEntity> Eval(IGameplayStatusWatcher gameWatcher)
    {
        return new [] { Target.Eval(gameWatcher) };
    }
}
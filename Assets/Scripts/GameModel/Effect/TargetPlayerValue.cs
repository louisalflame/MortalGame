using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public interface ITargetPlayerValue
{
    IPlayerEntity Eval(IGameplayStatusWatcher gameWatcher, IActionSource source);
}
[Serializable]
public class NonePlayer : ITargetPlayerValue
{
    public IPlayerEntity Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return PlayerEntity.DummyPlayer;
    }
}
[Serializable]
public class ThisExecutePlayer : ITargetPlayerValue
{
    public IPlayerEntity Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return source switch
        {
            CardSource cardSource => cardSource.Card.Owner,
            PlayerBuffSource playerBuffSource => playerBuffSource.Buff.Owner,
            CardBuffSource cardBuffSource => PlayerEntity.DummyPlayer, //TODO
            SystemSource _ => PlayerEntity.DummyPlayer,
            _ => PlayerEntity.DummyPlayer
        };
    }
}
[Serializable]
public class OppositePlayer : ITargetPlayerValue
{    
    public ITargetPlayerValue Reference;

    public IPlayerEntity Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        var reference = Reference.Eval(gameWatcher, source);
        return
            reference.Faction == Faction.Ally ? gameWatcher.GameStatus.Enemy : 
            reference.Faction == Faction.Enemy ? gameWatcher.GameStatus.Ally : 
            PlayerEntity.DummyPlayer;
    }
}
[Serializable]
public class CardCaster : ITargetPlayerValue
{
    public IPlayerEntity Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return source switch
        {
            CardSource cardSource => cardSource.Card.Owner,
            PlayerBuffSource playerBuffSource => PlayerEntity.DummyPlayer,
            CardBuffSource cardBuffSource => PlayerEntity.DummyPlayer,
            SystemSource _ => PlayerEntity.DummyPlayer,
            _ => PlayerEntity.DummyPlayer
        };
    }
}
[Serializable]
public class OwnerOfPlayerBuff : ITargetPlayerValue
{
    public ITargetPlayerBuffValue PlayerBuff;

    public IPlayerEntity Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        var playerBuff = PlayerBuff.Eval(gameWatcher, source);
        return playerBuff.Owner;
    }
}


public interface ITargetPlayerCollectionValue
{
    IReadOnlyCollection<IPlayerEntity> Eval(IGameplayStatusWatcher gameWatcher, IActionSource source);
}
[Serializable]
public class NonePlayers : ITargetPlayerCollectionValue
{
    public IReadOnlyCollection<IPlayerEntity> Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return new PlayerEntity[0];
    }
}
[Serializable]
public class SinglePlayerCollection : ITargetPlayerCollectionValue
{
    public ITargetPlayerValue Target;

    public IReadOnlyCollection<IPlayerEntity> Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return new [] { Target.Eval(gameWatcher, source) };
    }
}
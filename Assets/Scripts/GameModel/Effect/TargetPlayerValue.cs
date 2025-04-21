using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using UniRx;
using UnityEngine;

public interface ITargetPlayerValue
{
    Option<IPlayerEntity> Eval(IGameplayStatusWatcher gameWatcher, IActionSource source);
}
[Serializable]
public class NonePlayer : ITargetPlayerValue
{
    public Option<IPlayerEntity> Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return Option.None<IPlayerEntity>();
    }
}
[Serializable]
public class ThisExecutePlayer : ITargetPlayerValue
{
    public Option<IPlayerEntity> Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return source switch
        {
            CardSource cardSource => cardSource.Card.Owner(gameWatcher),
            PlayerBuffSource playerBuffSource => playerBuffSource.Buff.Owner,
            _ => Option.None<IPlayerEntity>()
        };
    }
}
[Serializable]
public class OppositePlayer : ITargetPlayerValue
{    
    public ITargetPlayerValue Reference;

    public Option<IPlayerEntity> Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        var referenceOpt = Reference.Eval(gameWatcher, source);
        return
            referenceOpt.FlatMap(reference => 
                reference.Faction == Faction.Ally ? (gameWatcher.GameStatus.Ally as IPlayerEntity).Some() :
                reference.Faction == Faction.Enemy ? (gameWatcher.GameStatus.Enemy as IPlayerEntity).Some() :
                Option.None<IPlayerEntity>());
    }
}
[Serializable]
public class CardCaster : ITargetPlayerValue
{
    public Option<IPlayerEntity> Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return source switch
        {
            CardSource cardSource => cardSource.Card.Owner(gameWatcher),
            _ => Option.None<IPlayerEntity>()
        };
    }
}
[Serializable]
public class OwnerOfPlayerBuff : ITargetPlayerValue
{
    public ITargetPlayerBuffValue PlayerBuff;

    public Option<IPlayerEntity> Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        var playerBuffOpt = PlayerBuff.Eval(gameWatcher, source);
        return playerBuffOpt.FlatMap(playerBuff => playerBuff.Owner);
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
        return Array.Empty<IPlayerEntity>();
    }
}
[Serializable]
public class SinglePlayerCollection : ITargetPlayerCollectionValue
{
    public ITargetPlayerValue Target;

    public IReadOnlyCollection<IPlayerEntity> Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return Target.Eval(gameWatcher, source).ToEnumerable().ToList();
    }
}
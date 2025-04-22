using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using UniRx;
using UnityEngine;

public static class TargetPlayerValueExtensions
{
    public static Option<IPlayerEntity> Eval(
        this ITargetPlayerValue targetPlayerValue, 
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger,
        IActionSource source)
    {
        return targetPlayerValue.Eval(gameWatcher, trigger.SomeNotNull(), source.SomeNotNull());
    }
    public static Option<IPlayerEntity> Eval(
        this ITargetPlayerValue targetPlayerValue, 
        IGameplayStatusWatcher gameWatcher,
        IActionSource source)
    {
        return targetPlayerValue.Eval(gameWatcher, Option.None<ITriggerSource>(), source.SomeNotNull());
    }
    public static Option<IPlayerEntity> Eval(
        this ITargetPlayerValue targetPlayerValue, 
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource trigger)
    {
        return targetPlayerValue.Eval(gameWatcher, trigger.SomeNotNull(), Option.None<IActionSource>());
    }

    public static IReadOnlyCollection<IPlayerEntity> Eval(
        this ITargetPlayerCollectionValue targetPlayerColleionValue, 
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger,
        IActionSource source)
    {
        return targetPlayerColleionValue.Eval(gameWatcher, trigger.SomeNotNull(), source.SomeNotNull());
    }
    public static IReadOnlyCollection<IPlayerEntity> Eval(
        this ITargetPlayerCollectionValue targetPlayerColleionValue, 
        IGameplayStatusWatcher gameWatcher,
        IActionSource source)
    {
        return targetPlayerColleionValue.Eval(gameWatcher, Option.None<ITriggerSource>(), source.SomeNotNull());
    }
    public static IReadOnlyCollection<IPlayerEntity> Eval(
        this ITargetPlayerCollectionValue targetPlayerColleionValue, 
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource trigger)
    {
        return targetPlayerColleionValue.Eval(gameWatcher,trigger.SomeNotNull(), Option.None<IActionSource>());
    }
}

public interface ITargetPlayerValue
{
    Option<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        Option<ITriggerSource> trigger,
        Option<IActionSource> source);
}

[Serializable]
public class NonePlayer : ITargetPlayerValue
{
    public Option<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        Option<ITriggerSource> trigger,
        Option<IActionSource> source)
    {
        return Option.None<IPlayerEntity>();
    }
}
[Serializable]
public class CurrentPlayer : ITargetPlayerValue
{
    public Option<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        Option<ITriggerSource> trigger,
        Option<IActionSource> source)
    {
        return gameWatcher.GameStatus.CurrentPlayer;
    }
}
[Serializable]
public class OppositePlayer : ITargetPlayerValue
{    
    public ITargetPlayerValue Reference;

    public Option<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        Option<ITriggerSource> trigger,
        Option<IActionSource> source)
    {
        var referenceOpt = Reference.Eval(gameWatcher, trigger, source);
        return
            referenceOpt.FlatMap(reference => 
                reference.Faction == Faction.Ally ? (gameWatcher.GameStatus.Ally as IPlayerEntity).Some() :
                reference.Faction == Faction.Enemy ? (gameWatcher.GameStatus.Enemy as IPlayerEntity).Some() :
                Option.None<IPlayerEntity>());
    }
}
[Serializable]
public class CasterOfCard : ITargetPlayerValue
{
    public ITargetCardValue Card;

    public Option<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        Option<ITriggerSource> trigger,
        Option<IActionSource> source)
    {
        var cardOpt = Card.Eval(gameWatcher, trigger, source);
        return cardOpt.FlatMap(card => card.Owner(gameWatcher));
    }
}
[Serializable]
public class OwnerOfPlayerBuff : ITargetPlayerValue
{
    public ITargetPlayerBuffValue PlayerBuff;

    public Option<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        Option<ITriggerSource> trigger,
        Option<IActionSource> source)
    {
        var playerBuffOpt = PlayerBuff.Eval(gameWatcher, trigger, source);
        return playerBuffOpt.FlatMap(playerBuff => playerBuff.Owner);
    }
}


public interface ITargetPlayerCollectionValue
{
    IReadOnlyCollection<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher,
        Option<ITriggerSource> trigger,
        Option<IActionSource> source);
}
[Serializable]
public class NonePlayers : ITargetPlayerCollectionValue
{
    public IReadOnlyCollection<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher,
        Option<ITriggerSource> trigger,
        Option<IActionSource> source)
    {
        return Array.Empty<IPlayerEntity>();
    }
}
[Serializable]
public class SinglePlayerCollection : ITargetPlayerCollectionValue
{
    public ITargetPlayerValue Target;

    public IReadOnlyCollection<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher,
        Option<ITriggerSource> trigger,
        Option<IActionSource> source)
    {
        return Target.Eval(gameWatcher, trigger, source).ToEnumerable().ToList();
    }
}
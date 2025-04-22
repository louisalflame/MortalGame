using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using UnityEngine;

public static class TargetCardValueExtensions
{
    public static Option<ICardEntity> Eval(
        this ITargetCardValue targetCardValue, 
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger,
        IActionSource source)
    {
        return targetCardValue.Eval(gameWatcher, trigger.SomeNotNull(), source.SomeNotNull());
    }
    public static Option<ICardEntity> Eval(
        this ITargetCardValue targetCardValue, 
        IGameplayStatusWatcher gameWatcher,
        IActionSource source)
    {
        return targetCardValue.Eval(gameWatcher, Option.None<ITriggerSource>(), source.SomeNotNull());
    }

    public static IReadOnlyCollection<ICardEntity> Eval(
        this ITargetCardCollectionValue targetCardColleionValue, 
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger,
        IActionSource source)
    {
        return targetCardColleionValue.Eval(gameWatcher, trigger.SomeNotNull(), source.SomeNotNull());
    }
    public static IReadOnlyCollection<ICardEntity> Eval(
        this ITargetCardCollectionValue targetCardColleionValue, 
        IGameplayStatusWatcher gameWatcher, 
        IActionSource source)
    {
        return targetCardColleionValue.Eval(gameWatcher,  Option.None<ITriggerSource>(), source.SomeNotNull());
    }
}

public interface ITargetCardValue
{
    Option<ICardEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        Option<ITriggerSource> trigger, 
        Option<IActionSource> source);
}

[Serializable]
public class NoneCard : ITargetCardValue
{
    public Option<ICardEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        Option<ITriggerSource> trigger, 
        Option<IActionSource> source)
    {
        return Option.None<ICardEntity>();
    }
}
[Serializable]
public class SelectedCard : ITargetCardValue
{
    public Option<ICardEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        Option<ITriggerSource> trigger, 
        Option<IActionSource> source)
    {
        return gameWatcher.GameContext.SelectedCard.Some();
    }
}

public interface ITargetCardCollectionValue
{
    IReadOnlyCollection<ICardEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        Option<ITriggerSource> trigger,
        Option<IActionSource> source);
}

[Serializable]
public class SingleCardCollection : ITargetCardCollectionValue
{
    public ITargetCardValue TargetCard;

    public IReadOnlyCollection<ICardEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        Option<ITriggerSource> trigger, 
        Option<IActionSource> source)
    {
        return TargetCard.Eval(gameWatcher, trigger, source).ToEnumerable().ToList();
    }
}
[Serializable]
public class AllyHandCards : ITargetCardCollectionValue
{
    public ITargetPlayerValue Player;

    public IReadOnlyCollection<ICardEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        Option<ITriggerSource> trigger, 
        Option<IActionSource> source)
    {
        var playerOpt = Player.Eval(gameWatcher, trigger, source);
        return playerOpt.Map(player => player.CardManager.HandCard.Cards).ValueOr(Array.Empty<ICardEntity>());
    }
}


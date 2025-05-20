using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using Sirenix.OdinInspector;
using UnityEngine;

public interface ITargetCardValue
{
    Option<ICardEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger,
        IActionUnit actionUnit);
}

[Serializable]
public class NoneCard : ITargetCardValue
{
    public Option<ICardEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger,
        IActionUnit actionUnit)
    {
        return Option.None<ICardEntity>();
    }
}
[Serializable]
public class SelectedCard : ITargetCardValue
{
    public Option<ICardEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger,
        IActionUnit actionUnit)
    {
        return gameWatcher.GameContext.SelectedCard.SomeNotNull();
    }
}
[Serializable]
public class PlayingCard : ITargetCardValue
{
    public Option<ICardEntity> Eval(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource trigger,
        IActionUnit actionUnit)
    {
        return actionUnit switch
        {
            IIntentAction intentAction =>
                intentAction.Source is CardPlaySource cardPlaySource ?
                    cardPlaySource.Card.SomeNotNull() :
                    Option.None<ICardEntity>(),
            _ => Option.None<ICardEntity>()
        };
    }
}
[Serializable]
public class IndexOfCardCollection : ITargetCardValue
{
    [HorizontalGroup("1")]
    public ITargetCardCollectionValue CardCollection;

    [HorizontalGroup("2")]
    public IIntegerValue Index;

    public Option<ICardEntity> Eval(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource trigger,
        IActionUnit actionUnit)
    {
        var cards = CardCollection.Eval(gameWatcher, trigger, actionUnit);
        var index = Index.Eval(gameWatcher, trigger, actionUnit);
        return cards.Count > index && index >= 0 ?
            cards.ElementAt(index).SomeNotNull() :
            Option.None<ICardEntity>();
    }
}   

public interface ITargetCardCollectionValue
{
    IReadOnlyCollection<ICardEntity> Eval(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource trigger,
        IActionUnit actionUnit);
}

[Serializable]
public class SingleCardCollection : ITargetCardCollectionValue
{
    [HorizontalGroup("1")]
    public ITargetCardValue TargetCard;

    public IReadOnlyCollection<ICardEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger,
        IActionUnit actionUnit)
    {
        return TargetCard
            .Eval(gameWatcher, trigger, actionUnit)
            .ToEnumerable().ToList();
    }
}
[Serializable]
public class AllyHandCards : ITargetCardCollectionValue
{
    [HorizontalGroup("1")]
    public ITargetPlayerValue Player;

    public IReadOnlyCollection<ICardEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger,
        IActionUnit actionUnit)
    {
        var playerOpt = Player.Eval(gameWatcher, trigger, actionUnit);
        return playerOpt
            .Map(player => player.CardManager.HandCard.Cards)
            .ValueOr(Array.Empty<ICardEntity>());
    }
}


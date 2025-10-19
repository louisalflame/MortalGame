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
        return gameWatcher.ContextManager.Context.SelectedCard.SomeNotNull();
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
        return actionUnit.Source switch
        {
            CardPlaySource cardPlaySource =>
                cardPlaySource.Card.SomeNotNull(),
            CardPlayResultSource cardPlayResultSource =>
                cardPlayResultSource.CardPlaySource.Card.SomeNotNull(),
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

    public OrderType Order;

    public Option<ICardEntity> Eval(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource trigger,
        IActionUnit actionUnit)
    {
        var cards = CardCollection.Eval(gameWatcher, trigger, actionUnit);
        var orderedCards = Order switch
        {
            OrderType.Ascending => cards.ToList(),
            OrderType.Descending => cards.Reverse().ToList(),
            _ => cards.ToList()
        };
        var index = Index.Eval(gameWatcher, trigger, actionUnit);
        return cards.Count > index && index >= 0 ?
            orderedCards.ElementAt(index).SomeNotNull() :
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


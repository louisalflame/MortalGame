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
        ITriggerSource trigger);
}

[Serializable]
public class NoneCard : ITargetCardValue
{
    public Option<ICardEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger)
    {
        return Option.None<ICardEntity>();
    }
}
[Serializable]
public class SelectedCard : ITargetCardValue
{
    public Option<ICardEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger)
    {
        return gameWatcher.GameContext.SelectedCard.SomeNotNull();
    }
}
[Serializable]
public class PlayingCard : ITargetCardValue
{
    public Option<ICardEntity> Eval(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource trigger)
    {
        return trigger switch
        {
            CardPlayTrigger cardPlay => cardPlay.Card.SomeNotNull(),
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
        ITriggerSource trigger)
    {
        var cards = CardCollection.Eval(gameWatcher, trigger);
        var index = Index.Eval(gameWatcher, trigger);
        return cards.Count > index && index >= 0 ?
            cards.ElementAt(index).SomeNotNull() :
            Option.None<ICardEntity>();
    }
}   

public interface ITargetCardCollectionValue
{
    IReadOnlyCollection<ICardEntity> Eval(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource trigger);
}

[Serializable]
public class SingleCardCollection : ITargetCardCollectionValue
{
    [HorizontalGroup("1")]
    public ITargetCardValue TargetCard;

    public IReadOnlyCollection<ICardEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger)
    {
        return TargetCard.Eval(gameWatcher, trigger).ToEnumerable().ToList();
    }
}
[Serializable]
public class AllyHandCards : ITargetCardCollectionValue
{
    [HorizontalGroup("1")]
    public ITargetPlayerValue Player;

    public IReadOnlyCollection<ICardEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger)
    {
        var playerOpt = Player.Eval(gameWatcher, trigger);
        return playerOpt.Map(player => player.CardManager.HandCard.Cards).ValueOr(Array.Empty<ICardEntity>());
    }
}


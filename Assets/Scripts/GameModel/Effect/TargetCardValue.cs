using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
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
        return gameWatcher.GameContext.SelectedCard.Some();
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
    public ITargetPlayerValue Player;

    public IReadOnlyCollection<ICardEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger)
    {
        var playerOpt = Player.Eval(gameWatcher, trigger);
        return playerOpt.Map(player => player.CardManager.HandCard.Cards).ValueOr(Array.Empty<ICardEntity>());
    }
}


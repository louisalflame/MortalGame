using System;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetCardValue
{
    ICardEntity Eval(IGameplayStatusWatcher gameWatcher, IActionSource source);
}

[Serializable]
public class NoneCard : ITargetCardValue
{
    public ICardEntity Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return CardEntity.DummyCard;
    }
}
[Serializable]
public class SelectedCard : ITargetCardValue
{
    public ICardEntity Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return gameWatcher.GameContext.SelectedCard;
    }
}

public interface ITargetCardCollectionValue
{
    IReadOnlyCollection<ICardEntity> Eval(IGameplayStatusWatcher gameWatcher, IActionSource source);
}

[Serializable]
public class SingleCardCollection : ITargetCardCollectionValue
{
    public ITargetCardValue TargetCard;

    public IReadOnlyCollection<ICardEntity> Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return new [] { TargetCard.Eval(gameWatcher, source) };
    }
}
[Serializable]
public class AllyHandCards : ITargetCardCollectionValue
{
    public IReadOnlyCollection<ICardEntity> Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return source switch
        {
            CardSource cardSource   => cardSource.Card.Owner.CardManager.HandCard.Cards,
            SystemSource _          => Array.Empty<ICardEntity>(),
            _                       => Array.Empty<ICardEntity>()
        };
    }
}


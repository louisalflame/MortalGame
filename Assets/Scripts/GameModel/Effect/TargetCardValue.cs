using System;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetCardValue
{
    ICardEntity Eval(IGameplayStatusWatcher gameWatcher);
}

[Serializable]
public class NoneCard : ITargetCardValue
{
    public ICardEntity Eval(IGameplayStatusWatcher gameWatcher)
    {
        return CardEntity.DummyCard;
    }
}
[Serializable]
public class SelectedCard : ITargetCardValue
{
    public ICardEntity Eval(IGameplayStatusWatcher gameWatcher)
    {
        return gameWatcher.GameContext.SelectedCard;
    }
}

public interface ITargetCardCollectionValue
{
    IReadOnlyCollection<ICardEntity> Eval(IGameplayStatusWatcher gameWatcher);
}

[Serializable]
public class SingleCardCollection : ITargetCardCollectionValue
{
    public ITargetCardValue TargetCard;

    public IReadOnlyCollection<ICardEntity> Eval(IGameplayStatusWatcher gameWatcher)
    {
        return new [] { TargetCard.Eval(gameWatcher) };
    }
}
[Serializable]
public class AllyHandCards : ITargetCardCollectionValue
{
    public IReadOnlyCollection<ICardEntity> Eval(IGameplayStatusWatcher gameWatcher)
    {
        return gameWatcher.GameContext.ExecutePlayer.CardManager.HandCard.Cards;
    }
}


using System;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetCardValue
{
    ICardEntity Eval(GameStatus gameStatus, GameContext context);
}

[Serializable]
public class NoneCard : ITargetCardValue
{
    public ICardEntity Eval(GameStatus gameStatus, GameContext context)
    {
        return CardEntity.DummyCard;
    }
}
[Serializable]
public class SelectedCard : ITargetCardValue
{
    public ICardEntity Eval(GameStatus gameStatus, GameContext context)
    {
        return context.SelectedCard;
    }
}

public interface ITargetCardCollectionValue
{
    IReadOnlyCollection<ICardEntity> Eval(GameStatus gameStatus, GameContext context);
}
[Serializable]
public class SingleCardCollection : ITargetCardCollectionValue
{
    public ITargetCardValue TargetCard;

    public IReadOnlyCollection<ICardEntity> Eval(GameStatus gameStatus, GameContext context)
    {
        return new [] { TargetCard.Eval(gameStatus, context) };
    }
}
[Serializable]
public class AllyHandCards : ITargetCardCollectionValue
{
    public IReadOnlyCollection<ICardEntity> Eval(GameStatus gameStatus, GameContext context)
    {
        return context.ExecutePlayer.CardManager.HandCard.Cards;
    }
}


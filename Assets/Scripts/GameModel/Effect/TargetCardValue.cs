using System;
using UnityEngine;

public interface ITargetCardValue
{
    CardEntity Eval(GameStatus gameStatus, GameContext context);
}

[Serializable]
public class NoneCard : ITargetCardValue
{
    public CardEntity Eval(GameStatus gameStatus, GameContext context)
    {
        return CardEntity.DummyCard;
    }
}
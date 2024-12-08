using System;
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
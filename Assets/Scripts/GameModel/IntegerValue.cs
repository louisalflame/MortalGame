using System;

public interface IIntegerValue
{
    int Eval(GameStatus gameStatus, GameContext gameContext);
}

[Serializable]
public class ThisCardPower : IIntegerValue
{   
    public int Eval(GameStatus gameStatus, GameContext gameContext)
    {
        return gameContext.UsingCard.Power;
    }
}

[Serializable]
public class ThisCardCost : IIntegerValue
{
    public int Eval(GameStatus gameStatus, GameContext gameContext)
    {
        return gameContext.UsingCard.Cost;
    }
}
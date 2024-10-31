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
        return gameContext.UsingCard.EvalPower(gameContext);
    }
}

[Serializable]
public class ThisCardCost : IIntegerValue
{
    public int Eval(GameStatus gameStatus, GameContext gameContext)
    {
        return gameContext.UsingCard.EvalCost(gameContext);
    }
}

[Serializable]
public class ConstInteger : IIntegerValue
{
    public int Value;

    public int Eval(GameStatus gameStatus, GameContext gameContext)
    {
        return Value;
    }
}

[Serializable]
public class ThisBuffLevel : IIntegerValue
{
    public int Eval(GameStatus gameStatus, GameContext gameContext)
    {
        return gameContext.UsingBuff.Level;
    }
}
using System;

public interface IIntegerValue
{
    int Eval(IGameplayStatusWatcher gameWatcher);
}

[Serializable]
public class ThisCardPower : IIntegerValue
{   
    public int Eval(IGameplayStatusWatcher gameWatcher)
    {
        return gameWatcher.GameContext.UsingCard.EvalPower(gameWatcher);
    }
}

[Serializable]
public class ThisCardCost : IIntegerValue
{
    public int Eval(IGameplayStatusWatcher gameWatcher)
    {
        return gameWatcher.GameContext.UsingCard.EvalCost(gameWatcher);
    }
}

[Serializable]
public class ConstInteger : IIntegerValue
{
    public int Value;

    public int Eval(IGameplayStatusWatcher gameWatcher)
    {
        return Value;
    }
}

[Serializable]
public class ThisBuffLevel : IIntegerValue
{
    public int Eval(IGameplayStatusWatcher gameWatcher)
    {
        return gameWatcher.GameContext.TriggeredBuff.Level;
    }
}
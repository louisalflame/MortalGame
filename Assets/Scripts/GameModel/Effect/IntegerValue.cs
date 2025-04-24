using System;

public interface IIntegerValue
{
    int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource);
}

[Serializable]
public class ThisCardPower : IIntegerValue
{   
    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource)
    {
        return triggerSource switch
        {
            CardPlay cardPlay => cardPlay.Card.Power,
            _ => 0
        };
    }
}

[Serializable]
public class ConstInteger : IIntegerValue
{
    public int Value;

    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource)
    {
        return Value;
    }
}

[Serializable]
public class ThisBuffLevel : IIntegerValue
{
    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource)
    {
        return triggerSource switch
        {
            PlayerBuffTrigger playerBuffTrigger => playerBuffTrigger.Buff.Level,
            CharacterBuffTrigger characterBuffTrigger => characterBuffTrigger.Buff.Level,
            _ => 0
        };
    }
}
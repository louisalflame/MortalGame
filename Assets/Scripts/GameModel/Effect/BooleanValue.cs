using System;

public interface IBooleanValue
{
    bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource);
}

[Serializable]
public class TrueValue : IBooleanValue
{
    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource)
    {
        return true;
    }
}
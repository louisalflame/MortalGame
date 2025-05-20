using System;

public interface IBooleanValue
{
    bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource, IActionUnit actionUnit);
}

[Serializable]
public class TrueValue : IBooleanValue
{
    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource, IActionUnit actionUnit)
    {
        return true;
    }
}
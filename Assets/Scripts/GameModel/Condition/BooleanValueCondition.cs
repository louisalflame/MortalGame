using System;
using UnityEngine;

public interface IBooleanValueCondition
{
    bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit, bool value);
}

[Serializable]
public class IsTrueCondition : IBooleanValueCondition
{
    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit, bool value)
    {
        return value;
    }
}

[Serializable]
public class IsFalseCondition : IBooleanValueCondition
{
    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit, bool value)
    {
        return !value;
    }
}

[Serializable]
public class IsEqualCondition : IBooleanValueCondition
{
    public IBooleanValue Boolean;

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit, bool value)
    {
        return value == Boolean.Eval(gameWatcher, source, actionUnit);
    }
}
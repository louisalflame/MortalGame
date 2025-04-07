using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
 

public interface ICondition
{
    bool Eval(IGameplayStatusWatcher gameWatcher);
}

[Serializable]
public class TrueCondition : ICondition
{
    public bool Eval(IGameplayStatusWatcher gameWatcher)
    {
        return true;
    }
}

[Serializable]
public class FalseCondition : ICondition
{
    public bool Eval(IGameplayStatusWatcher gameWatcher)
    {
        return false;
    }
}

[Serializable]
public class AllCondition : ICondition
{
    public List<ICondition> Conditions;

    public bool Eval(IGameplayStatusWatcher gameWatcher)
    {
        return Conditions.All(condition => condition.Eval(gameWatcher));
    }
}

[Serializable]
public class AnyCondition : ICondition
{
    public List<ICondition> Conditions;

    public bool Eval(IGameplayStatusWatcher gameWatcher)
    {
        return Conditions.Any(condition => condition.Eval(gameWatcher));
    }
}
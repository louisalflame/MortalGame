using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
 

public interface ICondition
{
    bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source);
}

public interface ICardBuffCondition : ICondition { }
public interface IPlayerBuffCondition : ICondition { }
public interface ICharacterBuffCondition : ICondition { }

[Serializable]
public class ConstCondition : ICardBuffCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    public bool Value;
    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source)
    {
        return Value;
    }
}

[Serializable]
public class AllCondition : ICardBuffCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    public List<ICondition> Conditions;

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source)
    {
        return Conditions.All(condition => condition.Eval(gameWatcher, source));
    }
}

[Serializable]
public class AnyCondition : ICardBuffCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    public List<ICondition> Conditions;

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source)
    {
        return Conditions.Any(condition => condition.Eval(gameWatcher, source));
    }
}

[Serializable]
public class IsSelfTurnCondition : IPlayerBuffCondition, ICharacterBuffCondition
{
    public ITargetPlayerValue TargetPlayer;

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return gameWatcher.GameStatus.CurrentPlayer.Match(
            currnentPlayer => TargetPlayer.Eval(gameWatcher, trigger).Match(
                                targetPlayer => currnentPlayer == targetPlayer,
                                ()           => false),
            ()             => false
        );
    }
}

[Serializable]
public class IntegerCondition : ICardBuffCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    public IIntegerValue Value1;
    public IIntegerValue Value2;
    public ArithmeticConditionType Condition;

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        var val1 = Value1.Eval(gameWatcher, trigger);
        var val2 = Value2.Eval(gameWatcher, trigger);
        return Condition switch
        {
            ArithmeticConditionType.Equal               => val1 == val2,
            ArithmeticConditionType.NotEqual            => val1 != val2,
            ArithmeticConditionType.GreaterThan         => val1 > val2,
            ArithmeticConditionType.LessThan            => val1 < val2,
            ArithmeticConditionType.GreaterThanOrEqual  => val1 >= val2,
            ArithmeticConditionType.LessThanOrEqual     => val1 <= val2,
            _ => false
        };
    }
}
using System;
using Optional;
using UnityEngine;

public interface IIntegerValueCondition
{
    bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, int value);
}

[Serializable]
public class IntegerCompare : IIntegerValueCondition
{
    public ArithmeticConditionType Arithmetic;
    public IIntegerValue CompareValue;

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, int value)
    {
        var compareValue = CompareValue.Eval(gameWatcher, source);
        return Arithmetic switch
        {
            ArithmeticConditionType.Equal               => value == compareValue,
            ArithmeticConditionType.NotEqual            => value != compareValue,
            ArithmeticConditionType.GreaterThan         => value > compareValue,
            ArithmeticConditionType.LessThan            => value < compareValue,
            ArithmeticConditionType.GreaterThanOrEqual  => value >= compareValue,
            ArithmeticConditionType.LessThanOrEqual     => value <= compareValue,
            _ => false
        };
    }
}
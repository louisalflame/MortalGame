using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;


[Serializable]
public class ConditionUpdateRule<T>
{
    [ShowInInspector]
    public ICondition[] Conditions = new ICondition[0];

    public T NewValue;
}


[Serializable]
public class BooleanUpdateRules : Dictionary<UpdateTiming, ConditionUpdateRule<IBooleanValue>[]> 
{ }

[Serializable]
public class IntegerUpdateRules : Dictionary<UpdateTiming, ConditionUpdateRule<IIntegerValue>[]>
{ }
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;


[Serializable]
public class ConditionUpdateRule<T>
{
    [ShowInInspector]
    
    [PropertyOrder(1)]
    public ICondition[] Conditions = new ICondition[0];
    
    [PropertyOrder(3)]
    public T NewValue;
}

[Serializable]
public class ConditionBooleanUpdateRule : ConditionUpdateRule<IBooleanValue> 
{
    public enum UpdateType
    {
        Overwrite,
        AndOrigin,
        OrOrigin,
    }
    
    [PropertyOrder(2)]
    public UpdateType Operation = UpdateType.Overwrite;
 }
 [Serializable]
public class ConditionIntegerUpdateRule : ConditionUpdateRule<IIntegerValue>
{
    public enum UpdateType
    {
        Overwrite,
        AddOrigin,
    }

    [PropertyOrder(2)]
    public UpdateType Operation = UpdateType.Overwrite;
}


[Serializable]
public class BooleanUpdateTimingRules : Dictionary<UpdateTiming, ConditionBooleanUpdateRule[]> 
{ }
[Serializable]
public class BooleanUpdateIntentRules : Dictionary<UpdateAction, ConditionBooleanUpdateRule[]> 
{ }
[Serializable]
public class BooleanUpdateResultRules : Dictionary<UpdateAction, ConditionBooleanUpdateRule[]> 
{ }

[Serializable]
public class IntegerUpdateTimingRules : Dictionary<UpdateTiming, ConditionIntegerUpdateRule[]>
{ }
[Serializable]
public class IntegerUpdateIntentRules : Dictionary<UpdateAction, ConditionIntegerUpdateRule[]>
{ }
[Serializable]
public class IntegerUpdateResultRules : Dictionary<UpdateAction, ConditionIntegerUpdateRule[]>
{ }
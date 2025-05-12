using System;
using Sirenix.OdinInspector;
using UnityEngine;

public interface ISessionValueData
{
    ISessionValueEntity GetEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger);
}


[Serializable]
public class SessionBoolean : ISessionValueData
{
    public bool Value;

    [ShowInInspector]
    public BooleanUpdateTimingRules TimingRules = new();
    [ShowInInspector]
    public BooleanUpdateIntentRules IntentRules = new();
    [ShowInInspector]
    public BooleanUpdateResultRules ResultRules = new();
    
    public ISessionValueEntity GetEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new SessionBooleanEntity(Value, TimingRules, IntentRules, ResultRules);
    }
}

[Serializable]
public class SessionInteger : ISessionValueData
{
    public int Value;

    [ShowInInspector]
    public IntegerUpdateTimingRules TimingRules = new();
    [ShowInInspector]
    public IntegerUpdateIntentRules IntentRules = new();
    [ShowInInspector]
    public IntegerUpdateResultRules ResultRules = new();

    public ISessionValueEntity GetEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new SessionIntegerEntity(Value, TimingRules, IntentRules, ResultRules);
    }
}

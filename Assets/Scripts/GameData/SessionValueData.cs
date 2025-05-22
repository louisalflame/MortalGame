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
    [ShowInInspector]
    public BooleanUpdateTimingRules TimingRules = new();
    [ShowInInspector]
    public BooleanUpdateIntentRules IntentRules = new();
    [ShowInInspector]
    public BooleanUpdateResultRules ResultRules = new();
    
    public ISessionValueEntity GetEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new SessionBooleanEntity(TimingRules, IntentRules, ResultRules);
    }
}

[Serializable]
public class SessionInteger : ISessionValueData
{
    [ShowInInspector]
    public IntegerUpdateTimingRules TimingRules = new();
    [ShowInInspector]
    public IntegerUpdateIntentRules IntentRules = new();
    [ShowInInspector]
    public IntegerUpdateResultRules ResultRules = new();

    public ISessionValueEntity GetEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new SessionIntegerEntity(TimingRules, IntentRules, ResultRules);
    }
}

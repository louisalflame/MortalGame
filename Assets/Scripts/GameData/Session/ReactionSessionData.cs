using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IReactionSessionData
{
    IReactionSessionEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit);
}

[Serializable]
public class SessionBoolean : IReactionSessionData
{
    public bool InitialValue;
    public SessionLifeTime LifeTime;

    [ShowInInspector]
    public BooleanUpdateTimingRules TimingRules = new();
    [ShowInInspector]
    public BooleanUpdateIntentRules IntentRules = new();
    [ShowInInspector]
    public BooleanUpdateResultRules ResultRules = new();
    
    public IReactionSessionEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        return new ReactionSessionEntity(
            new SessionBooleanEntity(InitialValue, TimingRules, IntentRules, ResultRules),
            LifeTime);
    }
}

[Serializable]
public class SessionInteger : IReactionSessionData
{
    public int InitialValue;
    public SessionLifeTime LifeTime;
    
    [ShowInInspector]
    public IntegerUpdateTimingRules TimingRules = new();
    [ShowInInspector]
    public IntegerUpdateIntentRules IntentRules = new();
    [ShowInInspector]
    public IntegerUpdateResultRules ResultRules = new();

    public IReactionSessionEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        return new ReactionSessionEntity(
            new SessionIntegerEntity(InitialValue, TimingRules, IntentRules, ResultRules),
            LifeTime);
    }
}

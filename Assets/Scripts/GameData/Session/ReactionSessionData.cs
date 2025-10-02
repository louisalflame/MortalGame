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
    [Serializable]
    public class TimingRule
    {
        [ValueDropdown("@DropdownHelper.UpdateTimings")]
        public GameTiming Timing;

        public ConditionBooleanUpdateRule[] Rules;
    }

    public bool InitialValue;
    public SessionLifeTime LifeTime;

    [ShowInInspector]
    [TableList]
    public List<TimingRule> UpdateRules = new ();
    
    public IReactionSessionEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        return new ReactionSessionEntity(
            new SessionBooleanEntity(InitialValue, UpdateRules),
            LifeTime);
    }
}

[Serializable]
public class SessionInteger : IReactionSessionData
{
    [Serializable]
    public class TimingRule
    {
        [ValueDropdown("@DropdownHelper.UpdateTimings")]
        public GameTiming Timing;

        public ConditionIntegerUpdateRule[] Rules;
    }

    public int InitialValue;
    public SessionLifeTime LifeTime;
    
    [ShowInInspector]
    [TableList]
    public List<TimingRule> UpdateRules = new ();

    public IReactionSessionEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        return new ReactionSessionEntity(
            new SessionIntegerEntity(InitialValue, UpdateRules),
            LifeTime);
    }
}

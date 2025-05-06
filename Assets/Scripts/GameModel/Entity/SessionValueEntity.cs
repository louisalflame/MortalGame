using System;
using System.Linq;

public interface ISessionValueEntity
{
    void UpdateByTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing);
    void UpdateIntent(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IIntentAction intent);
    void UpdateResult(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IResultAction result);
}

[Serializable]
public class SessionBooleanEntity : ISessionValueEntity
{
    public bool Value;
    public readonly BooleanUpdateTimingRules TimingRules;
    public readonly BooleanUpdateIntentRules IntentRules;
    public readonly BooleanUpdateResultRules ResultRules;
    
    public SessionBooleanEntity(
        bool value, 
        BooleanUpdateTimingRules timingRules,
        BooleanUpdateIntentRules intentRules, 
        BooleanUpdateResultRules resultRules)
    {
        Value = value;
        TimingRules = timingRules;
        IntentRules = intentRules;
        ResultRules = resultRules;
    }

    public void UpdateByTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing)
    {
        if (TimingRules.TryGetValue(timing, out var rules))
        {
            foreach (var rule in rules)
            {
                //TODO
                /*if (rule.Conditions.All(condition => condition.Eval(gameWatcher, )))
                {
                    Value = rule.NewValue.Eval(gameWatcher);
                    break;
                }
                */
            }
        }
    }

    public void UpdateIntent(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IIntentAction intent)
    {
    }

    public void UpdateResult(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IResultAction result)
    {
    }
}

[Serializable]
public class SessionIntegerEntity : ISessionValueEntity
{
    public int Value;
    public readonly IntegerUpdateTimingRules TimingRules;
    public readonly IntegerUpdateIntentRules IntentRules;
    public readonly IntegerUpdateResultRules ResultRules;

    public SessionIntegerEntity(
        int value, 
        IntegerUpdateTimingRules timingRules,
        IntegerUpdateIntentRules intentRules,
        IntegerUpdateResultRules resultRules)
    {
        Value = value;
        TimingRules = timingRules;
        IntentRules = intentRules;
        ResultRules = resultRules;
    }

    public void UpdateByTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing)
    {
    }

    public void UpdateIntent(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IIntentAction intent)
    {
        if (IntentRules.TryGetValue(intent.ActionType, out var rules))
        {
            foreach (var rule in rules)
            {
                if (rule.Conditions.All(condition => condition.Eval(gameWatcher, trigger)))
                {
                    Value = rule.NewValue.Eval(gameWatcher, trigger);
                    break;
                }
                
            }
        }
    }

    public void UpdateResult(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IResultAction result)
    {
    }
}

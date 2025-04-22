using System;
using System.Linq;

public interface ISessionValueEntity
{
    void UpdateByTiming(IGameplayStatusWatcher gameWatcher, UpdateTiming timing);
    void UpdateIntent(IGameplayStatusWatcher gameWatcher, IIntentAction intent);
    void UpdateResult(IGameplayStatusWatcher gameWatcher, IResultAction result);
}

[Serializable]
public class SessionBooleanEntity : ISessionValueEntity
{
    public bool Value;
    public readonly BooleanUpdateRules TimingRules;
    
    public SessionBooleanEntity(bool value, BooleanUpdateRules timingRules)
    {
        Value = value;
        TimingRules = timingRules;
    }

    public void UpdateByTiming(IGameplayStatusWatcher gameWatcher, UpdateTiming timing)
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

    public void UpdateIntent(IGameplayStatusWatcher gameWatcher, IIntentAction intent)
    {
    }

    public void UpdateResult(IGameplayStatusWatcher gameWatcher, IResultAction result)
    {
    }
}

[Serializable]
public class SessionIntegerEntity : ISessionValueEntity
{
    public int Value;
    public readonly IntegerUpdateRules Rules;

    public SessionIntegerEntity(int value, IntegerUpdateRules rules)
    {
        Value = value;
        Rules = rules;
    }

    public void UpdateByTiming(IGameplayStatusWatcher gameWatcher, UpdateTiming timing)
    {
    }

    public void UpdateIntent(IGameplayStatusWatcher gameWatcher, IIntentAction intent)
    {
    }

    public void UpdateResult(IGameplayStatusWatcher gameWatcher, IResultAction result)
    {
    }
}

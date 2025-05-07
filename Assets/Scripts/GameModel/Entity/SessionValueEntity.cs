using System;
using System.Linq;

public interface ISessionValueEntity
{
    ISessionValueEntity Clone();
    void UpdateByTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing);
    void UpdateIntent(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IIntentAction intent);
    void UpdateResult(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IResultAction result);
}

[Serializable]
public class SessionBooleanEntity : ISessionValueEntity
{
    public bool Value;
    private readonly bool _initialValue;
    private readonly BooleanUpdateTimingRules _timingRules;
    private readonly BooleanUpdateIntentRules _intentRules;
    private readonly BooleanUpdateResultRules _resultRules;
    
    public SessionBooleanEntity(
        bool value, 
        BooleanUpdateTimingRules timingRules,
        BooleanUpdateIntentRules intentRules, 
        BooleanUpdateResultRules resultRules)
    {
        _initialValue = value;
        _timingRules = timingRules;
        _intentRules = intentRules;
        _resultRules = resultRules;
        Value = _initialValue;
    }

    public ISessionValueEntity Clone()
    {
        return new SessionBooleanEntity(
            Value, 
            _timingRules,
            _intentRules, 
            _resultRules);
    }

    public void UpdateByTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing)
    {
        if (_timingRules.TryGetValue(timing, out var rules))
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
    private readonly int _initialValue;
    private readonly IntegerUpdateTimingRules _timingRules;
    private readonly IntegerUpdateIntentRules _intentRules;
    private readonly IntegerUpdateResultRules _resultRules;

    public SessionIntegerEntity(
        int value, 
        IntegerUpdateTimingRules timingRules,
        IntegerUpdateIntentRules intentRules,
        IntegerUpdateResultRules resultRules)
    {
        _initialValue = value;
        _timingRules = timingRules;
        _intentRules = intentRules;
        _resultRules = resultRules;
        Value = _initialValue;
    }

    public ISessionValueEntity Clone()
    {
        return new SessionIntegerEntity(
            Value, 
            _timingRules,
            _intentRules, 
            _resultRules);
    }

    public void UpdateByTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing)
    {
    }

    public void UpdateIntent(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IIntentAction intent)
    {
        if (_intentRules.TryGetValue(intent.ActionType, out var rules))
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

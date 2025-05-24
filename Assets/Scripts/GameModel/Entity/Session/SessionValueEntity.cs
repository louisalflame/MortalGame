using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Optional;

public interface ISessionValueEntity
{
    ISessionValueEntity Clone();

    bool IsUpdated { get; }
    void Update(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit);
}

[Serializable]
public class SessionBooleanEntity : ISessionValueEntity
{
    public Option<bool> Value;
    private readonly BooleanUpdateTimingRules _timingRules;
    private readonly BooleanUpdateIntentRules _intentRules;
    private readonly BooleanUpdateResultRules _resultRules;
    
    public bool IsUpdated => Value.HasValue;
    
    public SessionBooleanEntity(
        BooleanUpdateTimingRules timingRules,
        BooleanUpdateIntentRules intentRules,
        BooleanUpdateResultRules resultRules)
    {
        _timingRules = timingRules;
        _intentRules = intentRules;
        _resultRules = resultRules;
    }

    public ISessionValueEntity Clone()
    {
        return new SessionBooleanEntity(
            _timingRules,
            _intentRules, 
            _resultRules);
    }

    private void _UpdateRules(
        IReadOnlyCollection<ConditionBooleanUpdateRule> rules,
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger,
        IActionUnit actionUnit)
    {
        foreach (var rule in rules)
        {
            if (rule.Conditions.All(condition => condition.Eval(gameWatcher, trigger, actionUnit)))
            {
                var oldVal = Value.ValueOr(false);
                var newVal = rule.NewValue.Eval(gameWatcher, trigger, actionUnit);
                var value = rule.Operation switch
                {
                    ConditionBooleanUpdateRule.UpdateType.AndOrigin => oldVal && newVal,
                    ConditionBooleanUpdateRule.UpdateType.OrOrigin => oldVal || newVal,
                    ConditionBooleanUpdateRule.UpdateType.Overwrite => newVal,
                    _ => oldVal
                };
                Value = value.Some();
            }
            break;
        }
    }

    public void Update(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        switch (actionUnit)
        {
            case UpdateTimingAction timingAction:
                if (_timingRules.TryGetValue(timingAction.Timing, out var timingRules))
                {
                    _UpdateRules(timingRules, gameWatcher, trigger, timingAction);
                }
                break;

            case IIntentAction intentAction:
                if (_intentRules.TryGetValue(intentAction.ActionType, out var intentRules))
                {
                    _UpdateRules(intentRules, gameWatcher, trigger, intentAction);
                }
                break;

            case IResultTargetAction resultAction:
                if (_resultRules.TryGetValue(resultAction.ActionType, out var resultRules))
                {
                    _UpdateRules(resultRules, gameWatcher, trigger, resultAction);
                }
                break;
        }
    }
}

[Serializable]
public class SessionIntegerEntity : ISessionValueEntity
{
    public Option<int> Value;
    private readonly IntegerUpdateTimingRules _timingRules;
    private readonly IntegerUpdateIntentRules _intentRules;
    private readonly IntegerUpdateResultRules _resultRules;

    public bool IsUpdated => Value.HasValue;

    public SessionIntegerEntity(
        IntegerUpdateTimingRules timingRules,
        IntegerUpdateIntentRules intentRules,
        IntegerUpdateResultRules resultRules)
    {
        _timingRules = timingRules;
        _intentRules = intentRules;
        _resultRules = resultRules;
    }

    public ISessionValueEntity Clone()
    {
        return new SessionIntegerEntity(
            _timingRules,
            _intentRules, 
            _resultRules);
    }

    private void _UpdateRules(
        IReadOnlyCollection<ConditionIntegerUpdateRule> rules,
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger,
        IActionUnit actionUnit)
    {
        foreach (var rule in rules)
        {
            if (rule.Conditions.All(condition => condition.Eval(gameWatcher, trigger, actionUnit)))
            {
                var oldVal = Value.ValueOr(0);
                var newVal = rule.NewValue.Eval(gameWatcher, trigger, actionUnit);
                var value = rule.Operation switch
                {
                    ConditionIntegerUpdateRule.UpdateType.AddOrigin => oldVal + newVal,
                    ConditionIntegerUpdateRule.UpdateType.Overwrite => newVal,
                    _ => oldVal
                };
                Value = value.Some();
            }
            break;
        }
    }

    public void Update(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        switch (actionUnit)
        {
            case UpdateTimingAction timingAction:
                if (_timingRules.TryGetValue(timingAction.Timing, out var timingRules))
                {
                    _UpdateRules(timingRules, gameWatcher, trigger, timingAction);
                }
                break;

            case IIntentAction intentAction:
                if (_intentRules.TryGetValue(intentAction.ActionType, out var intentRules))
                {
                    _UpdateRules(intentRules, gameWatcher, trigger, intentAction);
                }
                break;

            case IResultTargetAction resultAction:
                if (_resultRules.TryGetValue(resultAction.ActionType, out var resultRules))
                {
                    _UpdateRules(resultRules, gameWatcher, trigger, resultAction);
                }
                break;
        }
    }
}

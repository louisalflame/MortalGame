using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Optional;

public interface ISessionValueEntity
{
    ISessionValueEntity Clone();

    void Update(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit);
}

public class SessionBooleanEntity : ISessionValueEntity
{
    public bool Value;
    private readonly BooleanUpdateTimingRules _timingRules;
    private readonly BooleanUpdateIntentRules _intentRules;
    private readonly BooleanUpdateResultRules _resultRules;

    public SessionBooleanEntity(
        bool initialValue,
        BooleanUpdateTimingRules timingRules,
        BooleanUpdateIntentRules intentRules,
        BooleanUpdateResultRules resultRules)
    {
        Value = initialValue;
        _timingRules = timingRules;
        _intentRules = intentRules;
        _resultRules = resultRules;
    }

    public ISessionValueEntity Clone()
    {
        return new SessionBooleanEntity(
            Value,
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
                var newVal = rule.NewValue.Eval(gameWatcher, trigger, actionUnit);
                Value = rule.Operation switch
                {
                    ConditionBooleanUpdateRule.UpdateType.AndOrigin => Value && newVal,
                    ConditionBooleanUpdateRule.UpdateType.OrOrigin => Value || newVal,
                    ConditionBooleanUpdateRule.UpdateType.Overwrite => newVal,
                    _ => Value
                };
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

public class SessionIntegerEntity : ISessionValueEntity
{
    public int Value;
    private readonly IntegerUpdateTimingRules _timingRules;
    private readonly IntegerUpdateIntentRules _intentRules;
    private readonly IntegerUpdateResultRules _resultRules;

    public SessionIntegerEntity(
        int initialValue,
        IntegerUpdateTimingRules timingRules,
        IntegerUpdateIntentRules intentRules,
        IntegerUpdateResultRules resultRules)
    {
        Value = initialValue;
        _timingRules = timingRules;
        _intentRules = intentRules;
        _resultRules = resultRules;
    }

    public ISessionValueEntity Clone()
    {
        return new SessionIntegerEntity(
            Value,
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
                var newVal = rule.NewValue.Eval(gameWatcher, trigger, actionUnit);
                Value = rule.Operation switch
                {
                    ConditionIntegerUpdateRule.UpdateType.AddOrigin => Value + newVal,
                    ConditionIntegerUpdateRule.UpdateType.Overwrite => newVal,
                    _ => Value
                };
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

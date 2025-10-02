using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Optional;

public interface ISessionValueEntity
{
    ISessionValueEntity Clone();

    bool Update(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit);
}

public class SessionBooleanEntity : ISessionValueEntity
{
    public bool Value;
    private readonly List<SessionBoolean.TimingRule> _updateRules;

    public SessionBooleanEntity(
        bool initialValue,
        List<SessionBoolean.TimingRule> updateRules)
    {
        Value = initialValue;
        _updateRules = updateRules;
    }

    public ISessionValueEntity Clone()
    {
        return new SessionBooleanEntity(Value, _updateRules);
    }

    private bool _UpdateRules(
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
                return true;
            }
            break;
        }
        return false;
    }

    public bool Update(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        var isChanged = false;
        foreach (var rule in _updateRules.Where(r => r.Timing == actionUnit.Timing))
        {
            isChanged |= _UpdateRules(rule.Rules, gameWatcher, trigger, actionUnit);
        }
        return isChanged;
    }
}

public class SessionIntegerEntity : ISessionValueEntity
{
    public int Value;
    private readonly List<SessionInteger.TimingRule> _updateRules;

    public SessionIntegerEntity(
        int initialValue,
        List<SessionInteger.TimingRule> updateRules)
    {
        Value = initialValue;
        _updateRules = updateRules;
    }

    public ISessionValueEntity Clone()
    {
        return new SessionIntegerEntity(Value, _updateRules);
    }

    private bool _UpdateRules(
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
                return true;
            }
            break;
        }
        return false;
    }

    public bool Update(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        var isChanged = false;

        foreach (var rule in _updateRules.Where(r => r.Timing == actionUnit.Timing))
        {
            isChanged |= _UpdateRules(rule.Rules, gameWatcher, trigger, actionUnit);
        }
        return isChanged;
    }
}

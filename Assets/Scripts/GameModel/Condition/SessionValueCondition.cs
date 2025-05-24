using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IReactionSessionValueCondition
{
    bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit, IReactionSessionEntity sessionEntity);
}

[Serializable]
public class ReactorSessionUpdatedCondition : IReactionSessionValueCondition
{
    public string SessionKey;

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit, IReactionSessionEntity sessionEntity)
    {
        return sessionEntity.IsSessionValueUpdated(SessionKey);
    }
}
[Serializable]
public class ReactionSessionValueBooleanCondition : IReactionSessionValueCondition
{
    public string SessionKey;

    [ShowInInspector]
    [HorizontalGroup("1")]
    public List<IBooleanValueCondition> Conditions = new();

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit, IReactionSessionEntity sessionEntity)
    {
        return sessionEntity
            .GetSessionBoolean(SessionKey)
            .Match(
                value => Conditions.All(condition => condition.Eval(gameWatcher, source, actionUnit, value)),
                () => false);
    }
}

[Serializable]
public class ReactionSessionValueIntegerCondition : IReactionSessionValueCondition
{
    public string SessionKey;

    [ShowInInspector]
    [HorizontalGroup("1")]
    public List<IIntegerValueCondition> Conditions = new();

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit, IReactionSessionEntity sessionEntity)
    {
        return sessionEntity
            .GetSessionInteger(SessionKey)
            .Match(
                value => Conditions.All(condition => condition.Eval(gameWatcher, source, actionUnit, value)),
                () => false);
    }
}
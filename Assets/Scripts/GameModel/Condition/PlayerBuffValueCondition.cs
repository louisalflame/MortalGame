using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IPlayerBuffValueCondition
{
    bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit, IPlayerBuffEntity playerBuff);
}

[Serializable]
public class PlayerBuffSessionCondition : IPlayerBuffValueCondition
{
    [ShowInInspector]
    [HorizontalGroup("1")]
    public List<IReactionSessionValueCondition> Conditions = new();

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit, IPlayerBuffEntity playerBuff)
    {
        return playerBuff
            .ReactionSessions
            .Any(session => Conditions
                .Any(condition => condition.Eval(gameWatcher, source, actionUnit, session)));
    }
}
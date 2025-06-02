using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

public interface IPlayerBuffValueCondition
{
    bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit, IPlayerBuffEntity playerBuff);
}

[Serializable]
public class PlayerBuffSessionCondition : IPlayerBuffValueCondition
{
    [HorizontalGroup("1")]
    public string SessionKey;
    [ShowInInspector]
    [HorizontalGroup("2")]
    public List<IReactionSessionValueCondition> Conditions = new();

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit, IPlayerBuffEntity playerBuff)
    {
        return playerBuff.ReactionSessions.TryGetValue(SessionKey, out var session) &&
            Conditions.All(condition => condition.Eval(gameWatcher, source, actionUnit, session));
    }
}
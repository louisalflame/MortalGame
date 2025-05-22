using System.Collections.Generic;
using System.Linq;
using Optional;
using UnityEngine;

public interface IPlayerBuffValueCondition
{
    bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IPlayerBuffEntity playerBuff);
}

public class PlayerBuffSessionCondition : IPlayerBuffValueCondition
{
    public List<IReactionSessionValueCondition> Conditions = new();

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IPlayerBuffEntity playerBuff)
    {
        return playerBuff
            .ReactionSessions
            .Any(session => Conditions
                .Any(condition => condition.Eval(gameWatcher, source, session)));
    }
}

public interface IReactionSessionValueCondition
{
    bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IReactionSessionEntity sessionEntity);
}

public class ReactorSessionUpdatedCondition : IReactionSessionValueCondition
{
    public string SessionKey;
    public List<IReactionSessionValueCondition> Conditions = new();

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IReactionSessionEntity sessionEntity)
    {
        return sessionEntity.IsSessionValueUpdated(SessionKey);
    }
}
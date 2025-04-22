using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
 

public interface ICondition
{
    bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source);
}

public interface ICardBuffCondition : ICondition { }
public interface IPlayerBuffCondition : ICondition { }
public interface ICharacterBuffCondition : ICondition { }

[Serializable]
public class TrueCondition : ICardBuffCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source)
    {
        return true;
    }
}

[Serializable]
public class FalseCondition : ICardBuffCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source)
    {
        return false;
    }
}

[Serializable]
public class AllCondition : ICardBuffCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    public List<ICondition> Conditions;

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source)
    {
        return Conditions.All(condition => condition.Eval(gameWatcher, source));
    }
}

[Serializable]
public class AnyCondition : ICardBuffCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    public List<ICondition> Conditions;

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source)
    {
        return Conditions.Any(condition => condition.Eval(gameWatcher, source));
    }
}

[Serializable]
public class IsSelfTurnCondition : IPlayerBuffCondition, ICharacterBuffCondition
{
    public ITargetPlayerValue TargetPlayer;

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return gameWatcher.GameStatus.CurrentPlayer.Match(
            currnentPlayer => TargetPlayer.Eval(gameWatcher, trigger).Match(
                                targetPlayer => currnentPlayer == targetPlayer,
                                ()           => false),
            ()             => false
        );
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
 

public interface ICondition
{
    bool Eval(IGameplayStatusWatcher gameWatcher, IActionSource source);
}

public interface ICardBuffCondition : ICondition { }
public interface IPlayerBuffCondition : ICondition { }
public interface ICharacterBuffCondition : ICondition { }

[Serializable]
public class TrueCondition : ICardBuffCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    public bool Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return true;
    }
}

[Serializable]
public class FalseCondition : ICardBuffCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    public bool Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return false;
    }
}

[Serializable]
public class AllCondition : ICardBuffCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    public List<ICondition> Conditions;

    public bool Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return Conditions.All(condition => condition.Eval(gameWatcher, source));
    }
}

[Serializable]
public class AnyCondition : ICardBuffCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    public List<ICondition> Conditions;

    public bool Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return Conditions.Any(condition => condition.Eval(gameWatcher, source));
    }
}

[Serializable]
public class IsSelfTurnCondition : IPlayerBuffCondition, ICharacterBuffCondition
{
    public ITargetPlayerValue TargetPlayer;

    public bool Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return gameWatcher.GameStatus.CurrentPlayer.Match(
            currnentPlayer => TargetPlayer.Eval(gameWatcher, source).Match(
                                targetPlayer => currnentPlayer == targetPlayer,
                                ()           => false),
            ()             => false
        );
    }
}
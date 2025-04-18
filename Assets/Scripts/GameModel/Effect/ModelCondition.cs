using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
 

public interface ICondition
{
    bool Eval(IGameplayStatusWatcher gameWatcher, IActionSource source);
}

public interface ICardStatusCondition : ICondition { }
public interface IPlayerBuffCondition : ICondition { }
public interface ICharacterBuffCondition : ICondition { }

[Serializable]
public class TrueCondition : ICardStatusCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    public bool Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return true;
    }
}

[Serializable]
public class FalseCondition : ICardStatusCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    public bool Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return false;
    }
}

[Serializable]
public class AllCondition : ICardStatusCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    public List<ICondition> Conditions;

    public bool Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return Conditions.All(condition => condition.Eval(gameWatcher, source));
    }
}

[Serializable]
public class AnyCondition : ICardStatusCondition, IPlayerBuffCondition, ICharacterBuffCondition
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
            player => player == TargetPlayer.Eval(gameWatcher, source),
            () => false
        );
    }
}
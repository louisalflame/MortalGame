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
public class ConstCondition : ICardBuffCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    public bool Value;
    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source)
    {
        return Value;
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

[Serializable]
public class IntegerCondition : ICardBuffCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    public IIntegerValue Value;

    public IIntegerValueCondition Condition;

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        var value = Value.Eval(gameWatcher, trigger);
        return Condition.Eval(gameWatcher, trigger, value);
    }
}

[Serializable]
public class CardCondition : ICardBuffCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    public ITargetCardValue Card;
    public ICardValueCondition Condition;

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        var cardOpt = Card.Eval(gameWatcher, trigger);
        return Condition.Eval(gameWatcher, trigger, cardOpt);
    }
}

[Serializable]
public class PlayerCondition : ICardBuffCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    public ITargetPlayerValue Player;
    public IPlayerValueCondition Condition;

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        var playerOpt = Player.Eval(gameWatcher, trigger);
        return Condition.Eval(gameWatcher, trigger, playerOpt);
    }
}

[Serializable]
public class CharacterCondition : ICardBuffCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    public ITargetCharacterValue Character;
    public ICharacterValueCondition Condition;

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        var characterOpt = Character.Eval(gameWatcher, trigger);
        return Condition.Eval(gameWatcher, trigger, characterOpt);
    }
}

[Serializable]
public class PlayerBuffCondition : IPlayerBuffCondition
{
    public ITargetPlayerBuffValue PlayerBuff;
    public IPlayerBuffValueCondition Condition;

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        var playerBuffOpt = PlayerBuff.Eval(gameWatcher, trigger);
        return Condition.Eval(gameWatcher, trigger, playerBuffOpt);
    }
}
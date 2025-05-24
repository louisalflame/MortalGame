using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
 

public interface ICondition
{
    bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit);
}

public interface ICardBuffCondition : ICondition { }
public interface IPlayerBuffCondition : ICondition { }
public interface ICharacterBuffCondition : ICondition { }

[Serializable]
public class ConstCondition : ICardBuffCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    public bool Value;
    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit)
    {
        return Value;
    }
}

[Serializable]
public class AllCondition : ICardBuffCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    [ShowInInspector]    
    [HorizontalGroup("1")]
    public List<ICondition> Conditions = new();

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit)
    {
        return Conditions.All(condition => condition.Eval(gameWatcher, source, actionUnit));
    }
}

[Serializable]
public class AnyCondition : ICardBuffCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    [ShowInInspector]    
    [HorizontalGroup("1")]
    public List<ICondition> Conditions = new();

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit)
    {
        return Conditions.Any(condition => condition.Eval(gameWatcher, source, actionUnit));
    }
}

[Serializable]
public class IsSelfTurnCondition : IPlayerBuffCondition, ICharacterBuffCondition
{
    [HorizontalGroup("1")]
    public ITargetPlayerValue TargetPlayer;

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        return gameWatcher.GameStatus.CurrentPlayer.Match(
            currnentPlayer => TargetPlayer.Eval(gameWatcher, trigger, actionUnit).Match(
                                targetPlayer => currnentPlayer == targetPlayer,
                                ()           => false),
            ()             => false
        );
    }
}

[Serializable]
public class IntegerCondition : ICardBuffCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    [HorizontalGroup("1")]
    public IIntegerValue Value;

    [ShowInInspector]
    [HorizontalGroup("2")]
    public List<IIntegerValueCondition> Conditions = new ();

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        var value = Value.Eval(gameWatcher, trigger, actionUnit);
        return Conditions.All(c => c.Eval(gameWatcher, trigger, actionUnit, value));
    }
}

[Serializable]
public class CardCondition : ICardBuffCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    [HorizontalGroup("1")]
    public ITargetCardValue Card;
    
    [ShowInInspector]
    [HorizontalGroup("2")]
    public List<ICardValueCondition> Conditions = new ();

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        var cardOpt = Card.Eval(gameWatcher, trigger, actionUnit);
        return cardOpt.Match(
            card => Conditions.All(c => c.Eval(gameWatcher, trigger, actionUnit, card)),
            ()   => false
        );
    }
}
[SerializeField]
public class CardPlayCondition : ICardBuffCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    [ShowInInspector]
    [HorizontalGroup("1")]
    public List<ICardPlayValueCondition> Conditions = new ();

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {        
        return actionUnit switch
        {
            CardPlayIntentAction cardPlayIntent =>
                Conditions.All(c => c.Eval(gameWatcher, trigger, actionUnit, cardPlayIntent.CardPlay)),
            _ => false,
        };
    }
}

[Serializable]
public class PlayerCondition : ICardBuffCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    [HorizontalGroup("1")]
    public ITargetPlayerValue Player;

    [ShowInInspector]
    [HorizontalGroup("2")]
    public List<IPlayerValueCondition> Conditions = new ();

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        var playerOpt = Player.Eval(gameWatcher, trigger, actionUnit);
        return playerOpt.Match(
            player => Conditions.All(c => c.Eval(gameWatcher, trigger, actionUnit, player)),
            ()     => false
        );
    }
}

[Serializable]
public class CharacterCondition : ICardBuffCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    [HorizontalGroup("1")]
    public ITargetCharacterValue Character;
    
    [ShowInInspector]
    [HorizontalGroup("2")]
    public List<ICharacterValueCondition> Conditions = new ();

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        var characterOpt = Character.Eval(gameWatcher, trigger, actionUnit);
        return characterOpt.Match(
            character => Conditions.All(c => c.Eval(gameWatcher, trigger, character)),
            ()        => false
        );
    }
}

[Serializable]
public class PlayerBuffCondition : IPlayerBuffCondition
{
    [HorizontalGroup("1")]
    public ITargetPlayerBuffValue PlayerBuff;

    [ShowInInspector]
    [HorizontalGroup("2")]
    public List<IPlayerBuffValueCondition> Conditions = new ();
    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        var playerBuffOpt = PlayerBuff.Eval(gameWatcher, trigger, actionUnit);
        return playerBuffOpt.Match(
            playerBuff => Conditions.All(c => c.Eval(gameWatcher, trigger, actionUnit, playerBuff)),
            ()         => false
        );
    }
}
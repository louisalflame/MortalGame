using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using Sirenix.OdinInspector;
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
    [ShowInInspector]    
    [HorizontalGroup("1")]
    public List<ICondition> Conditions = new();

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source)
    {
        return Conditions.All(condition => condition.Eval(gameWatcher, source));
    }
}

[Serializable]
public class AnyCondition : ICardBuffCondition, IPlayerBuffCondition, ICharacterBuffCondition
{
    [ShowInInspector]    
    [HorizontalGroup("1")]
    public List<ICondition> Conditions = new();

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source)
    {
        return Conditions.Any(condition => condition.Eval(gameWatcher, source));
    }
}

[Serializable]
public class IsSelfTurnCondition : IPlayerBuffCondition, ICharacterBuffCondition
{
    [HorizontalGroup("1")]
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
    [HorizontalGroup("1")]
    public IIntegerValue Value;

    [ShowInInspector]
    [HorizontalGroup("2")]
    public List<IIntegerValueCondition> Conditions = new ();

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        var value = Value.Eval(gameWatcher, trigger);
        return Conditions.All(c => c.Eval(gameWatcher, trigger, value));
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

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        var cardOpt = Card.Eval(gameWatcher, trigger);
        return cardOpt.Match(
            card => Conditions.All(c => c.Eval(gameWatcher, trigger, card)),
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

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return trigger switch
        {
            CardPlayTrigger cardPlay =>
                Conditions.All(c => c.Eval(gameWatcher, trigger, cardPlay)),
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

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        var playerOpt = Player.Eval(gameWatcher, trigger);
        return playerOpt.Match(
            player => Conditions.All(c => c.Eval(gameWatcher, trigger, player)),
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

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        var characterOpt = Character.Eval(gameWatcher, trigger);
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
    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        var playerBuffOpt = PlayerBuff.Eval(gameWatcher, trigger);
        return playerBuffOpt.Match(
            playerBuff => Conditions.All(c => c.Eval(gameWatcher, trigger, playerBuff)),
            ()         => false
        );
    }
}
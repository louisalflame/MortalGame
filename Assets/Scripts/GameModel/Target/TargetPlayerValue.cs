using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

public interface ITargetPlayerValue
{
    Option<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger,
        IActionUnit actionUnit);
}

[Serializable]
public class NonePlayer : ITargetPlayerValue
{
    public Option<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger,
        IActionUnit actionUnit)
    {
        return Option.None<IPlayerEntity>();
    }
}
[Serializable]
public class CurrentPlayer : ITargetPlayerValue
{
    public Option<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger,
        IActionUnit actionUnit)
    {
        return gameWatcher.GameStatus.CurrentPlayer;
    }
}
[Serializable]
public class OppositePlayer : ITargetPlayerValue
{    
    [HorizontalGroup("1")]
    public ITargetPlayerValue Reference;

    public Option<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger,
        IActionUnit actionUnit)
    {
        var referenceOpt = Reference.Eval(gameWatcher, trigger, actionUnit);
        return
            referenceOpt.FlatMap(reference => 
                reference.Faction == Faction.Ally ? (gameWatcher.GameStatus.Enemy as IPlayerEntity).Some() :
                reference.Faction == Faction.Enemy ? (gameWatcher.GameStatus.Ally as IPlayerEntity).Some() :
                Option.None<IPlayerEntity>());
    }
}
[Serializable]
public class CardOwner : ITargetPlayerValue
{
    [HorizontalGroup("1")]
    public ITargetCardValue Card;

    public Option<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger,
        IActionUnit actionUnit)
    {
        var cardOpt = Card.Eval(gameWatcher, trigger, actionUnit);
        return cardOpt.FlatMap(card => card.Owner(gameWatcher));
    }
}
[Serializable]
public class PlayerBuffContentPlayer : ITargetPlayerValue
{
    public enum PlayerType
    {
        Owner,
        Caster,
    }

    [HorizontalGroup("1")]
    public ITargetPlayerBuffValue PlayerBuff;
    
    public PlayerType Type;

    public Option<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger,
        IActionUnit actionUnit)
    {
        var playerBuffOpt = PlayerBuff.Eval(gameWatcher, trigger, actionUnit);
        return playerBuffOpt.FlatMap(playerBuff => Type switch
        {
            PlayerType.Owner => playerBuff.Owner(gameWatcher),
            PlayerType.Caster => playerBuff.Caster,
            _ => Option.None<IPlayerEntity>()
        });
    }
}
[SerializeField]
public class CharacterOwner : ITargetPlayerValue
{
    [HorizontalGroup("1")]
    public ITargetCharacterValue Character;

    public Option<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger,
        IActionUnit actionUnit)
    {
        var characterOpt = Character.Eval(gameWatcher, trigger, actionUnit);
        return characterOpt.FlatMap(character => character.Owner(gameWatcher));
    }
}

public interface ITargetPlayerCollectionValue
{
    IReadOnlyCollection<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource trigger,
        IActionUnit actionUnit);
}
[Serializable]
public class NonePlayers : ITargetPlayerCollectionValue
{
    public IReadOnlyCollection<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource trigger,
        IActionUnit actionUnit)
    {
        return Array.Empty<IPlayerEntity>();
    }
}
[Serializable]
public class SinglePlayerCollection : ITargetPlayerCollectionValue
{
    [HorizontalGroup("1")]
    public ITargetPlayerValue Target;

    public IReadOnlyCollection<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource trigger,
        IActionUnit actionUnit)
    {
        return Target.Eval(gameWatcher, trigger, actionUnit).ToEnumerable().ToList();
    }
}
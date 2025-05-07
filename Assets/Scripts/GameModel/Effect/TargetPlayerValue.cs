using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using UniRx;
using UnityEngine;

public interface ITargetPlayerValue
{
    Option<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger);
}

[Serializable]
public class NonePlayer : ITargetPlayerValue
{
    public Option<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger)
    {
        return Option.None<IPlayerEntity>();
    }
}
[Serializable]
public class CurrentPlayer : ITargetPlayerValue
{
    public Option<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger)
    {
        return gameWatcher.GameStatus.CurrentPlayer;
    }
}
[Serializable]
public class OppositePlayer : ITargetPlayerValue
{    
    public ITargetPlayerValue Reference;

    public Option<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger)
    {
        var referenceOpt = Reference.Eval(gameWatcher, trigger);
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
    public ITargetCardValue Card;

    public Option<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger)
    {
        var cardOpt = Card.Eval(gameWatcher, trigger);
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

    public ITargetPlayerBuffValue PlayerBuff;
    public PlayerType Type;

    public Option<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger)
    {
        var playerBuffOpt = PlayerBuff.Eval(gameWatcher, trigger);
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
    public ITargetCharacterValue Character;

    public Option<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger)
    {
        var characterOpt = Character.Eval(gameWatcher, trigger);
        return characterOpt.FlatMap(character => character.Owner(gameWatcher));
    }
}

public interface ITargetPlayerCollectionValue
{
    IReadOnlyCollection<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource trigger);
}
[Serializable]
public class NonePlayers : ITargetPlayerCollectionValue
{
    public IReadOnlyCollection<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource trigger)
    {
        return Array.Empty<IPlayerEntity>();
    }
}
[Serializable]
public class SinglePlayerCollection : ITargetPlayerCollectionValue
{
    public ITargetPlayerValue Target;

    public IReadOnlyCollection<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource trigger)
    {
        return Target.Eval(gameWatcher, trigger).ToEnumerable().ToList();
    }
}
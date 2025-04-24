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
                reference.Faction == Faction.Ally ? (gameWatcher.GameStatus.Ally as IPlayerEntity).Some() :
                reference.Faction == Faction.Enemy ? (gameWatcher.GameStatus.Enemy as IPlayerEntity).Some() :
                Option.None<IPlayerEntity>());
    }
}
[Serializable]
public class CasterOfCard : ITargetPlayerValue
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
public class OwnerOfPlayerBuff : ITargetPlayerValue
{
    public ITargetPlayerBuffValue PlayerBuff;

    public Option<IPlayerEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger)
    {
        var playerBuffOpt = PlayerBuff.Eval(gameWatcher, trigger);
        return playerBuffOpt.FlatMap(playerBuff => playerBuff.Owner);
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
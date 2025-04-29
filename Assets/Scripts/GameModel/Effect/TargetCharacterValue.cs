using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using UniRx;
using UnityEngine;

public interface ITargetCharacterValue
{
    Option<ICharacterEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger);
}

[Serializable]
public class NoneCharacter : ITargetCharacterValue
{
    public Option<ICharacterEntity> Eval(IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger)
    {
        return Option.None<ICharacterEntity>();
    }
}
[Serializable]
public class MainCharacterOfPlayer : ITargetCharacterValue
{
    public ITargetPlayerValue Player;

    public Option<ICharacterEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger)
    {
        return Player.Eval(gameWatcher, trigger).Map(player => player.MainCharacter);
    }
}
[Serializable]
public class SelectedCharacter : ITargetCharacterValue
{
    public Option<ICharacterEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger)
    {
        return gameWatcher.GameContext.SelectedCharacter.SomeNotNull();
    }
}

public interface ITargetCharacterCollectionValue
{
    IReadOnlyCollection<ICharacterEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger);
}

[Serializable]
public class NoneCharacters : ITargetCharacterCollectionValue
{
    public IReadOnlyCollection<ICharacterEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger)
    {
        return  Array.Empty<ICharacterEntity>();
    }
}
[Serializable]
public class SingleCharacterCollection : ITargetCharacterCollectionValue
{
    public ITargetCharacterValue Target;

    public IReadOnlyCollection<ICharacterEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger)
    {
        return Target.Eval(gameWatcher, trigger).ToEnumerable().ToList();
    }
}
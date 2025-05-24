using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

public interface ITargetCharacterValue
{
    Option<ICharacterEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger,
        IActionUnit actionUnit);
}

[Serializable]
public class NoneCharacter : ITargetCharacterValue
{
    public Option<ICharacterEntity> Eval(IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger,
        IActionUnit actionUnit)
    {
        return Option.None<ICharacterEntity>();
    }
}
[Serializable]
public class MainCharacterOfPlayer : ITargetCharacterValue
{
    [HorizontalGroup("1")]
    public ITargetPlayerValue Player;

    public Option<ICharacterEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger,
        IActionUnit actionUnit)
    {
        return Player.Eval(gameWatcher, trigger, actionUnit).Map(player => player.MainCharacter);
    }
}
[Serializable]
public class SelectedCharacter : ITargetCharacterValue
{
    public Option<ICharacterEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger,
        IActionUnit actionUnit)
    {
        return gameWatcher.GameContext.SelectedCharacter.SomeNotNull();
    }
}

public interface ITargetCharacterCollectionValue
{
    IReadOnlyCollection<ICharacterEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger,
        IActionUnit actionUnit);
}

[Serializable]
public class NoneCharacters : ITargetCharacterCollectionValue
{
    public IReadOnlyCollection<ICharacterEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger,
        IActionUnit actionUnit)
    {
        return  Array.Empty<ICharacterEntity>();
    }
}
[Serializable]
public class SingleCharacterCollection : ITargetCharacterCollectionValue
{
    [HorizontalGroup("1")]
    public ITargetCharacterValue Target;

    public IReadOnlyCollection<ICharacterEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger,
        IActionUnit actionUnit)
    {
        return Target
            .Eval(gameWatcher, trigger, actionUnit)
            .ToEnumerable().ToList();
    }
}
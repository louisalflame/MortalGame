using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using UniRx;
using UnityEngine;

public static class TargetCharacterValueExtensions
{
    public static Option<ICharacterEntity> Eval(
        this ITargetCharacterValue targetCharacterValue, 
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger,
        IActionSource source)
    {
        return targetCharacterValue.Eval(gameWatcher, trigger.SomeNotNull(), source.SomeNotNull());
    }
    public static Option<ICharacterEntity> Eval(
        this ITargetCharacterValue targetCharacterValue, 
        IGameplayStatusWatcher gameWatcher,
        IActionSource source)
    {
        return targetCharacterValue.Eval(gameWatcher, Option.None<ITriggerSource>(), source.SomeNotNull());
    }

    public static IReadOnlyCollection<ICharacterEntity> Eval(
        this ITargetCharacterCollectionValue targetCharacterColleionValue, 
        IGameplayStatusWatcher gameWatcher, 
        ITriggerSource trigger,
        IActionSource source)
    {
        return targetCharacterColleionValue.Eval(gameWatcher, trigger.SomeNotNull(), source.SomeNotNull());
    }

    public static IReadOnlyCollection<ICharacterEntity> Eval(
        this ITargetCharacterCollectionValue targetCharacterColleionValue, 
        IGameplayStatusWatcher gameWatcher,
        IActionSource source)
    {
        return targetCharacterColleionValue.Eval(gameWatcher, Option.None<ITriggerSource>(), source.SomeNotNull());
    }
}

public interface ITargetCharacterValue
{
    Option<ICharacterEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        Option<ITriggerSource> trigger,
        Option<IActionSource> source);
}

[Serializable]
public class NoneCharacter : ITargetCharacterValue
{
    public Option<ICharacterEntity> Eval(IGameplayStatusWatcher gameWatcher, 
        Option<ITriggerSource> trigger,
        Option<IActionSource> source)
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
        Option<ITriggerSource> trigger,
        Option<IActionSource> source)
    {
        return Player.Eval(gameWatcher, trigger, source).Map(player => player.MainCharacter);
    }
}

public interface ITargetCharacterCollectionValue
{
    IReadOnlyCollection<ICharacterEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        Option<ITriggerSource> trigger,
        Option<IActionSource> source);
}

[Serializable]
public class NoneCharacters : ITargetCharacterCollectionValue
{
    public IReadOnlyCollection<ICharacterEntity> Eval(
        IGameplayStatusWatcher gameWatcher, 
        Option<ITriggerSource> trigger,
        Option<IActionSource> source)
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
        Option<ITriggerSource> trigger,
        Option<IActionSource> source)
    {
        return Target.Eval(gameWatcher, trigger, source).ToEnumerable().ToList();
    }
}
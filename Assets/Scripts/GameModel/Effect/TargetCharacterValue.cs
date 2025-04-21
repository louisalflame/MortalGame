using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using UniRx;
using UnityEngine;

public interface ITargetCharacterValue
{
    Option<ICharacterEntity> Eval(IGameplayStatusWatcher gameWatcher, IActionSource source);
}
[Serializable]
public class NoneCharacter : ITargetCharacterValue
{
    public Option<ICharacterEntity> Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return Option.None<ICharacterEntity>();
    }
}
[Serializable]
public class MainCharacterOfPlayer : ITargetCharacterValue
{
    public ITargetPlayerValue Player;

    public Option<ICharacterEntity> Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return Player.Eval(gameWatcher, source).Map(player => player.MainCharacter);
    }
}

public interface ITargetCharacterCollectionValue
{
    IReadOnlyCollection<ICharacterEntity> Eval(IGameplayStatusWatcher gameWatcher, IActionSource source);
}

[Serializable]
public class NoneCharacters : ITargetCharacterCollectionValue
{
    public IReadOnlyCollection<ICharacterEntity> Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return  Array.Empty<ICharacterEntity>();
    }
}
[Serializable]
public class SingleCharacterCollection : ITargetCharacterCollectionValue
{
    public ITargetCharacterValue Target;

    public IReadOnlyCollection<ICharacterEntity> Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return Target.Eval(gameWatcher, source).ToEnumerable().ToList();
    }
}
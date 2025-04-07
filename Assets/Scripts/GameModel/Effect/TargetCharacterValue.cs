using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public interface ITargetCharacterValue
{
    ICharacterEntity Eval(IGameplayStatusWatcher gameWatcher);
}
[Serializable]
public class NoneCharacter : ITargetCharacterValue
{
    public ICharacterEntity Eval(IGameplayStatusWatcher gameWatcher)
    {
        return CharacterEntity.DummyCharacter;
    }
}
[Serializable]
public class MainCharacterOfPlayer : ITargetCharacterValue
{
    public ITargetPlayerValue Player;

    public ICharacterEntity Eval(IGameplayStatusWatcher gameWatcher)
    {
        return Player.Eval(gameWatcher).MainCharacter;
    }
}

public interface ITargetCharacterCollectionValue
{
    IReadOnlyCollection<ICharacterEntity> Eval(IGameplayStatusWatcher gameWatcher);
}

[Serializable]
public class NoneCharacters : ITargetCharacterCollectionValue
{
    public IReadOnlyCollection<ICharacterEntity> Eval(IGameplayStatusWatcher gameWatcher)
    {
        return new ICharacterEntity[0];
    }
}
[Serializable]
public class SingleCharacterCollection : ITargetCharacterCollectionValue
{
    public ITargetCharacterValue Target;

    public IReadOnlyCollection<ICharacterEntity> Eval(IGameplayStatusWatcher gameWatcher)
    {
        return new [] { Target.Eval(gameWatcher) };
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public interface ITargetCharacterValue
{
    ICharacterEntity Eval(GameStatus gameStatus, GameContext context);
}
[Serializable]
public class NoneCharacter : ITargetCharacterValue
{
    public ICharacterEntity Eval(GameStatus gameStatus, GameContext context)
    {
        return CharacterEntity.DummyCharacter;
    }
}
[Serializable]
public class MainCharacterOfPlayer : ITargetCharacterValue
{
    public ITargetPlayerValue Player;

    public ICharacterEntity Eval(GameStatus gameStatus, GameContext context)
    {
        return Player.Eval(gameStatus, context).MainCharacter;
    }
}

public interface ITargetCharacterCollectionValue
{
    IReadOnlyCollection<ICharacterEntity> Eval(GameStatus gameStatus, GameContext context);
}
[Serializable]
public class NoneCharacters : ITargetCharacterCollectionValue
{
    public IReadOnlyCollection<ICharacterEntity> Eval(GameStatus gameStatus, GameContext context)
    {
        return new ICharacterEntity[0];
    }
}
[Serializable]
public class SingleCharacterCollection : ITargetCharacterCollectionValue
{
    public ITargetCharacterValue Target;

    public IReadOnlyCollection<ICharacterEntity> Eval(GameStatus gameStatus, GameContext context)
    {
        return new [] { Target.Eval(gameStatus, context) };
    }
}
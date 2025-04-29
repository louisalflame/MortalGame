using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using Unity.VisualScripting;
using UnityEngine;

public static class CharacterManagerExtensions
{
    public static Option<ICharacterEntity> GetAllyCharacter(this GameStatus gameStatus, Guid identity)
    {
        foreach (var character in gameStatus.Ally.Characters)
        {
            if (character.Identity == identity)
            {
                return character.SomeNotNull();
            }
        }

        return Option.None<ICharacterEntity>();
    }

    public static Option<ICharacterEntity> GetEnemyCharacter(this GameStatus gameStatus, Guid identity)
    {
        foreach (var character in gameStatus.Enemy.Characters)
        {
            if (character.Identity == identity)
            {
                return character.SomeNotNull();
            }
        }

        return Option.None<ICharacterEntity>();
    }

    public static Option<ICharacterEntity> GetCharacter(this GameStatus gameStatus, Guid identity)
    {
        foreach (var character in gameStatus.Ally.Characters)
        {
            if (character.Identity == identity)
            {
                return character.SomeNotNull();
            }
        }

        foreach (var character in gameStatus.Enemy.Characters)
        {
            if (character.Identity == identity)
            {
                return character.SomeNotNull();
            }
        }

        return Option.None<ICharacterEntity>();
    }
}
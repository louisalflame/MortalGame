using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using Unity.VisualScripting;
using UnityEngine;
public static class CharacterManagerExtensions
{
    public static Option<ICharacterEntity> GetCharacter(this IGameplayStatusWatcher gameplayWatcher, Guid identity)
    {
        foreach (var character in gameplayWatcher.GameStatus.Ally.Characters)
        {
            if (character.Identity == identity)
            {
                return character.SomeNotNull();
            }
        }

        foreach (var character in gameplayWatcher.GameStatus.Enemy.Characters)
        {
            if (character.Identity == identity)
            {
                return character.SomeNotNull();
            }
        }

        return Option.None<ICharacterEntity>();
    }
}
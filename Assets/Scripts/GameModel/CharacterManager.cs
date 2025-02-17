using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using UnityEngine;

public interface ICharacterManager
{
    IReadOnlyCollection<ICharacterEntity> Characters { get; }
    Option<ICharacterEntity> GetCharacter(Guid identity);
    void AddCharacters(IEnumerable<ICharacterEntity> characters);
    void RemoveCharacter(ICharacterEntity character);
}

public class CharacterManager : ICharacterManager
{
    private List<ICharacterEntity> _characters;
    public IReadOnlyCollection<ICharacterEntity> Characters => _characters;

    public CharacterManager()
    {
        _characters = new List<ICharacterEntity>();
    }

    public Option<ICharacterEntity> GetCharacter(Guid identity)
    {
        var character = Characters.FirstOrDefault(p => p.Identity == identity);
        return character.SomeNotNull();
    }

    public void AddCharacters(IEnumerable<ICharacterEntity> characters)
    {
        _characters.AddRange(characters);
    }

    public void RemoveCharacter(ICharacterEntity character)
    {
        _characters.Remove(character);
    }
}
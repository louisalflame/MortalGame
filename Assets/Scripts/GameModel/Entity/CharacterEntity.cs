using System;
using System.Linq;
using Optional;
using UnityEngine;

public interface ICharacterEntity
{
    public Guid Identity { get; }
    public string NameKey { get; }
    public IHealthManager HealthManager { get; }
    public ICharacterBuffManager BuffManager { get; }

    int CurrentHealth { get; }
    int MaxHealth { get; }
    int CurrentArmor { get; }
    bool IsDead { get; }
}

public record CharacterParameter
{
    public string NameKey;
    public int CurrentHealth;
    public int MaxHealth;
}

public class CharacterEntity : ICharacterEntity
{
    private readonly Guid _identity;
    private readonly string _nameKey;
    private readonly IHealthManager _healthManager;
    private readonly ICharacterBuffManager _buffManager;

    public Guid Identity => _identity;
    public string NameKey => _nameKey;
    public IHealthManager HealthManager => _healthManager;
    public ICharacterBuffManager BuffManager => _buffManager;
    public int CurrentHealth => HealthManager.Hp;
    public int MaxHealth => HealthManager.MaxHp;
    public int CurrentArmor => HealthManager.Dp;
    public bool IsDead => CurrentHealth <= 0;
    
    public bool IsDummy => this == DummyCharacter;
    public static ICharacterEntity DummyCharacter = new DummyCharacter();

    public CharacterEntity(
        string nameKey,
        int currentHealth,
        int maxHealth)
    {
        _identity = Guid.NewGuid();
        _nameKey = nameKey;
        _healthManager = new HealthManager(currentHealth, maxHealth);
        _buffManager = new CharacterBuffManager();
    }

    public static CharacterEntity Create(CharacterParameter characterParameter)
    {
        return new CharacterEntity(characterParameter.NameKey, characterParameter.CurrentHealth, characterParameter.MaxHealth);
    }
}

public class DummyCharacter : CharacterEntity
{
    public DummyCharacter() : base(string.Empty, 0, 0) { }
}

public static class CharacterEntityExtensions
{
    public static Option<IPlayerEntity> Owner(this ICharacterEntity character, IGameplayModel gameWatcher)
    {
        if (gameWatcher.GameStatus.Ally.Characters.Any(c => c.Identity == character.Identity))
            return (gameWatcher.GameStatus.Ally as IPlayerEntity).Some();
        if (gameWatcher.GameStatus.Enemy.Characters.Any(c => c.Identity == character.Identity))
            return (gameWatcher.GameStatus.Enemy as IPlayerEntity).Some();
        return Option.None<IPlayerEntity>();
    }
    public static Faction Faction(this ICharacterEntity character, IGameplayModel watcher)
    {
        return character.Owner(watcher).ValueOr(PlayerEntity.DummyPlayer).Faction;
    }
}
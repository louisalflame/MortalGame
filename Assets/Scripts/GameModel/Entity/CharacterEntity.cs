using System;
using UnityEngine;

public interface ICharacterEntity
{
    public Guid Identity { get; }
    public string NameKey { get; }
    public IHealthManager HealthManager { get; }
    public IPlayerEntity Owner { get; }

    int CurrentHealth { get; }
    int MaxHealth { get; }
    int CurrentArmor { get; }
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
    private readonly IPlayerEntity _owner;
    //TODO : CharacterBuffManager, instead of playerBuffManager

    public Guid Identity => _identity;
    public string NameKey => _nameKey;
    public IHealthManager HealthManager => _healthManager;
    public IPlayerEntity Owner => _owner;
    public int CurrentHealth => HealthManager.Hp;
    public int MaxHealth => HealthManager.MaxHp;
    public int CurrentArmor => HealthManager.Dp;
    
    public bool IsDummy => this == DummyCharacter;
    public static ICharacterEntity DummyCharacter = new DummyCharacter();

    public CharacterEntity(
        IPlayerEntity owner,
        string nameKey,
        int currentHealth,
        int maxHealth)
    {
        _identity = Guid.NewGuid();
        _owner = owner;
        _nameKey = nameKey;
        _healthManager = new HealthManager(currentHealth, maxHealth);
    }

    public static CharacterEntity Create(CharacterParameter characterParameter, IPlayerEntity owner)
    {
        return new CharacterEntity(owner, characterParameter.NameKey, characterParameter.CurrentHealth, characterParameter.MaxHealth);
    }
}

public class DummyCharacter : CharacterEntity
{
    public DummyCharacter() : base(PlayerEntity.DummyPlayer, string.Empty, 0, 0)
    { }
}

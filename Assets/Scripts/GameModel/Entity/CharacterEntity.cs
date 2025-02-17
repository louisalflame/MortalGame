using System;
using UnityEngine;

public interface ICharacterEntity
{
    public Guid Identity { get; }
    public string NameKey { get; }
    public IHealthManager HealthManager { get; }

    int CurrentHealth { get; }
    int MaxHealth { get; }
    int CurrentArmor { get; }
}

public class CharacterEntity : ICharacterEntity
{
    private readonly Guid _identity;
    private readonly string _nameKey;
    private readonly IHealthManager _healthManager;
    //TODO : CharacterBuffManager, instead of playerBuffManager

    public Guid Identity => _identity;
    public string NameKey => _nameKey;
    public IHealthManager HealthManager => _healthManager;
    public int CurrentHealth => HealthManager.Hp;
    public int MaxHealth => HealthManager.MaxHp;
    public int CurrentArmor => HealthManager.Dp;

    public CharacterEntity(
        string nameKey,
        int currentHealth,
        int maxHealth)
    {
        _identity = Guid.NewGuid();
        _nameKey = nameKey;
        _healthManager = new HealthManager(currentHealth, maxHealth);
    }
}



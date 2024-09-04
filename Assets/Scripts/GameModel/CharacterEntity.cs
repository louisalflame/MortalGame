using UnityEngine;

public interface ICharacterEntity
{
    int CurrentHealth { get; }
    int MaxHealth { get; }
    int CurrentArmor { get; }
    int CurrentEnergy { get; }
    int MaxEnergy { get; }
}

public class CharacterEntity : ICharacterEntity
{
    public IHealthManager HealthManager;
    public IEnergyManager EnergyManager;
    public IBuffManager BuffManager;

    public int CurrentHealth => HealthManager.Hp;
    public int MaxHealth => HealthManager.MaxHp;
    public int CurrentArmor => HealthManager.Dp;
    public int CurrentEnergy => EnergyManager.Energy;
    public int MaxEnergy => EnergyManager.MaxEnergy;

}



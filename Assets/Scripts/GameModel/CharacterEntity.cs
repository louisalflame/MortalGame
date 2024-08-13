using UnityEngine;

public class CharacterEntity
{
    public HealthManager HealthManager;
    public StatusManager StatusManager;
    public EnergyManager EnergyManager;

    public int CurrentHealth => HealthManager.Hp;
}



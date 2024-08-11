using UnityEngine;

public class CharacterEntity
{
    public HealthManager HealthManager;
    public StatusManager StatusManager;
    public PowerManager PowerManager;
    public EnergyManager EnergyManager;

    public int CurrentHealth => HealthManager.Hp;
    public int CurrentPower => PowerManager.Power;
}



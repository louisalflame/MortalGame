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

public class HealthManager
{
    public int Hp;
    public int MaxHp;
}

public class PowerManager
{
    public int Power;
}

public class EnergyManager
{
    public int Energy;
    public int MaxEnergy;
}

public class StatusManager
{

}
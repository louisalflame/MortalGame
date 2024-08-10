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

public class DispositionManager
{
    public enum Name 
    {
        Coward = -2,
        Cautious = -1,
        Moderate = 0,
        Brave = 1,
        Reckless = 2,
    }

    public int CurrentDisposition;
    public const int MAX_DISPOSITION = 10;

    public DispositionManager(int initialDisposition)
    {
        CurrentDisposition = initialDisposition; 
    }

    public string GetName() {
        if      ( CurrentDisposition <= 1 )
            return Name.Coward.ToString();
        else if ( CurrentDisposition <= 3 )
            return Name.Cautious.ToString();
        else if ( CurrentDisposition <= 6 )
            return Name.Moderate.ToString();
        else if ( CurrentDisposition <= 8 )
            return Name.Brave.ToString();
        else
            return Name.Reckless.ToString();
    }
}
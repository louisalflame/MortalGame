using UnityEngine;

public class CharacterEntity
{
    public HealthManager HealthManager;
    public StatusManager StatusManager;
    public PowerManager PowerManager;
    public EnergyManager EnergyManager;
    public DispositionManager DispositionManager;

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
    public int MaxDisposition;

    public DispositionManager(int currentDisposition, int maxDisposition)
    {
        CurrentDisposition = currentDisposition;
        MaxDisposition = maxDisposition;
    }

    public string GetName() {
        float ratio = (float)CurrentDisposition / MaxDisposition;
        if ( ratio < 0.2f )
            return Name.Coward.ToString();
        else if ( ratio < 0.4f )
            return Name.Cautious.ToString();
        else if ( ratio <= 0.6f )
            return Name.Moderate.ToString();
        else if ( ratio <= 0.8f )
            return Name.Brave.ToString();
        else
            return Name.Reckless.ToString();
    }
}
using UnityEngine;


public class EnergyManager
{
    public int Energy;
    public int MaxEnergy;

    public EnergyManager ConsumeEnergy(int amount, out int deltaEnergy)
    {
        var newEnergy = Mathf.Clamp(Energy - amount, 0, MaxEnergy);
        deltaEnergy = Energy - newEnergy;

        return new EnergyManager()
        {
            Energy = newEnergy,
            MaxEnergy = MaxEnergy,
        };
    }

    public EnergyManager GainEnergy(int amount, out int deltaEnergy)
    {
        var newEnergy = Mathf.Clamp(Energy + amount, 0, MaxEnergy);
        deltaEnergy = newEnergy - Energy;

        return new EnergyManager()
        {
            Energy = newEnergy,
            MaxEnergy = MaxEnergy,
        };
    }
}
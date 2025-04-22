using UnityEngine;

public class TakeDamageResult
{
    public DamageType Type;
    public int DamagePoint;
    public int DeltaHp;
    public int DeltaDp;
    public int OverHp;
}
public class GetHealResult
{
    public int HealPoint;
    public int DeltaHp;
    public int OverHp;
}
public class GetShieldResult
{
    public int ShieldPoint;
    public int DeltaDp;
    public int OverDp;
}

public class GetEnergyResult
{
    public EnergyGainType Type;
    public int EnergyPoint;
    public int DeltaEp;
    public int OverEp;
}
public class LoseEnergyResult
{
    public EnergyLoseType Type;
    public int EnergyPoint;
    public int DeltaEp;
    public int OverEp;
}
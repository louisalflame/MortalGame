using UnityEngine;

public enum Faction
{
    None = 0,
    Ally,
    Enemy
}

public enum DamageType
{
    Normal,
    Penetrate,
    Additional,
    Effective
}

public enum EnergyGainType
{
    GainEffect,
    Recover,
}
public enum EnergyLoseType
{
    LoseEffect,
    Consume,

}

public enum CardTiming
{
    None = 0,
    OnPlayCard,
}

public enum BuffTiming
{
    None = 0,
    OnTurnStart,
}
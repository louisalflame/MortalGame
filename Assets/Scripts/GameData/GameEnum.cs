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

public enum PropertyDuration
{
    None = 0,
    ThisTurn,
    ThisBattle,
    ThisGame
}
public enum CardProperty
{
    None = 0,
    OnPlayExtraTimes,
    ExtraPower,
    ExtraCost,
    OverWritePower,
    OverWriteCost,
    InitialPriority,
    Preserved,
    Sealed,
}
public enum BuffProperty
{
    None = 0,
    AttackIncrease,
    PenetrateAttackIncrease,
    AdditionalAttackIncrease,
    EffectiveAttackIncrease,
    DefenseIncrease,
    HealIncrease,
    EnergyIncrease,
    MaxHealthIncrease,
    MaxEnergyIncrease,
}
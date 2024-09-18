using UnityEngine;

public enum AlgorithmType
{
    None = 0,
    Add,
    Multiply,
    Overwrite
}
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


public enum BuffTiming
{
    None = 0,
    OnTurnStart,
    OnExecuteEnd,
    OnTurnEnd,
}

public enum BuffPropertyDuration
{
    None = 0,
    ThisTurn,
    ThisBattle,
    ThisGame
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
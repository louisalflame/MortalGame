using UnityEngine;

public enum ArithmeticType
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
public enum DamageStyle
{
    None = 1 >> 0,
    FullAttack = 1 >> 1,
    QuickAttack = 1 >> 2,
    ComboAttack = 1 >> 3,
    FollowAttack = 1 >> 4,
    CounterAttack = 1 >> 5,
}

public enum EnergyGainType
{
    None = 0,
    GainEffect,
    Recover,
}
public enum EnergyLoseType
{
    None = 0,
    LoseEffect,
    Consume,
}

public enum PlayerBuffPropertyDuration
{
    None = 0,
    ThisTurn,
    ThisBattle,
    ThisGame
}
public enum PlayerBuffProperty
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
public enum CharacterBuffProperty
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

public enum SelectType
{
    None = 0,
    Character,
    AllyCharacter,
    EnemyCharacter,
    Card,
    AllyCard,
    EnemyCard,
}

public enum TargetType
{
    None = 0,
    AllyCard,
    EnemyCard,
    AllyCharacter,
    EnemyCharacter,
}

public enum UpdateTiming
{
    None = 0,
    GameStart,
    TurnStart,
    TurnEnd,
    ExecuteStart,
    ExecuteEnd,
    DrawCard,
    PlayCard,
    TriggerBuffStart,
    TriggerBuffEnd,
}

public enum TriggerTiming
{
    None = 0,
    GameStart,
    TurnStart,
    TurnEnd,
    ExecuteStart,
    ExecuteEnd,
    DrawCard,
    PlayCardStart,
    PlayCardEnd,
    TriggerBuffEnd,
}


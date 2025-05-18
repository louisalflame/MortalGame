using Rayark.Mast;
using UnityEngine;

public interface ICharacterBuffPropertyEntity
{
    CharacterBuffProperty Property { get; }
    
    int Eval(IGameplayStatusWatcher gameWatcher);
}

public class AttackPropertyCharacterBuffEntity : ICharacterBuffPropertyEntity
{
    public CharacterBuffProperty Property => CharacterBuffProperty.Attack;

    public AttackPropertyCharacterBuffEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher) => 0;
}

public class PenetrateAttackPropertyCharacterBuffEntity : ICharacterBuffPropertyEntity
{
    public CharacterBuffProperty Property => CharacterBuffProperty.PenetrateAttackIncrease;

    public PenetrateAttackPropertyCharacterBuffEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher) => 0;
}   

public class AdditionalAttackPropertyCharacterBuffEntity : ICharacterBuffPropertyEntity
{
    public CharacterBuffProperty Property => CharacterBuffProperty.AdditionalAttack;

    public AdditionalAttackPropertyCharacterBuffEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher) => 0;
}

public class EffectiveAttackPropertyCharacterBuffEntity : ICharacterBuffPropertyEntity
{
    public CharacterBuffProperty Property => CharacterBuffProperty.EffectiveAttack;
    public EffectiveAttackPropertyCharacterBuffEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher) => 0;
}

public class DefensePropertyCharacterBuffEntity : ICharacterBuffPropertyEntity
{
    public CharacterBuffProperty Property => CharacterBuffProperty.Defense;

    public DefensePropertyCharacterBuffEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher) => 0;
}

public class HealPropertyCharacterBuffEntity : ICharacterBuffPropertyEntity
{
    public CharacterBuffProperty Property => CharacterBuffProperty.Heal;

    public HealPropertyCharacterBuffEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher) => 0;
}

public class EnergyPropertyCharacterBuffEntity : ICharacterBuffPropertyEntity
{
    public CharacterBuffProperty Property => CharacterBuffProperty.EnergyGain;

    public EnergyPropertyCharacterBuffEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher) => 0;
}

public class MaxHealthPropertyCharacterBuffEntity : ICharacterBuffPropertyEntity
{
    public CharacterBuffProperty Property => CharacterBuffProperty.MaxHealth;

    public MaxHealthPropertyCharacterBuffEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher) => 0;
}

public class MaxEnergyPropertyCharacterBuffEntity : ICharacterBuffPropertyEntity
{
    public CharacterBuffProperty Property => CharacterBuffProperty.MaxEnergy;

    public MaxEnergyPropertyCharacterBuffEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher) => 0;
}
using Rayark.Mast;
using UnityEngine;

public interface IPlayerBuffPropertyEntity
{
    PlayerBuffProperty Property { get; }
    
    int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource);
}

public class AttackPlayerBuffPropertyEntity : IPlayerBuffPropertyEntity
{
    public PlayerBuffProperty Property => PlayerBuffProperty.Attack;

    private readonly IIntegerValue _value;

    public AttackPlayerBuffPropertyEntity(IIntegerValue value) 
    { 
        _value = value;
    }

    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource)
    {
        return _value.Eval(gameWatcher, triggerSource,  SystemAction.Instance);
    }
}

public class PenetrateAttackPlayerBuffPropertyEntity : IPlayerBuffPropertyEntity
{
    public PlayerBuffProperty Property => PlayerBuffProperty.PenetrateAttack;

    public PenetrateAttackPlayerBuffPropertyEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource) => 0;
}   

public class AdditionalAttackPlayerBuffPropertyEntity : IPlayerBuffPropertyEntity
{
    public PlayerBuffProperty Property => PlayerBuffProperty.AdditionalAttack;

    public AdditionalAttackPlayerBuffPropertyEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource) => 0;
}

public class EffectiveAttackPlayerBuffPropertyEntity : IPlayerBuffPropertyEntity
{
    public PlayerBuffProperty Property => PlayerBuffProperty.EffectiveAttack;
    public EffectiveAttackPlayerBuffPropertyEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource) => 0;
}

public class DefensePlayerBuffPropertyEntity : IPlayerBuffPropertyEntity
{
    public PlayerBuffProperty Property => PlayerBuffProperty.Defense;

    public DefensePlayerBuffPropertyEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource) => 0;
}

public class HealPlayerBuffPropertyEntity : IPlayerBuffPropertyEntity
{
    public PlayerBuffProperty Property => PlayerBuffProperty.Heal;

    public HealPlayerBuffPropertyEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource) => 0;
}

public class EnergyPlayerBuffPropertyEntity : IPlayerBuffPropertyEntity
{
    public PlayerBuffProperty Property => PlayerBuffProperty.EnergyGain;

    public EnergyPlayerBuffPropertyEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource) => 0;
}

public class MaxHealthPlayerBuffPropertyEntity : IPlayerBuffPropertyEntity
{
    public PlayerBuffProperty Property => PlayerBuffProperty.MaxHealth;

    public MaxHealthPlayerBuffPropertyEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource) => 0;
}

public class MaxEnergyPlayerBuffPropertyEntity : IPlayerBuffPropertyEntity
{
    public PlayerBuffProperty Property => PlayerBuffProperty.MaxEnergy;

    public MaxEnergyPlayerBuffPropertyEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource) => 0;
}
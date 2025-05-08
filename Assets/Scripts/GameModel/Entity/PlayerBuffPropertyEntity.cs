using Rayark.Mast;
using UnityEngine;

public interface IPlayerBuffPropertyEntity
{
    PlayerBuffProperty Property { get; }
    
    int Eval(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource triggerSource);
}

public abstract class PlayerBuffPropertyEntity : IPlayerBuffPropertyEntity
{
    public abstract PlayerBuffProperty Property { get; }

    public virtual int Eval(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource triggerSource)
    {
        return 0;
    }
}

public class AttackPropertyPlayerBuffEntity : PlayerBuffPropertyEntity
{
    public override PlayerBuffProperty Property => PlayerBuffProperty.AttackIncrease;

    private readonly IIntegerValue _value;

    public AttackPropertyPlayerBuffEntity(IIntegerValue value) 
    { 
        _value = value;
    }

    public override int Eval(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource triggerSource)
    {
        return _value.Eval(gameWatcher, triggerSource);
    }
}

public class PenetrateAttackPropertyPlayerBuffEntity : PlayerBuffPropertyEntity
{
    public override PlayerBuffProperty Property => PlayerBuffProperty.PenetrateAttackIncrease;

    public PenetrateAttackPropertyPlayerBuffEntity() { }
}   

public class AdditionalAttackPropertyPlayerBuffEntity : PlayerBuffPropertyEntity
{
    public override PlayerBuffProperty Property => PlayerBuffProperty.AdditionalAttackIncrease;

    public AdditionalAttackPropertyPlayerBuffEntity()
    {
    }
}

public class EffectiveAttackPropertyPlayerBuffEntity : PlayerBuffPropertyEntity
{
    public override PlayerBuffProperty Property => PlayerBuffProperty.EffectiveAttackIncrease;
    public EffectiveAttackPropertyPlayerBuffEntity()
    {
    }
}

public class DefensePropertyPlayerBuffEntity : PlayerBuffPropertyEntity
{
    public override PlayerBuffProperty Property => PlayerBuffProperty.DefenseIncrease;

    public DefensePropertyPlayerBuffEntity()
    {
    }
}

public class HealPropertyPlayerBuffEntity : PlayerBuffPropertyEntity
{
    public override PlayerBuffProperty Property => PlayerBuffProperty.HealIncrease;

    public HealPropertyPlayerBuffEntity()
    {
    }
}

public class EnergyPropertyPlayerBuffEntity : PlayerBuffPropertyEntity
{
    public override PlayerBuffProperty Property => PlayerBuffProperty.EnergyIncrease;

    public EnergyPropertyPlayerBuffEntity()
    {
    }
}

public class MaxHealthPropertyPlayerBuffEntity : PlayerBuffPropertyEntity
{
    public override PlayerBuffProperty Property => PlayerBuffProperty.MaxHealthIncrease;

    public MaxHealthPropertyPlayerBuffEntity()
    {
    }
}

public class MaxEnergyPropertyPlayerBuffEntity : PlayerBuffPropertyEntity
{
    public override PlayerBuffProperty Property => PlayerBuffProperty.MaxEnergyIncrease;

    public MaxEnergyPropertyPlayerBuffEntity()
    {
    }
}
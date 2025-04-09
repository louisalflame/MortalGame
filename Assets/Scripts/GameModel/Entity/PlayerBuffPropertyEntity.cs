using Rayark.Mast;
using UnityEngine;

public interface IPlayerBuffPropertyEntity
{
    PlayerBuffProperty Property { get; }
    
    int Eval(IGameplayStatusWatcher gameWatcher);
    void Update(IGameplayStatusWatcher gameWatcher);
}

public abstract class PlayerBuffPropertyEntity : IPlayerBuffPropertyEntity
{
    public abstract PlayerBuffProperty Property { get; }

    public virtual int Eval(IGameplayStatusWatcher gameWatcher)
    {
        return 0;
    }

    public virtual void Update(IGameplayStatusWatcher gameWatcher)
    { }
}

public class AttackPropertyEntity : PlayerBuffPropertyEntity
{
    public override PlayerBuffProperty Property => PlayerBuffProperty.AttackIncrease;

    public AttackPropertyEntity() { }
}

public class PenetrateAttackPropertyEntity : PlayerBuffPropertyEntity
{
    public override PlayerBuffProperty Property => PlayerBuffProperty.PenetrateAttackIncrease;

    public PenetrateAttackPropertyEntity() { }
}   

public class AdditionalAttackPropertyEntity : PlayerBuffPropertyEntity
{
    public override PlayerBuffProperty Property => PlayerBuffProperty.AdditionalAttackIncrease;

    public AdditionalAttackPropertyEntity()
    {
    }
}

public class EffectiveAttackPropertyEntity : PlayerBuffPropertyEntity
{
    public override PlayerBuffProperty Property => PlayerBuffProperty.EffectiveAttackIncrease;
    public EffectiveAttackPropertyEntity()
    {
    }
}

public class DefensePropertyEntity : PlayerBuffPropertyEntity
{
    public override PlayerBuffProperty Property => PlayerBuffProperty.DefenseIncrease;

    public DefensePropertyEntity()
    {
    }
}

public class HealPropertyEntity : PlayerBuffPropertyEntity
{
    public override PlayerBuffProperty Property => PlayerBuffProperty.HealIncrease;

    public HealPropertyEntity()
    {
    }
}

public class EnergyPropertyEntity : PlayerBuffPropertyEntity
{
    public override PlayerBuffProperty Property => PlayerBuffProperty.EnergyIncrease;

    public EnergyPropertyEntity()
    {
    }
}

public class MaxHealthPropertyEntity : PlayerBuffPropertyEntity
{
    public override PlayerBuffProperty Property => PlayerBuffProperty.MaxHealthIncrease;

    public MaxHealthPropertyEntity()
    {
    }
}

public class MaxEnergyPropertyEntity : PlayerBuffPropertyEntity
{
    public override PlayerBuffProperty Property => PlayerBuffProperty.MaxEnergyIncrease;

    public MaxEnergyPropertyEntity()
    {
    }
}
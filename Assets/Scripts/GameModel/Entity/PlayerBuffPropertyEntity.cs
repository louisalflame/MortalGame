using Rayark.Mast;
using UnityEngine;

public interface IPlayerBuffPropertyEntity
{
    PlayerBuffProperty Property { get; }
    
    int Eval(IGameplayStatusWatcher gameWatcher);
    void UpdateTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing);
    void UpdateIntent(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IIntentAction intent);
    void UpdateResult(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IResultAction result);
}

public abstract class PlayerBuffPropertyEntity : IPlayerBuffPropertyEntity
{
    public abstract PlayerBuffProperty Property { get; }

    public virtual int Eval(IGameplayStatusWatcher gameWatcher)
    {
        return 0;
    }

    public virtual void UpdateTiming(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, UpdateTiming timing)
    { }
    public virtual void UpdateIntent(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IIntentAction intent)
    { }
    public virtual void UpdateResult(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IResultAction result)
    { }
}

public class AttackPropertyPlayerBuffEntity : PlayerBuffPropertyEntity
{
    public override PlayerBuffProperty Property => PlayerBuffProperty.AttackIncrease;

    public AttackPropertyPlayerBuffEntity() { }
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
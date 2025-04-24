using Rayark.Mast;
using UnityEngine;

public interface ICharacterBuffPropertyEntity
{
    CharacterBuffProperty Property { get; }
    
    int Eval(IGameplayStatusWatcher gameWatcher);
    void UpdateByTiming(IGameplayStatusWatcher gameWatcher, UpdateTiming timing);
    void UpdateIntent(IGameplayStatusWatcher gameWatcher, IIntentAction intent);
    void UpdateResult(IGameplayStatusWatcher gameWatcher, IResultAction result);
}

public abstract class CharacterBuffPropertyEntity : ICharacterBuffPropertyEntity
{
    public abstract CharacterBuffProperty Property { get; }

    public virtual int Eval(IGameplayStatusWatcher gameWatcher)
    {
        return 0;
    }

    public virtual void UpdateByTiming(IGameplayStatusWatcher gameWatcher, UpdateTiming timing) {}
    public virtual void UpdateIntent(IGameplayStatusWatcher gameWatcher, IIntentAction intent) {}
    public virtual void UpdateResult(IGameplayStatusWatcher gameWatcher, IResultAction result) {}
}

public class AttackPropertyCharacterBuffEntity : CharacterBuffPropertyEntity
{
    public override CharacterBuffProperty Property => CharacterBuffProperty.AttackIncrease;

    public AttackPropertyCharacterBuffEntity() { }
}

public class PenetrateAttackPropertyCharacterBuffEntity : CharacterBuffPropertyEntity
{
    public override CharacterBuffProperty Property => CharacterBuffProperty.PenetrateAttackIncrease;

    public PenetrateAttackPropertyCharacterBuffEntity() { }
}   

public class AdditionalAttackPropertyCharacterBuffEntity : CharacterBuffPropertyEntity
{
    public override CharacterBuffProperty Property => CharacterBuffProperty.AdditionalAttackIncrease;

    public AdditionalAttackPropertyCharacterBuffEntity()
    {
    }
}

public class EffectiveAttackPropertyCharacterBuffEntity : CharacterBuffPropertyEntity
{
    public override CharacterBuffProperty Property => CharacterBuffProperty.EffectiveAttackIncrease;
    public EffectiveAttackPropertyCharacterBuffEntity()
    {
    }
}

public class DefensePropertyCharacterBuffEntity : CharacterBuffPropertyEntity
{
    public override CharacterBuffProperty Property => CharacterBuffProperty.DefenseIncrease;

    public DefensePropertyCharacterBuffEntity()
    {
    }
}

public class HealPropertyCharacterBuffEntity : CharacterBuffPropertyEntity
{
    public override CharacterBuffProperty Property => CharacterBuffProperty.HealIncrease;

    public HealPropertyCharacterBuffEntity()
    {
    }
}

public class EnergyPropertyCharacterBuffEntity : CharacterBuffPropertyEntity
{
    public override CharacterBuffProperty Property => CharacterBuffProperty.EnergyIncrease;

    public EnergyPropertyCharacterBuffEntity()
    {
    }
}

public class MaxHealthPropertyCharacterBuffEntity : CharacterBuffPropertyEntity
{
    public override CharacterBuffProperty Property => CharacterBuffProperty.MaxHealthIncrease;

    public MaxHealthPropertyCharacterBuffEntity()
    {
    }
}

public class MaxEnergyPropertyCharacterBuffEntity : CharacterBuffPropertyEntity
{
    public override CharacterBuffProperty Property => CharacterBuffProperty.MaxEnergyIncrease;

    public MaxEnergyPropertyCharacterBuffEntity()
    {
    }
}
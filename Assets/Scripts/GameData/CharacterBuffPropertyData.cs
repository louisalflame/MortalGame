using System;
using Sirenix.OdinInspector;

public interface ICharacterBuffPropertyData
{
    ICharacterBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger);
}


[Serializable]
public class AttackPropertyCharacterBuffData : ICharacterBuffPropertyData
{
    public ICharacterBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new AttackPropertyCharacterBuffEntity();
    }
}

[Serializable]
public class PenetrateAttackPropertyCharacterBuffData : ICharacterBuffPropertyData
{
    public ICharacterBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new PenetrateAttackPropertyCharacterBuffEntity();
    }
}

[Serializable]
public class AdditionalAttackPropertyCharacterBuffData : ICharacterBuffPropertyData
{
    public ICharacterBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new AdditionalAttackPropertyCharacterBuffEntity();
    }
}

[Serializable]
public class EffectiveAttackPropertyCharacterBuffData : ICharacterBuffPropertyData
{
    public ICharacterBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new EffectiveAttackPropertyCharacterBuffEntity();
    }
}

[Serializable]
public class DefensePropertyCharacterBuffData : ICharacterBuffPropertyData
{
    public ICharacterBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new DefensePropertyCharacterBuffEntity();
    }
}

[Serializable]
public class HealPropertyCharacterBuffData : ICharacterBuffPropertyData
{
    public ICharacterBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new HealPropertyCharacterBuffEntity();
    }
}

[Serializable]
public class EnergyPropertyCharacterBuffData : ICharacterBuffPropertyData
{
    public ICharacterBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new EnergyPropertyCharacterBuffEntity();
    }
}

[Serializable]
public class MaxHealthPropertyCharacterBuffData : ICharacterBuffPropertyData
{
    public ICharacterBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new MaxHealthPropertyCharacterBuffEntity();
    }
}

[Serializable]
public class MaxEnergyPropertyCharacterBuffData : ICharacterBuffPropertyData
{
    public ICharacterBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new MaxEnergyPropertyCharacterBuffEntity();
    }
}
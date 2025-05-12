using System;
using Sirenix.OdinInspector;

public interface IPlayerBuffPropertyData
{
    IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger);
}


[Serializable]
public class AttackPropertyPlayerBuffData : IPlayerBuffPropertyData
{
    public IIntegerValue Value;

    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new AttackPropertyPlayerBuffEntity(Value);
    }
}

[Serializable]
public class PenetrateAttackPropertyPlayerBuffData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new PenetrateAttackPropertyPlayerBuffEntity();
    }
}

[Serializable]
public class AdditionalAttackPropertyPlayerBuffData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new AdditionalAttackPropertyPlayerBuffEntity();
    }
}

[Serializable]
public class EffectiveAttackPropertyPlayerBuffData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new EffectiveAttackPropertyPlayerBuffEntity();
    }
}

[Serializable]
public class DefensePropertyPlayerBuffData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new DefensePropertyPlayerBuffEntity();
    }
}

[Serializable]
public class HealPropertyPlayerBuffData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new HealPropertyPlayerBuffEntity();
    }
}

[Serializable]
public class EnergyPropertyPlayerBuffData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new EnergyPropertyPlayerBuffEntity();
    }
}

[Serializable]
public class MaxHealthPropertyPlayerBuffData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new MaxHealthPropertyPlayerBuffEntity();
    }
}

[Serializable]
public class MaxEnergyPropertyPlayerBuffData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new MaxEnergyPropertyPlayerBuffEntity();
    }
}
using System;
using Sirenix.OdinInspector;

public interface IPlayerBuffPropertyData
{
    IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger);
}


[Serializable]
public class AttackPlayerBuffPropertyData : IPlayerBuffPropertyData
{
    public IIntegerValue Value;

    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new AttackPlayerBuffPropertyEntity(Value);
    }
}

[Serializable]
public class PenetrateAttackPlayerBuffPropertyData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new PenetrateAttackPlayerBuffPropertyEntity();
    }
}

[Serializable]
public class AdditionalAttackPlayerBuffPropertyData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new AdditionalAttackPlayerBuffPropertyEntity();
    }
}

[Serializable]
public class EffectiveAttackPlayerBuffPropertyData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new EffectiveAttackPlayerBuffPropertyEntity();
    }
}

[Serializable]
public class DefensePlayerBuffPropertyData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new DefensePlayerBuffPropertyEntity();
    }
}

[Serializable]
public class HealPlayerBuffPropertyData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new HealPlayerBuffPropertyEntity();
    }
}

[Serializable]
public class EnergyPlayerBuffPropertyData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new EnergyPlayerBuffPropertyEntity();
    }
}

[Serializable]
public class MaxHealthPlayerBuffPropertyData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new MaxHealthPlayerBuffPropertyEntity();
    }
}

[Serializable]
public class MaxEnergyPlayerBuffPropertyData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new MaxEnergyPlayerBuffPropertyEntity();
    }
}
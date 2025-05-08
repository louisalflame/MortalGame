using System;
using Sirenix.OdinInspector;

public interface IPlayerBuffPropertyData
{
    IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher);
}


[Serializable]
public class AttackPropertyPlayerBuffData : IPlayerBuffPropertyData
{
    public IIntegerValue Value;

    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new AttackPropertyPlayerBuffEntity(Value);
    }
}

[Serializable]
public class PenetrateAttackPropertyPlayerBuffData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new PenetrateAttackPropertyPlayerBuffEntity();
    }
}

[Serializable]
public class AdditionalAttackPropertyPlayerBuffData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new AdditionalAttackPropertyPlayerBuffEntity();
    }
}

[Serializable]
public class EffectiveAttackPropertyPlayerBuffData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new EffectiveAttackPropertyPlayerBuffEntity();
    }
}

[Serializable]
public class DefensePropertyPlayerBuffData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new DefensePropertyPlayerBuffEntity();
    }
}

[Serializable]
public class HealPropertyPlayerBuffData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new HealPropertyPlayerBuffEntity();
    }
}

[Serializable]
public class EnergyPropertyPlayerBuffData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new EnergyPropertyPlayerBuffEntity();
    }
}

[Serializable]
public class MaxHealthPropertyPlayerBuffData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new MaxHealthPropertyPlayerBuffEntity();
    }
}

[Serializable]
public class MaxEnergyPropertyPlayerBuffData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new MaxEnergyPropertyPlayerBuffEntity();
    }
}
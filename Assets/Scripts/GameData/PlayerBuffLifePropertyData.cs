using System;
using Sirenix.OdinInspector;

public interface IPlayerBuffPropertyData
{
    IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher);
}


[Serializable]
public class AttackPropertyData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new AttackPropertyEntity();
    }
}

[Serializable]
public class PenetrateAttackPropertyData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new PenetrateAttackPropertyEntity();
    }
}

[Serializable]
public class AdditionalAttackPropertyData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new AdditionalAttackPropertyEntity();
    }
}

[Serializable]
public class EffectiveAttackPropertyData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new EffectiveAttackPropertyEntity();
    }
}

[Serializable]
public class DefensePropertyData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new DefensePropertyEntity();
    }
}

[Serializable]
public class HealPropertyData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new HealPropertyEntity();
    }
}

[Serializable]
public class EnergyPropertyData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new EnergyPropertyEntity();
    }
}

[Serializable]
public class MaxHealthPropertyData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new MaxHealthPropertyEntity();
    }
}

[Serializable]
public class MaxEnergyPropertyData : IPlayerBuffPropertyData
{
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new MaxEnergyPropertyEntity();
    }
}
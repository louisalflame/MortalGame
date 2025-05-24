using System;
using Sirenix.OdinInspector;

public interface IPlayerBuffPropertyData
{
    IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger);
}


[Serializable]
public class EffectAttributePlayerBuffPropertyData : IPlayerBuffPropertyData
{
    public EffectAttributeType Type;
    public IIntegerValue Value;

    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new EffectAttributePlayerBuffPropertyEntity(Type, Value);
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
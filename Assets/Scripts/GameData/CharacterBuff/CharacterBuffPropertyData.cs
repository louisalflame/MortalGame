using System;
using Sirenix.OdinInspector;

public interface ICharacterBuffPropertyData
{
    ICharacterBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger);
}


[Serializable]
public class EffectAttributePropertyCharacterBuffData : ICharacterBuffPropertyData
{
    public EffectAttributeType Type;
    public IIntegerValue Value;

    public ICharacterBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new EffectAttributePropertyCharacterBuffEntity();
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
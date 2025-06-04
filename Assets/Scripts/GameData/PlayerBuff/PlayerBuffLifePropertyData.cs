using System;
using Sirenix.OdinInspector;

public interface IPlayerBuffPropertyData
{
    IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger);
}

[Serializable]
public class AllCardPowerPlayerBuffPropertyData : IPlayerBuffPropertyData
{
    public IIntegerValue Value;

    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new AllCardPowerPlayerBuffPropertyEntity(Value);
    }
}
[Serializable]
public class AllCardCostPlayerBuffPropertyData : IPlayerBuffPropertyData
{
    public IIntegerValue Value;

    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new AllCardCostPlayerBuffPropertyEntity(Value);
    }
}

[Serializable]
public class NormalDamageAdditionPlayerBuffPropertyData : IPlayerBuffPropertyData
{
    public IIntegerValue Value;

    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new NormalDamageAdditionPlayerBuffPropertyEntity(Value);
    }
}
[Serializable]
public class NormalDamageRatioPlayerBuffPropertyData : IPlayerBuffPropertyData
{
    public float Value;

    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new NormalDamageRatioPlayerBuffPropertyEntity(Value);
    }
}

[Serializable]
public class MaxHealthPlayerBuffPropertyData : IPlayerBuffPropertyData
{
    public IIntegerValue Value;
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new MaxHealthPlayerBuffPropertyEntity(Value);
    }
}

[Serializable]
public class MaxEnergyPlayerBuffPropertyData : IPlayerBuffPropertyData
{
    public IIntegerValue Value;
    public IPlayerBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new MaxEnergyPlayerBuffPropertyEntity(Value);
    }
}
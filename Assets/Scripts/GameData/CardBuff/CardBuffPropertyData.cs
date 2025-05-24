using System;
using UnityEngine;

public interface ICardBuffPropertyData
{
    ICardBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger);
}

[Serializable]
public class SealedCardBuffPropertyData : ICardBuffPropertyData
{
    public ICardBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new SealedCardBuffPropertyEntity();
    }
}

[Serializable]
public class PowerCardBuffPropertyData : ICardBuffPropertyData
{
    public IIntegerValue Value;

    public ICardBuffPropertyEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger)
    {
        return new PowerCardBuffPropertyEntity(Value);
    }
}
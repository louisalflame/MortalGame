using System;
using UnityEngine;

public interface ICardBuffLifeTimeData
{
    ICardBuffLifeTimeEntity CreateEntity(IGameplayStatusWatcher watcher, ITriggerSource trigger);
}

[Serializable]
public class AlwaysLifeTimeCardBuffData : ICardBuffLifeTimeData
{
    public ICardBuffLifeTimeEntity CreateEntity(IGameplayStatusWatcher watcher, ITriggerSource trigger)
    {
        return new AlwaysLifeTimeCardBuffEntity();
    }
}

[Serializable]
public class TurnLifeTimeCardBuffData : ICardBuffLifeTimeData
{
    public IIntegerValue Turn;

    public ICardBuffLifeTimeEntity CreateEntity(IGameplayStatusWatcher watcher, ITriggerSource trigger)
    {
        return new TurnLifeTimeCardBuffEntity(Turn.Eval(watcher, trigger));
    }
}
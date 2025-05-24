using System;
using UnityEngine;

public interface ICardBuffLifeTimeData
{
    ICardBuffLifeTimeEntity CreateEntity(IGameplayStatusWatcher watcher, ITriggerSource trigger, IActionUnit actionUnit);
}

[Serializable]
public class AlwaysLifeTimeCardBuffData : ICardBuffLifeTimeData
{
    public ICardBuffLifeTimeEntity CreateEntity(IGameplayStatusWatcher watcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        return new AlwaysLifeTimeCardBuffEntity();
    }
}

[Serializable]
public class TurnLifeTimeCardBuffData : ICardBuffLifeTimeData
{
    public IIntegerValue Turn;

    public ICardBuffLifeTimeEntity CreateEntity(IGameplayStatusWatcher watcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        return new TurnLifeTimeCardBuffEntity(Turn.Eval(watcher, trigger, actionUnit));
    }
}

[Serializable]
public class HandCardLifeTimeCardBuffData : ICardBuffLifeTimeData
{
    public ICardBuffLifeTimeEntity CreateEntity(IGameplayStatusWatcher watcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        return new HandCardLifeTimeCardBuffEntity();
    }
}
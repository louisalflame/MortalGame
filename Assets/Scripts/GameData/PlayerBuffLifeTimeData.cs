using System;

public interface IPlayerBuffLifeTimeData
{
    IPlayerBuffLifeTimeEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit);
}

[Serializable]
public class AlwaysLifeTimePlayerBuffData : IPlayerBuffLifeTimeData
{
    public IPlayerBuffLifeTimeEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        return new AlwaysLifeTimePlayerBuffEntity();
    }
}

[Serializable]
public class PlayerBuffTurnLifeTimeData : IPlayerBuffLifeTimeData
{
    public IIntegerValue Turn;

    public IPlayerBuffLifeTimeEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        return new TurnLifeTimePlayerBuffEntity(Turn.Eval(gameWatcher, trigger, actionUnit));
    }
}
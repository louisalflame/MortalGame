using System;

public interface ICharacterBuffLifeTimeData
{
    ICharacterBuffLifeTimeEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit);
}

[Serializable]
public class AlwaysLifeTimeCharacterBuffData : ICharacterBuffLifeTimeData
{
    public ICharacterBuffLifeTimeEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        return new AlwaysLifeTimeCharacterBuffEntity();
    }
}

[Serializable]
public class TurnLifeTimeCharacterBuffData : ICharacterBuffLifeTimeData
{
    public int Turn;

    public ICharacterBuffLifeTimeEntity CreateEntity(IGameplayStatusWatcher gameWatcher, ITriggerSource trigger, IActionUnit actionUnit)
    {
        return new TurnLifeTimeCharacterBuffEntity(Turn);
    }
}
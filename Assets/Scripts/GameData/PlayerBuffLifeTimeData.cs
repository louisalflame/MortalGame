using System;

public interface IPlayerBuffLifeTimeData
{
    IPlayerBuffLifeTimeEntity CreateEntity(IGameplayStatusWatcher gameWatcher);
}

[Serializable]
public class AlwaysLifeTimePlayerBuffData : IPlayerBuffLifeTimeData
{
    public IPlayerBuffLifeTimeEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new AlwaysLifeTimePlayerBuffEntity();
    }
}

[Serializable]
public class PlayerBuffTurnLifeTimeData : IPlayerBuffLifeTimeData
{
    public int Turn;

    public IPlayerBuffLifeTimeEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new TurnLifeTimePlayerBuffEntity(Turn);
    }
}
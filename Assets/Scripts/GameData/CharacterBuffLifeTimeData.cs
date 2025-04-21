using System;

public interface ICharacterBuffLifeTimeData
{
    ICharacterBuffLifeTimeEntity CreateEntity(IGameplayStatusWatcher gameWatcher);
}

[Serializable]
public class AlwaysLifeTimeCharacterBuffData : ICharacterBuffLifeTimeData
{
    public ICharacterBuffLifeTimeEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new AlwaysLifeTimeCharacterBuffEntity();
    }
}

[Serializable]
public class TurnLifeTimeCharacterBuffData : ICharacterBuffLifeTimeData
{
    public int Turn;

    public ICharacterBuffLifeTimeEntity CreateEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new TurnLifeTimeCharacterBuffEntity(Turn);
    }
}
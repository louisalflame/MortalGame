using System;
using UnityEngine;

public interface ICardStatusLifeTimeData
{
    ICardStatusLifeTimeEntity CreateEntity();
}

[Serializable]
public class AlwaysLifeTimeCardStatusData : ICardStatusLifeTimeData
{
    public ICardStatusLifeTimeEntity CreateEntity()
    {
        return new AlwaysLifeTimeCardStatusEntity();
    }
}

[Serializable]
public class TurnLifeTimeCardStatusData : ICardStatusLifeTimeData
{
    public int Turn;

    public ICardStatusLifeTimeEntity CreateEntity()
    {
        return new TurnLifeTimeCardStatusEntity(Turn);
    }
}
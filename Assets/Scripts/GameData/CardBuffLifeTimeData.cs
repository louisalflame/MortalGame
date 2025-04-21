using System;
using UnityEngine;

public interface ICardBuffLifeTimeData
{
    ICardBuffLifeTimeEntity CreateEntity();
}

[Serializable]
public class AlwaysLifeTimeCardBuffData : ICardBuffLifeTimeData
{
    public ICardBuffLifeTimeEntity CreateEntity()
    {
        return new AlwaysLifeTimeCardBuffEntity();
    }
}

[Serializable]
public class TurnLifeTimeCardBuffData : ICardBuffLifeTimeData
{
    public int Turn;

    public ICardBuffLifeTimeEntity CreateEntity()
    {
        return new TurnLifeTimeCardBuffEntity(Turn);
    }
}
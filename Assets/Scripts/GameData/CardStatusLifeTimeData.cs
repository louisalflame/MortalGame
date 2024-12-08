using UnityEngine;

public interface ICardStatusLifeTimeData
{
    ICardStatusLifeTimeEntity CreateEntity();
}

public class CardStatusAlwaysLifeTimeData : ICardStatusLifeTimeData
{
    public ICardStatusLifeTimeEntity CreateEntity()
    {
        return new CardStatusAlwaysLifeTimeEntity();
    }
}

public class CardStatusTurnLifeTimeData : ICardStatusLifeTimeData
{
    public int Turn;

    public ICardStatusLifeTimeEntity CreateEntity()
    {
        return new CardStatusTurnLifeTimeEntity(Turn);
    }
}
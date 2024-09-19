using UnityEngine;

public interface ICardPropertyLifetimeData
{
    ICardPropertyLifetimeEntity CreateEntity();
}

public class CardPropertyAlwaysData : ICardPropertyLifetimeData
{
    public ICardPropertyLifetimeEntity CreateEntity()
    {
        return new CardPropertyAlwaysEntity();
    }
}

public class CardPropertyTurnCountData : ICardPropertyLifetimeData
{
    public int Turns;

    public ICardPropertyLifetimeEntity CreateEntity()
    {
        return new CardPropertyTurnCountEntity
        {
            Turns = Turns
        };
    }
}


public interface ICardPropertyLifetimeEntity
{
    bool NextTurn();
}
public class CardPropertyAlwaysEntity : ICardPropertyLifetimeEntity
{
    public bool NextTurn()
    {
        return true;
    }
}

public class CardPropertyTurnCountEntity : ICardPropertyLifetimeEntity
{
    public int Turns;

    public bool NextTurn()
    {
        Turns--;
        return Turns > 0;
    }
}
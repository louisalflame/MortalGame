using UnityEngine;

public interface ICardPropertyLifetime
{
}

public class Always : ICardPropertyLifetime
{
}

public class TurnCount : ICardPropertyLifetime
{
    public int Turns;
}
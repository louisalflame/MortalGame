using System.Collections.Generic;
using UnityEngine;

public interface IGameEvent
{

}

public class DrawCardEvent : IGameEvent
{
    public IReadOnlyCollection<CardInfo> CardInfos;
}

public class RoundStartEvent : IGameEvent
{
    public int Round;
    public PlayerEntity Player;
    public PlayerEntity Enemy;
}

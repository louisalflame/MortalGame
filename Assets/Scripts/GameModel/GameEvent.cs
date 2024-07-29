using System.Collections.Generic;
using UnityEngine;

public interface IGameEvent
{

}

public class DrawCardEvent : IGameEvent
{
    public IReadOnlyCollection<CardInfo> CardInfos;
}

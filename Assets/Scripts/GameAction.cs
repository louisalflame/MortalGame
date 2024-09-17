using System;
using System.Collections.Generic;
using UnityEngine;

public interface IGameAction
{

}

public class UseCardAction : IGameAction
{
    public Guid CardIndentity;
}

public class TurnSubmitAction : IGameAction
{
    public Faction Faction;
}

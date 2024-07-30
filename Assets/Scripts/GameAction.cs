using System.Collections.Generic;
using UnityEngine;

public interface IGameAction
{

}

public class UseCardAction : IGameAction
{
    public int CardIndentity;
}

public class TurnSubmitAction : IGameAction
{
    public bool IsNPC;
}

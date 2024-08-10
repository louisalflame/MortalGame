using System.Collections.Generic;
using UnityEngine;

public interface IGameAction
{

}

public class UseCardAction : IGameAction
{
    public string CardIndentity;
}

public class TurnSubmitAction : IGameAction
{
    public Faction Faction;
}

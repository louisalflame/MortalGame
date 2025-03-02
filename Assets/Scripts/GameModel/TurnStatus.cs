using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public interface ITurnStatus
{
    IPlayerTurnStatus AllyStatus { get; }
    IPlayerTurnStatus EnemyStatus { get; }
}

public interface IPlayerTurnStatus
{
    int ComboCount { get; }
}

public class TurnStatus
{
    public IPlayerTurnStatus AllyStatus { get; private set; }
    public IPlayerTurnStatus EnemyStatus { get; private set; }

    public TurnStatus()
    {
        AllyStatus = new AllyTurnStatus();
        EnemyStatus = new EnemyTurnStatus();
    }
}

public abstract class PlayerTurnStatus : IPlayerTurnStatus
{
    protected int _comboCount = 0;

    public int ComboCount => _comboCount;
}

public class AllyTurnStatus : PlayerTurnStatus
{

}

public class EnemyTurnStatus : PlayerTurnStatus
{

}

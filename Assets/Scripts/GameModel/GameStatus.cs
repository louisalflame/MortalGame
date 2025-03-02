using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum GameState
{
    None = 0,
    GameStart,
    TurnStart,
    DrawCard,
    EnemyPrepare,
    PlayerPrepare,
    PlayerExecute,
    Enemy_Execute,
    TurnEnd,
    GameEnd,
}

public class GameStatus
{
    public int TurnCount { get; private set; }
    public GameState State { get; private set; }
    public AllyEntity Ally { get; private set; }
    public EnemyEntity Enemy { get; private set; }
    public ICharacterManager CharacterManager { get; private set; }
    public TurnStatus TurnStatus { get; private set; }

    public GameStatus(
        int turnCount,
        GameState state,
        AllyEntity player,
        EnemyEntity enemy) 
    {
        TurnCount = turnCount;
        State = state;
        Ally = player;
        Enemy = enemy;
        CharacterManager = new CharacterManager();
        CharacterManager.AddCharacters(player.Characters);
        CharacterManager.AddCharacters(enemy.Characters);
        TurnStatus = new TurnStatus();
    } 

    public void SetState(GameState state)
    {
        State = state;
    }

    public void SetNewTurn()
    {
        TurnCount++;
        TurnStatus = new TurnStatus();
    }
}

public class GameResult
{
    public bool IsWin { get; }

    public GameResult(bool isWin)
    {
        IsWin = isWin;
    }
}
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum GameState
{
    None = 0,
    TurnStart,
    DrawCard,
    EnemyPrepare,
    PlayerExecute,
    Enemy_Execute,
    TurnEnd,
    GameEnd,
}

public class GameStatus
{
    public int Round { get; private set; }
    public GameState State { get; private set; }
    public PlayerEntity Ally { get; private set; }
    public PlayerEntity Enemy { get; private set; }

    public GameStatus(
        int round,
        GameState state,
        PlayerEntity player,
        PlayerEntity enemy) 
    {
        Round = round;
        State = state;
        Ally = player;
        Enemy = enemy;
    }

    public GameStatus With(
        int round = -1,
        GameState state = GameState.None,
        PlayerEntity player = null,
        PlayerEntity enemy = null)
    {
        return new GameStatus(
            round: round == -1 ? Round : round,
            state: state == GameState.None ? State : state,
            player: player ?? Ally,
            enemy: enemy ?? Enemy
        );
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
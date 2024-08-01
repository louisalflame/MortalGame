using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum GameState
{
    None = 0,
    Player_Prepare,
    Player_DrawCard,
    Player_Execute,
    Player_Finalize,
    Enemy_Prepare,
    Enemy_DrawCard,
    Enemy_Execute,
    Enemy_Finalize,
    GameEnd,
}

public class GameStatus
{
    public int Round { get; private set; }
    public GameState State { get; private set; }
    public PlayerEntity Player { get; private set; }
    public PlayerEntity Enemy { get; private set; }

    public GameStatus(
        int round,
        GameState state,
        PlayerEntity player,
        PlayerEntity enemy) 
    {
        Round = round;
        State = state;
        Player = player;
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
            player: player ?? Player,
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
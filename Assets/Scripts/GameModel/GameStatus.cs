using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using Rayark.Mast;
using UniRx;

public enum GameState
{
    None = 0,
    GameStart,
    TurnStart,
    DrawCard,
    EnemyPrepare,
    PlayerPrepare,
    PlayerExecute,
    EnemyExecute,
    TurnEnd,
    GameEnd,
}

public class GameStatus
{
    public int TurnCount { get; private set; }
    public AllyEntity Ally { get; private set; }
    public EnemyEntity Enemy { get; private set; }
    public Option<IPlayerEntity> CurrentPlayer { get; private set; } = Option.None<IPlayerEntity>();
    public Option<IPlayerEntity> OppositePlayer => CurrentPlayer
        .Map(current => current.Faction == Faction.Ally ? (IPlayerEntity)Enemy : (IPlayerEntity)Ally);
    public UnityEngine.Random.State RandomState => UnityEngine.Random.state;

    public GameStatus(
        int turnCount,
        AllyEntity player,
        EnemyEntity enemy,
        int randomSeed) 
    {
        TurnCount = turnCount;
        Ally = player;
        Enemy = enemy;
        UnityEngine.Random.InitState(randomSeed);
    }

    public IDisposable SetCurrentPlayer(IPlayerEntity player)
    {
        CurrentPlayer = player.Some();

        return Disposable.Create(() => CurrentPlayer = Option.None<IPlayerEntity>());
    }

    public void SetNewTurn()
    {
        TurnCount++;
    }
}

public enum LevelMapStatus
{
    None,
    Walk,
    Battle,
    Shop,
    Leave
}
public enum LevelMapReactionType
{
    StartGamePlay,
    Finish,
    Fail,
    Restart
}

public enum LoseReactionType
{
    Quit,
    Restart,
    Retry
}


public record BattleResult(bool IsAllyWin);

public record GameplayResultCommand(
    GameplayResult Result);

public abstract record GameplayResult;
public record GameplayWinResult() : GameplayResult;
public record GameplayLoseResult(
    LoseReactionType ReactionType) : GameplayResult;

public record LevelMapCommand(
    LevelMapReactionType ReactionType);
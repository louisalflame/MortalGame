using System.Collections.Generic;
using System.Linq;
using Optional;

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
    public GameState State { get; private set; }
    public AllyEntity Ally { get; private set; }
    public EnemyEntity Enemy { get; private set; }
    public TurnStatus TurnStatus { get; private set; }
    public Option<IPlayerEntity> CurrentPlayer { get; private set; } = Option.None<IPlayerEntity>();
    public Option<IPlayerEntity> OppositePlayer => CurrentPlayer
        .Map(current => current.Faction == Faction.Ally ? (IPlayerEntity)Enemy : (IPlayerEntity)Ally);

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
        TurnStatus = new TurnStatus();
    } 

    public void SetState(GameState state)
    {
        State = state;

        CurrentPlayer = State switch
        {
            GameState.EnemyPrepare => (Enemy as IPlayerEntity).Some(),
            GameState.PlayerPrepare => (Ally as IPlayerEntity).Some(),
            GameState.PlayerExecute => (Ally as IPlayerEntity).Some(),
            GameState.EnemyExecute => (Enemy as IPlayerEntity).Some(),
            _ => Option.None<IPlayerEntity>()
        };
    }

    public void SetNewTurn()
    {
        TurnCount++;
        TurnStatus = new TurnStatus();
    }
}

public record BattleResult(bool IsAllyWin);

public record GameplayResultCommand(
    GameplayResult Result);

public abstract record GameplayResult;
public record GameplayWinResult() : GameplayResult;
public record GameplayLoseResult(
    LoseReactionType ReactionType) : GameplayResult;
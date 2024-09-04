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
    public AllyEntity Ally { get; private set; }
    public EnemyEntity Enemy { get; private set; }

    public GameStatus(
        int round,
        GameState state,
        AllyEntity player,
        EnemyEntity enemy) 
    {
        Round = round;
        State = state;
        Ally = player;
        Enemy = enemy;
    }

    public GameStatus With(
        int round = -1,
        GameState state = GameState.None,
        AllyEntity player = null,
        EnemyEntity enemy = null)
    {
        return new GameStatus(
            round: round == -1 ? Round : round,
            state: state == GameState.None ? State : state,
            player: player ?? Ally,
            enemy: enemy ?? Enemy
        );
    }
}

public class GameContextManager
{
    public Dictionary<string, BuffData> BuffTable;
    public Dictionary<string, CardData> CardTable;
    
    private Stack<GameContext> _contextStack = new Stack<GameContext>();
    public GameContext Context => _contextStack.Peek();

    public GameContextManager() 
    {
        _contextStack.Push(new GameContext());
    }
    public GameContext Popout() 
    {
        return _contextStack.Pop();
    }

    public void SetCaster(PlayerEntity Caster) 
    {
        _contextStack.Push(Context.With(caster: Caster));
    }
    public void SetSelectedPlayer(PlayerEntity SelectedPlayer) 
    {
        _contextStack.Push(Context.With(slectedPlayer: SelectedPlayer));
    }
    public void SetSelectedCard(CardEntity SelectedCard) 
    {
        _contextStack.Push(Context.With(selectedCard: SelectedCard));
    }
    public void SetUsingCard(CardEntity UsingCard) 
    {
        _contextStack.Push(Context.With(usingCard: UsingCard));
    }
    public void SetUsingEffect(ICardEffect UsingEffect) 
    {
        _contextStack.Push(Context.With(UsingEffect: UsingEffect));
    }
    public void SetEffectTarget(PlayerEntity EffectTarget) 
    {
        _contextStack.Push(Context.With(EffectTarget: EffectTarget));
    }
}

public class GameContext
{
    public PlayerEntity Caster;
    public PlayerEntity SelectedPlayer;
    public CardEntity SelectedCard;
    public CardEntity UsingCard;
    public ICardEffect UsingEffect;
    public PlayerEntity EffectTarget;

    public GameContext() { }
    public GameContext With(
        PlayerEntity caster = null,
        PlayerEntity slectedPlayer = null,
        CardEntity selectedCard = null,
        CardEntity usingCard = null,
        ICardEffect UsingEffect = null,
        PlayerEntity EffectTarget = null)
    {
        return new GameContext() 
        {
            Caster = caster ?? Caster,
            SelectedPlayer = slectedPlayer ?? SelectedPlayer,
            SelectedCard = selectedCard ?? SelectedCard,
            UsingCard = usingCard ?? UsingCard,
            UsingEffect = UsingEffect ?? UsingEffect,
            EffectTarget = EffectTarget ?? EffectTarget
        };
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
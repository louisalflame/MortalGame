using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IGameplayStatusWatcher
{
    GameStatus GameStatus { get; }
}

public class GameplayManager : IGameplayStatusWatcher
{
    private GameStatus _gameStatus;
    private GameResult _gameResult;
    private List<IGameEvent> _gameEvents;
    private Queue<IGameAction> _gameActions;

    public bool IsEnd { get{ return _gameResult != null; } }
    public GameResult GameResult { get{ return _gameResult; } }
    public GameStatus GameStatus { get{ return _gameStatus; } }

    public GameplayManager(GameStatus initialState)
    {
        _gameStatus = initialState;
    }

    public void Start()
    {
        _gameEvents = new List<IGameEvent>();
        _gameActions = new Queue<IGameAction>();
        _gameResult = null;

        _gameStatus = _gameStatus.With(state: GameState.Player_Prepare);
        _NextState(_gameStatus);
    }

    public void EnqueueAction(IGameAction action)
    {
        _gameActions.Enqueue(action);
    }

    public IReadOnlyCollection<IGameEvent> PopAllEvents()
    {
        _NextState(_gameStatus);
        var events = _gameEvents.ToArray();
        _gameEvents.Clear();
        return events;
    }

    private void _NextState(GameStatus gameStatus)
    {
        switch(gameStatus.State)
        {
            case GameState.Player_Prepare:
                _TurnPreapre(gameStatus.Player);
                break;
            case GameState.Player_DrawCard:
                _TurnDrawCard(gameStatus.Player);
                break;
            case GameState.Player_Execute:
                _TurnExecute(gameStatus.Player);
                break;
            case GameState.Player_Finalize:
                break;
            case GameState.Enemy_Prepare:
                break;
            case GameState.Enemy_DrawCard:
                break;
            case GameState.Enemy_Execute:
                break;
            case GameState.Enemy_Finalize:
                break;
        }
    }

    private void _TurnPreapre(PlayerEntity player)
    {
        _gameStatus = _gameStatus.With(
            round: player.IsNPC ? _gameStatus.Round + 1 : _gameStatus.Round,
            state: GameState.Player_DrawCard
        );
        _gameEvents.Add(new RoundStartEvent(){
            Round = _gameStatus.Round,
            Player = _gameStatus.Player,
            Enemy = _gameStatus.Enemy
        });
    
    }

    private void _TurnDrawCard(PlayerEntity player)
    {
        var drawCount = 0;
        while( player.HandCard.Cards.Count < player.HandCard.MaxCount &&
               player.Deck.Cards.Count > 0)
        {
            drawCount ++;
            _DrawCard(player);
        }

        _gameStatus = _gameStatus.With(
            state: GameState.Player_Execute
        );
    }

    private void _TurnExecute(PlayerEntity player)
    {
        while(_gameActions.Count > 0)
        {
            var action = _gameActions.Dequeue();
            switch(action)
            {
                case UseCardAction useCardAction:
                    Debug.Log($"-- useCard:{useCardAction.CardIndentity} --");
                    break;
                case TurnSubmitAction turnSubmitAction:
                    Debug.Log($"-- submit:{turnSubmitAction.IsNPC} --");
                    if (_gameStatus.State == GameState.Player_Execute &&
                        !turnSubmitAction.IsNPC)
                    {
                        _gameStatus = _gameStatus.With(
                            state: GameState.Player_Finalize
                        );
                    }
                    else if (_gameStatus.State == GameState.Enemy_Execute &&
                        turnSubmitAction.IsNPC)
                    {
                        _gameStatus = _gameStatus.With(
                            state: GameState.Enemy_Finalize
                        );
                    }
                    break;
            }
        }
    }

    private void _DrawCard(PlayerEntity player)
    {
        player.Deck = player.Deck.PopCard(out CardEntity newCard);
        Debug.Log($"player.Deck.Cards.Count:{player.Deck.Cards.Count}");
        player.HandCard = player.HandCard.AddCard(newCard);
        Debug.Log($"player.HandCard.Cards.Count:{player.HandCard.Cards.Count}");

        var newCardInfo = new CardInfo(newCard);
        _gameEvents.Add(new DrawCardEvent(){
            CardInfos = new List<CardInfo> { newCardInfo }
        });
    }
}
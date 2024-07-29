using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameplayManager
{
    private GameStatus _gameStatus;
    private GameResult _gameResult;
    private List<IGameEvent> _gameEvents;

    public bool IsEnd { get{ return _gameResult != null; } }
    public GameResult GameResult { get{ return _gameResult; } }

    public GameplayManager(GameStatus initialState)
    {
        _gameStatus = initialState;
    }

    public void Start()
    {
        _gameEvents = new List<IGameEvent>();
        _gameResult = null;

        _gameStatus = _gameStatus.With(state: GameState.Player_Prepare);
        _NextState(_gameStatus);
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
                _PlayerTurnPreapre(gameStatus.Player);
                break;
            case GameState.Player_DrawCard:
                _DrawCardTurnBegin(gameStatus.Player);
                break;
            case GameState.Player_Execute:
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

    private void _PlayerTurnPreapre(PlayerEntity player)
    {
        _gameStatus = _gameStatus.With(
            state: GameState.Player_DrawCard
        );
    }

    private void _DrawCardTurnBegin(PlayerEntity player)
    {
        var drawCount = 0;
        while(player.HandCard.Cards.Count < 5 && drawCount < 3)
        {
            drawCount ++;
            _DrawCard(player);
        }

        _gameStatus = _gameStatus.With(
            state: GameState.Player_Execute
        );
    }

    private void _DrawCard(PlayerEntity player)
    {
        var newCard = player.Deck.PopCard();
        var newCardInfo = new CardInfo(newCard);

        _gameEvents.Add(new DrawCardEvent(){
            CardInfos = new List<CardInfo> { newCardInfo }
        });
    }
}

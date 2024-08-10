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

        _gameStatus = _gameStatus.With(state: GameState.TurnStart);
        Debug.Log($"-- goto state:{_gameStatus.State} --");
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
            case GameState.TurnStart:
                _TurnStart();
                break;
            case GameState.DrawCard:
                _TurnDrawCard();
                break;
            case GameState.EnemyPrepare:
                _EnemyPreapre();
                break;
            case GameState.PlayerExecute:
                _TurnExecute(gameStatus.Ally);
                break;
            case GameState.Enemy_Execute:
                _EnemyExecute();
                break;
            case GameState.TurnEnd:
                _TurnEnd();
                break;
            case GameState.GameEnd:
                break;
        }
    }

    private void _TurnStart()
    {
        _gameStatus = _gameStatus.With(
            round: _gameStatus.Round + 1,
            state: GameState.DrawCard
        );
        Debug.Log($"-- goto state:{_gameStatus.State} --");   

        _gameEvents.Add(new RoundStartEvent(){
            Round = _gameStatus.Round,
            Player = _gameStatus.Ally,
            Enemy = _gameStatus.Enemy
        });
    }

    private void _TurnDrawCard()
    {
        _DrawCardToMaxCount(_gameStatus.Ally);
        _DrawCardToMaxCount(_gameStatus.Enemy);

        _gameStatus = _gameStatus.With(
            state: GameState.EnemyPrepare
        );
        _gameActions.Clear();
        Debug.Log($"-- goto state:{_gameStatus.State} --");
    }

    private void _EnemyPreapre()
    {
        _gameStatus.Enemy.SelectedCards = _gameStatus.Enemy.GetRecommendCards();
        foreach(var card in _gameStatus.Enemy.SelectedCards)
        {
            Debug.Log($"-- enemy recommend card:{card.Indentity} --");
            _gameEvents.Add(new EnemySelectCardEvent(){
                SelectedCardInfo = new CardInfo(card),
                SelectedCardInfos = _gameStatus.Enemy.SelectedCards.Select(c => new CardInfo(c)).ToArray()
            });
        }

        _gameStatus = _gameStatus.With(
            state: GameState.PlayerExecute
        );
        Debug.Log($"-- goto state:{_gameStatus.State} --");
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
                    _PassCardFromHandToGraveyard(player, useCardAction.CardIndentity);
                    break;
                case TurnSubmitAction turnSubmitAction:
                    Debug.Log($"-- submit:{turnSubmitAction.Faction} --");
                    _FinishExecuteTurn(turnSubmitAction);
                    break;
            }
        }
    }

    private void _EnemyExecute()
    {
        foreach(var card in _gameStatus.Enemy.SelectedCards)
        {
            Debug.Log($"-- enemy use card:{card.Indentity} --");
            _gameActions.Enqueue(new UseCardAction(){
                CardIndentity = card.Indentity
            });
        }

        _TurnExecute(_gameStatus.Enemy);

        _gameStatus = _gameStatus.With(
            state: GameState.TurnEnd
        );
        Debug.Log($"-- goto state:{_gameStatus.State} --");
    }

    private void _TurnEnd()
    {
        _gameStatus = _gameStatus.With(
            state: GameState.TurnStart
        );
        Debug.Log($"-- goto state:{_gameStatus.State} --");
    }

    private void _PassCardFromHandToGraveyard(PlayerEntity player, string CardIndentity)
    {
        var usedCard = player.HandCard.Cards.FirstOrDefault(c => c.Indentity == CardIndentity);
        if (usedCard != null)
        {
            player.HandCard = player.HandCard.RemoveCard(usedCard);
            player.Graveyard = player.Graveyard.AddCard(usedCard);

            var usedCardInfo = new CardInfo(usedCard);
            _gameEvents.Add(new UsedCardEvent() {
                Faction = player.Faction,
                UsedCardInfo = usedCardInfo,
                HandCardInfos = player.HandCard.CardInfos,
                GraveyardCardInfos = player.Graveyard.CardInfos
        });
        }
    }

    private void _FinishExecuteTurn(TurnSubmitAction turnSubmitAction)
    {
        if (_gameStatus.State == GameState.PlayerExecute &&
            turnSubmitAction.Faction == Faction.Ally)
        {
            _gameStatus = _gameStatus.With(
                state: GameState.Enemy_Execute
            );
        }
        else if (_gameStatus.State == GameState.Enemy_Execute &&
            turnSubmitAction.Faction == Faction.Enemy)
        {
            _gameStatus = _gameStatus.With(
                state: GameState.TurnEnd
            );
        }
        Debug.Log($"-- goto state:{_gameStatus.State} --");
    }

    private void _DrawCardToMaxCount(PlayerEntity player)
    {
        while( player.HandCard.Cards.Count < player.HandCard.MaxCount)
        {
            if( player.Deck.Cards.Count == 0 &&
                player.Graveyard.Cards.Count > 0)
            {
                player.Graveyard = player.Graveyard.PopAllCards(out IReadOnlyCollection<CardEntity> graveyardCards);
                player.Deck = player.Deck.EnqueueCards(graveyardCards).Shuffle();
                _gameEvents.Add(new RecycleGraveyardEvent() {
                    Faction = player.Faction,
                    DeckCardInfos = player.Deck.CardInfos,
                    GraveyardCardInfos = player.Graveyard.CardInfos
                });
            }

            if (player.Deck.Cards.Count > 0)
                _DrawCard(player); 
            else
                break;
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
            Faction = player.Faction,
            NewCardInfo = newCardInfo,
            HandCardInfos = player.HandCard.CardInfos,
            DeckCardInfos = player.Deck.CardInfos,
        });
    }
}

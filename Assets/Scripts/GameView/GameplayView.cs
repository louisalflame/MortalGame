using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameplayView : MonoBehaviour
{
    [SerializeField]
    private PlayerInfoView _playerInfoView;
    [SerializeField]
    private EnemyInfoView  _enemyInfoView;
    [SerializeField]
    private PlayerHandCardView _playerHandCardView;
    [SerializeField]
    private EnemySelectedCardView _enemySelectedCardView;
    [SerializeField]
    private DeckCardView _deckCardView;
    [SerializeField]
    private GraveyardCardView _graveyardCardView;
    [SerializeField]
    private SubmitView _submitView;

    public void Init(IGameplayActionReciever reciever, IGameplayStatusWatcher statusWatcher)
    {
        _playerInfoView.Init(statusWatcher);
        _enemyInfoView.Init(statusWatcher);
        _playerHandCardView.Init(statusWatcher, reciever);
        _enemySelectedCardView.Init(statusWatcher, reciever);
        _deckCardView.Init(statusWatcher);
        _graveyardCardView.Init(statusWatcher);
        _submitView.Init(reciever);
    }

    public void Render(IReadOnlyCollection<IGameEvent> events, IGameplayActionReciever reciever) 
    {
        foreach (var gameEvent in events)
        {
            switch (gameEvent)
            {
                case RoundStartEvent roundStartEvent:
                    _UpdateRoundAndPlayer(roundStartEvent);
                    break;
                case DrawCardEvent drawCardEvent:
                    _DrawCardView(drawCardEvent, reciever);
                    break;
                case EnemySelectCardEvent enemySelectCardEvent:
                    _SelectCardView(enemySelectCardEvent);
                    break;
                case UsedCardEvent usedCardEvent:
                    _UsedCardView(usedCardEvent);
                    break;
            }
        }
    }

    private void _UpdateRoundAndPlayer(RoundStartEvent roundStartEvent)
    {
        _playerInfoView.SetPlayerInfo(roundStartEvent.Round, roundStartEvent.Player);
        _enemyInfoView.SetPlayerInfo(roundStartEvent.Enemy);
    }

    private void _DrawCardView(DrawCardEvent drawCardEvent, IGameplayActionReciever reciever)
    {
        switch (drawCardEvent.Faction)
        {
            case Faction.Ally:
                _playerHandCardView.CreateCardView(drawCardEvent);
                _deckCardView.UpdateDeckView(drawCardEvent);
                break;
            case Faction.Enemy:
                _enemySelectedCardView.UpdateDeckView(drawCardEvent);
                break;
        }
    }

    private void _SelectCardView(EnemySelectCardEvent enemySelectCardEvent)
    {
        _enemySelectedCardView.CreateCardView(enemySelectCardEvent);
    }

    private void _UsedCardView(UsedCardEvent usedCardEvent)
    {
        switch (usedCardEvent.Faction)
        {
            case Faction.Ally:
                _playerHandCardView.RemoveCardView(usedCardEvent);
                _graveyardCardView.UpdateDeckView(usedCardEvent);
                break;
            case Faction.Enemy:
                _enemySelectedCardView.RemoveCardView(usedCardEvent);
                break;
        }
    }
}

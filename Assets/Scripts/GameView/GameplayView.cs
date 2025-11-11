using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public interface IGameplayView : IAllCardDetailPanelView, IInteractionButtonView
{
    void Init(
        IGameViewModel gameInfoModel,
        IGameplayActionReciever reciever, 
        IGameplayStatusWatcher statusWatcher, 
        LocalizeLibrary localizeLibrary,
        DispositionLibrary dispositionLibrary);
    void Render(IReadOnlyCollection<IGameEvent> events, IGameplayActionReciever reciever);
    void DisableAllHandCards();
    void DisableAllInteraction();

    IEnumerable<ISelectableView> SelectableViews { get; }
    ISelectableView BasicSelectableView { get; }
}

public interface IInteractionButtonView
{ 
    DeckCardView DeckCardView { get; }
    GraveyardCardView GraveyardCardView { get; }
}
public interface IAllCardDetailPanelView
{
    AllCardDetailPanel DetailPanel { get; }
    SingleCardDetailPopupPanel SinglePopupPanel { get; }
    FocusCardDetailView FocusCardDetailView { get; }
}

public class GameplayView : MonoBehaviour, IGameplayView
{
    [BoxGroup("Canvas")]
    [SerializeField]
    private Canvas _handCardCanvas;
    [BoxGroup("Canvas")]
    [SerializeField]
    private Canvas _overlayCanvas;

    [BoxGroup("PlayGround")]
    [SerializeField]
    private PlaygroundView _playGround;

    [BoxGroup("PlayGround")]
    [SerializeField]
    private TopBarInfoView _topBarInfoView;

    [BoxGroup("AllyView")]
    [SerializeField]
    private AllyInfoView _allyInfoView;
    [BoxGroup("AllyView")]
    [SerializeField]
    private AllyCharacterView _allyCharacterView;
    
    [BoxGroup("EnemyView")]
    [SerializeField]
    private EnemyInfoView  _enemyInfoView;
    [BoxGroup("EnemyView")]
    [SerializeField]
    private EnemyCharacterView _enemyCharacterView;
    [BoxGroup("EnemyView")]
    [SerializeField]
    private EnemySelectedCardView _enemySelectedCardView;
    
    [BoxGroup("HandView")]
    [SerializeField]
    private AllyHandCardView _allyHandCardView;
    [BoxGroup("HandView")]
    [SerializeField]
    private DeckCardView _deckCardView;
    [BoxGroup("HandView")]
    [SerializeField]
    private GraveyardCardView _graveyardCardView;
    [BoxGroup("HandView")]
    [SerializeField]
    private SubmitView _submitView;

    [BoxGroup("Popup")]
    [SerializeField]
    private AllCardDetailPanel _allCardDetailPanel;
    [BoxGroup("Popup")]
    [SerializeField]
    private SingleCardDetailPopupPanel _singleCardDetailPopupPanel;
    [BoxGroup("Popup")]
    [SerializeField]
    private FocusCardDetailView _focusCardDetailView;
    [BoxGroup("Popup")]
    [SerializeField]
    private SimpleTitleInfoHintView _simpleHintView;

    public AllCardDetailPanel DetailPanel => _allCardDetailPanel;
    public SingleCardDetailPopupPanel SinglePopupPanel => _singleCardDetailPopupPanel;
    public FocusCardDetailView FocusCardDetailView => _focusCardDetailView;
    public DeckCardView DeckCardView => _deckCardView;
    public GraveyardCardView GraveyardCardView => _graveyardCardView;

    private IGameViewModel _gameViewModel;

    public IEnumerable<ISelectableView> SelectableViews
    {
        get
        {
            return _allyHandCardView.SelectableViews
                .Concat(_enemySelectedCardView.SelectableViews)
                .Append(_allyCharacterView)
                .Append(_enemyCharacterView);
        }
    }
    public ISelectableView BasicSelectableView => _playGround;

    public void Init(
        IGameViewModel gameInfoModel,
        IGameplayActionReciever reciever, 
        IGameplayStatusWatcher statusWatcher,
        LocalizeLibrary localizeLibrary, 
        DispositionLibrary dispositionLibrary)
    {
        _gameViewModel = gameInfoModel;

        _allyInfoView.Init(_gameViewModel, _topBarInfoView, _simpleHintView, localizeLibrary, dispositionLibrary);
        _allyHandCardView.Init(statusWatcher, reciever, _gameViewModel, this, localizeLibrary);
        _allyCharacterView.Init(statusWatcher);

        _enemyInfoView.Init(statusWatcher, _gameViewModel, _simpleHintView, localizeLibrary);
        _enemySelectedCardView.Init(statusWatcher, reciever, localizeLibrary);
        _enemyCharacterView.Init(statusWatcher);

        _deckCardView.Init(_gameViewModel);
        _graveyardCardView.Init(_gameViewModel);
        _submitView.Init(reciever);

        _focusCardDetailView.Init(_gameViewModel, localizeLibrary);
        _singleCardDetailPopupPanel.Init(_gameViewModel, localizeLibrary);
        _simpleHintView.Init(localizeLibrary);
    }

    public void Render(IReadOnlyCollection<IGameEvent> events, IGameplayActionReciever reciever) 
    {
        foreach (var gameEvent in events)
        {
            switch (gameEvent)
            {
                case GeneralUpdateEvent updateEvent:
                    _UpdateGeneralInfo(updateEvent);
                    break;
                case AllySummonEvent allySummonEvent:
                    _AllySummonEvent(allySummonEvent);
                    break;
                case EnemySummonEvent enemySummonEvent:
                    _EnemySummonEvent(enemySummonEvent);
                    break;
                case RoundStartEvent roundStartEvent:
                    _UpdateRoundAndPlayer(roundStartEvent);
                    break;
                case RecycleGraveyardEvent recycleGraveyardEvent:
                    _RecycleGraveyardEvent(recycleGraveyardEvent);
                    break;
                case RecycleHandCardEvent recycleHandCardEvent:
                    _RecycleHandCardEvent(recycleHandCardEvent);
                    break;
                case DrawCardEvent drawCardEvent:
                    _DrawCardView(drawCardEvent, reciever);
                    break;
                case MoveCardEvent moveCardEvent:
                    _MoveCardView(moveCardEvent);
                    break;
                case AddCardEvent addCardEvent:
                    _AddCardView(addCardEvent);
                    break;
                case EnemySelectCardEvent enemySelectCardEvent:
                    _SelectCardView(enemySelectCardEvent);
                    break;
                case EnemyUnselectedCardEvent enemyUnselectedCardEvent:
                    _UnselectCardView(enemyUnselectedCardEvent);
                    break;
                case PlayerExecuteStartEvent playerExecuteStartEvent:
                    _PlayerExecuteStart(playerExecuteStartEvent);
                    break;
                case UsedCardEvent usedCardEvent:
                    _UsedCardView(usedCardEvent);
                    break;
                case PlayerExecuteEndEvent playerExecuteEndEvent:
                    _PlayerExecuteEnd(playerExecuteEndEvent);
                    break;
                case LoseEnergyEvent loseEnergyEvent:
                    _ConsumeEnergyView(loseEnergyEvent);
                    break;
                case GainEnergyEvent gainEnergyEvent:
                    _gainEnergyView(gainEnergyEvent);
                    break;
                case IncreaseDispositionEvent increaseDispositionEvent:
                    _IncreaseDispositionView(increaseDispositionEvent);
                    break;
                case HealthEvent healthEvent:
                    _updateHealthView(healthEvent);
                    break;
                case AddPlayerBuffEvent addBuffEvent:
                    _AddBuffView(addBuffEvent);
                    break;
                case RemovePlayerBuffEvent removeBuffEvent:
                    _RemoveBuffView(removeBuffEvent);
                    break;
            }
        }
    }

    public void DisableAllHandCards()
    {
        _gameViewModel.DisableHandCardsAction();
        _allyHandCardView.DisableAllHandCardsAction();
    }
    public void DisableAllInteraction()
    {
        DisableAllHandCards();
        // TODO: disable other interaction
    }

    private void _UpdateGeneralInfo(GeneralUpdateEvent updateEvent)
    {
        foreach (var playerBuffInfo in updateEvent.PlayerBuffInfos)
        {
            _gameViewModel.UpdatePlayerBuffInfo(playerBuffInfo);
        }
        foreach (var characterBuffInfo in updateEvent.CharacterBuffInfos)
        {
            _gameViewModel.UpdateCharacterBuffInfo(characterBuffInfo);
        }
        foreach (var cardInfo in updateEvent.CardInfos)
        {
            _gameViewModel.UpdateCardInfo(cardInfo);
        }
    }

    private void _AllySummonEvent(AllySummonEvent allySummonEvent)
    {
        _allyCharacterView.SummonAlly(allySummonEvent);
    }
    private void _EnemySummonEvent(EnemySummonEvent enemySummonEvent)
    {
        _enemyCharacterView.SummonEnemy(enemySummonEvent);
    }
    private void _UpdateRoundAndPlayer(RoundStartEvent roundStartEvent)
    {
        _gameViewModel.UpdateDispositionInfo(
            new(roundStartEvent.Player.DispositionManager.CurrentDisposition,
                roundStartEvent.Player.DispositionManager.MaxDisposition));
        _allyInfoView.SetPlayerInfo(roundStartEvent.Round, roundStartEvent.Player);
        _enemyInfoView.SetPlayerInfo(roundStartEvent.Enemy);
    }

    private void _DrawCardView(DrawCardEvent drawCardEvent, IGameplayActionReciever reciever)
    {
        _gameViewModel.UpdateCardInfo(drawCardEvent.NewCardInfo);
        _gameViewModel.UpdateCardCollectionInfo(drawCardEvent.Faction, drawCardEvent.HandCardInfo);
        _gameViewModel.UpdateCardCollectionInfo(drawCardEvent.Faction, drawCardEvent.DeckInfo);
        switch (drawCardEvent.Faction)
        {
            case Faction.Ally:
                _allyHandCardView.CreateCardView(drawCardEvent.NewCardInfo, drawCardEvent.HandCardInfo);
                break;
            case Faction.Enemy:
                _gameViewModel.UpdateCardInfo(drawCardEvent.NewCardInfo);
                _enemySelectedCardView.UpdateDeckView(drawCardEvent);
                break;
        }
    }

    private void _MoveCardView(MoveCardEvent moveCardEvent)
    {
        _gameViewModel.UpdateCardCollectionInfo(moveCardEvent.Faction, moveCardEvent.StartZoneInfo);
        _gameViewModel.UpdateCardCollectionInfo(moveCardEvent.Faction, moveCardEvent.DestinationZoneInfo);
        switch (moveCardEvent.Faction)
        {
            case Faction.Ally:
                switch (moveCardEvent.StartZoneInfo.Type)
                {
                    case CardCollectionType.HandCard:
                        _allyHandCardView.RemoveCardView(moveCardEvent);
                        break;
                }
                break;
            case Faction.Enemy:
                switch (moveCardEvent.StartZoneInfo.Type)
                {
                    case CardCollectionType.HandCard:
                        _enemySelectedCardView.RemoveCardView(moveCardEvent);
                        break;
                }
                break;
        }
    }

    private void _AddCardView(AddCardEvent addCardEvent)
    {
        _gameViewModel.UpdateCardCollectionInfo(addCardEvent.Faction, addCardEvent.DestinationZoneInfo);
        switch (addCardEvent.Faction)
        {
            case Faction.Ally:
                switch (addCardEvent.DestinationZoneInfo.Type)
                {
                    case CardCollectionType.HandCard:
                        _allyHandCardView.CreateCardView(addCardEvent.CardInfo, addCardEvent.DestinationZoneInfo);
                        break;
                }
                break;
            case Faction.Enemy:
                switch(addCardEvent.DestinationZoneInfo.Type)
                {
                    case CardCollectionType.HandCard:
                        _enemySelectedCardView.CreateCardView(addCardEvent);
                        break;
                }
                break;
        }
    }

    private void _RecycleGraveyardEvent(RecycleGraveyardEvent recycleGraveyardEvent)
    {
        _gameViewModel.UpdateCardCollectionInfo(recycleGraveyardEvent.Faction, recycleGraveyardEvent.DeckInfo);
        _gameViewModel.UpdateCardCollectionInfo(recycleGraveyardEvent.Faction, recycleGraveyardEvent.GraveyardInfo);
        switch (recycleGraveyardEvent.Faction)
        {
            case Faction.Ally:
                break;
            case Faction.Enemy:
                _enemySelectedCardView.UpdateDeckView(recycleGraveyardEvent);
                break;
        }
    }
    private void _RecycleHandCardEvent(RecycleHandCardEvent recycleHandCardEvent)
    {
        _gameViewModel.UpdateCardCollectionInfo(recycleHandCardEvent.Faction, recycleHandCardEvent.HandCardInfo);
        _gameViewModel.UpdateCardCollectionInfo(recycleHandCardEvent.Faction, recycleHandCardEvent.GraveyardInfo);
        _gameViewModel.UpdateCardCollectionInfo(recycleHandCardEvent.Faction, recycleHandCardEvent.ExclusionZoneInfo);
        _gameViewModel.UpdateCardCollectionInfo(recycleHandCardEvent.Faction, recycleHandCardEvent.DisposeZoneInfo);
        switch (recycleHandCardEvent.Faction)
        {
            case Faction.Ally:
                _allyHandCardView.RecycleHandCards(recycleHandCardEvent);
                break;
            case Faction.Enemy:
                _enemySelectedCardView.RemoveCardView(recycleHandCardEvent);
                break;
        }
    }

    private void _SelectCardView(EnemySelectCardEvent enemySelectCardEvent)
    {
        _enemySelectedCardView.CreateCardView(enemySelectCardEvent);
    }
    private void _UnselectCardView(EnemyUnselectedCardEvent enemyUnselectedCardEvent)
    {
        _enemySelectedCardView.RemoveCardView(enemyUnselectedCardEvent);
    }

    private void _PlayerExecuteStart(PlayerExecuteStartEvent playerExecuteStartEvent)
    {
        _gameViewModel.EnableHandCardsAction(playerExecuteStartEvent.HandCardInfo);
        _gameViewModel.UpdateCardCollectionInfo(playerExecuteStartEvent.Faction, playerExecuteStartEvent.GraveyardInfo);
        _gameViewModel.UpdateCardCollectionInfo(playerExecuteStartEvent.Faction, playerExecuteStartEvent.ExclusionZoneInfo);
        _gameViewModel.UpdateCardCollectionInfo(playerExecuteStartEvent.Faction, playerExecuteStartEvent.DisposeZoneInfo);
        _allyHandCardView.EnableHandCardsUseCardAction(playerExecuteStartEvent);
    }
    private void _PlayerExecuteEnd(PlayerExecuteEndEvent playerExecuteEndEvent)
    {
        _gameViewModel.DisableHandCardsAction();
        _gameViewModel.UpdateCardCollectionInfo(playerExecuteEndEvent.Faction, playerExecuteEndEvent.HandCardInfo);
    }
    private void _UsedCardView(UsedCardEvent usedCardEvent)
    {
        _gameViewModel.UpdateCardCollectionInfo(usedCardEvent.Faction, usedCardEvent.HandCardInfo);
        _gameViewModel.UpdateCardCollectionInfo(usedCardEvent.Faction, usedCardEvent.GraveyardInfo);
        _gameViewModel.UpdateCardCollectionInfo(usedCardEvent.Faction, usedCardEvent.ExclusionZoneInfo);
        _gameViewModel.UpdateCardCollectionInfo(usedCardEvent.Faction, usedCardEvent.DisposeZoneInfo);
        switch (usedCardEvent.Faction)
        {
            case Faction.Ally:
                _allyHandCardView.RemoveCardView(usedCardEvent);
                break;
            case Faction.Enemy:
                _enemySelectedCardView.RemoveCardView(usedCardEvent);
                break;
        }
    }

    private void _ConsumeEnergyView(LoseEnergyEvent loseEnergyEvent)
    {
        switch (loseEnergyEvent.Faction)
        {
            case Faction.Ally:
                _allyInfoView.UpdateEnergy(loseEnergyEvent);
                break;
            case Faction.Enemy:
                _enemyInfoView.UpdateEnergy(loseEnergyEvent);
                break;
        }
    }
    private void _gainEnergyView(GainEnergyEvent gainEnergyEvent)
    {
        switch (gainEnergyEvent.Faction)
        {
            case Faction.Ally:
                _allyInfoView.UpdateEnergy(gainEnergyEvent);
                break;
            case Faction.Enemy:
                _enemyInfoView.UpdateEnergy(gainEnergyEvent);
                break;
        }
    }
    private void _IncreaseDispositionView(IncreaseDispositionEvent increaseDispositionEvent)
    {
        _gameViewModel.UpdateDispositionInfo(increaseDispositionEvent.Info);
        _allyCharacterView.UpdateDisposition(increaseDispositionEvent);
    }
    private void _DecreaseDispositionView(DecreaseDispositionEvent decreaseDispositionEvent)
    {
        _gameViewModel.UpdateDispositionInfo(decreaseDispositionEvent.Info);
        _allyCharacterView.UpdateDisposition(decreaseDispositionEvent);
    }

    private void _updateHealthView(HealthEvent healthEvent)
    {
        switch (healthEvent.Faction)
        {
            case Faction.Ally:
                _allyInfoView.UpdateHealth(healthEvent);
                _allyCharacterView.UpdateHealth(healthEvent);
                break;
            case Faction.Enemy:
                _enemyInfoView.UpdateHealth(healthEvent);
                _enemyCharacterView.UpdateHealth(healthEvent);
                break;
        }
    }

    private void _AddBuffView(AddPlayerBuffEvent addBuffEvent)
    {
        switch (addBuffEvent.Faction)
        {
            case Faction.Ally:
                _gameViewModel.UpdatePlayerBuffInfo(addBuffEvent.Buff);
                _allyInfoView.AddBuff(addBuffEvent);
                break;
            case Faction.Enemy:
                _gameViewModel.UpdatePlayerBuffInfo(addBuffEvent.Buff);
                _enemyInfoView.AddBuff(addBuffEvent);
                break;
        }
    }
    private void _RemoveBuffView(RemovePlayerBuffEvent removeBuffEvent)
    {
        switch (removeBuffEvent.Faction)
        {
            case Faction.Ally:
                _allyInfoView.RemoveBuff(removeBuffEvent);
                break;
            case Faction.Enemy:
                _enemyInfoView.RemoveBuff(removeBuffEvent);
                break;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public interface IGameplayView : IAllCardDetailPanelView
{
    void Init(
        IGameplayActionReciever reciever, 
        IGameplayStatusWatcher statusWatcher, 
        LocalizeLibrary localizeLibrary,
        DispositionLibrary dispositionLibrary);
    void Render(IReadOnlyCollection<IGameEvent> events, IGameplayActionReciever reciever);
    void DisableAllHandCards();

    IEnumerable<ISelectableView> SelectableViews { get; }
    ISelectableView BasicSelectableView { get; }
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
    private SimpleTitleIInfoHintView _simpleHintView;

    public AllCardDetailPanel DetailPanel => _allCardDetailPanel;
    public SingleCardDetailPopupPanel SinglePopupPanel => _singleCardDetailPopupPanel;
    public FocusCardDetailView FocusCardDetailView => _focusCardDetailView;

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
        IGameplayActionReciever reciever, 
        IGameplayStatusWatcher statusWatcher,
        LocalizeLibrary localizeLibrary, 
        DispositionLibrary dispositionLibrary)
    {
        _allyInfoView.Init(statusWatcher, _topBarInfoView, _simpleHintView, localizeLibrary, dispositionLibrary);
        _allyHandCardView.Init(statusWatcher, reciever, this, localizeLibrary);
        _allyCharacterView.Init(statusWatcher);

        _enemyInfoView.Init(statusWatcher, _simpleHintView, localizeLibrary);
        _enemySelectedCardView.Init(statusWatcher, reciever, localizeLibrary);
        _enemyCharacterView.Init(statusWatcher);

        _deckCardView.Init(statusWatcher, reciever);
        _graveyardCardView.Init(statusWatcher, reciever);
        _submitView.Init(reciever);

        _focusCardDetailView.Init(_overlayCanvas, localizeLibrary);
        _singleCardDetailPopupPanel.Init(localizeLibrary);
        _simpleHintView.Init(localizeLibrary);
    }

    public void Render(IReadOnlyCollection<IGameEvent> events, IGameplayActionReciever reciever) 
    {
        foreach (var gameEvent in events)
        {
            Debug.Log($"-- GameplayView.Render:[{gameEvent}] --");
            switch (gameEvent)
            {
                case AllySummonEvent allySummonEvent:
                    _AllySummonEvent(allySummonEvent);
                    break;
                case EnemySummonEvent enemySummonEvent:
                    _EnemySummonEvent(enemySummonEvent);
                    break;
                case RoundStartEvent roundStartEvent:
                    _UpdateRoundAndPlayer(roundStartEvent);
                    break;
                case DrawCardEvent drawCardEvent:
                    _DrawCardView(drawCardEvent, reciever);
                    break;
                case DiscardCardEvent discardCardEvent:
                    _DiscardCardView(discardCardEvent);
                    break;
                case RecycleGraveyardEvent recycleGraveyardEvent:
                    _RecycleGraveyardEvent(recycleGraveyardEvent);
                    break;
                case RecycleHandCardEvent recycleHandCardEvent:
                    _RecycleHandCardEvent(recycleHandCardEvent);
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
                case ConsumeEnergyEvent consumeEnergyEvent:
                    _ConsumeEnergyView(consumeEnergyEvent);
                    break;
                case GainEnergyEvent gainEnergyEvent:
                    _gainEnergyView(gainEnergyEvent);
                    break;
                case HealthEvent healthEvent:
                    _updateHealthView(healthEvent);
                    break;
                case AddBuffEvent addBuffEvent:
                    _AddBuffView(addBuffEvent);
                    break;
                case UpdateBuffEvent updateBuffEvent:
                    _UpdateBuffView(updateBuffEvent);
                    break;
            }
        }
    }

    public void DisableAllHandCards()
    {
        _allyHandCardView.DisableAllHandCards();
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
        _allyInfoView.SetPlayerInfo(roundStartEvent.Round, roundStartEvent.Player);
        _enemyInfoView.SetPlayerInfo(roundStartEvent.Enemy);
    }

    private void _DrawCardView(DrawCardEvent drawCardEvent, IGameplayActionReciever reciever)
    {
        switch (drawCardEvent.Faction)
        {
            case Faction.Ally:
                _allyHandCardView.CreateCardView(drawCardEvent);
                _deckCardView.UpdateDeckView(drawCardEvent);
                break;
            case Faction.Enemy:
                _enemySelectedCardView.UpdateDeckView(drawCardEvent);
                break;
        }
    }

    private void _DiscardCardView(DiscardCardEvent discardCardEvent)
    {
        switch (discardCardEvent.Faction)
        {
            case Faction.Ally:
                _allyHandCardView.RemoveCardView(discardCardEvent);
                _graveyardCardView.UpdateDeckView(discardCardEvent);
                break;
            case Faction.Enemy:
                _enemySelectedCardView.RemoveCardView(discardCardEvent);
                break;
        }
    }

    private void _RecycleGraveyardEvent(RecycleGraveyardEvent recycleGraveyardEvent)
    {
        switch (recycleGraveyardEvent.Faction)
        {
            case Faction.Ally:
                _deckCardView.UpdateDeckView(recycleGraveyardEvent);
                _graveyardCardView.UpdateDeckView(recycleGraveyardEvent);
                break;
            case Faction.Enemy:
                _enemySelectedCardView.UpdateDeckView(recycleGraveyardEvent);
                break;
        }
    }
    private void _RecycleHandCardEvent(RecycleHandCardEvent recycleHandCardEvent)
    {
        switch (recycleHandCardEvent.Faction)
        {
            case Faction.Ally:
                _allyHandCardView.RecycleHandCards(recycleHandCardEvent);
                _graveyardCardView.UpdateDeckView(recycleHandCardEvent);
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
        _allyHandCardView.EnableHandCardsUseCardAction(playerExecuteStartEvent);
    }
    private void _PlayerExecuteEnd(PlayerExecuteEndEvent playerExecuteEndEvent)
    {
    }
    private void _UsedCardView(UsedCardEvent usedCardEvent)
    {
        switch (usedCardEvent.Faction)
        {
            case Faction.Ally:
                _allyHandCardView.RemoveCardView(usedCardEvent);
                _graveyardCardView.UpdateDeckView(usedCardEvent);
                break;
            case Faction.Enemy:
                _enemySelectedCardView.RemoveCardView(usedCardEvent);
                break;
        }
    }

    private void _ConsumeEnergyView(ConsumeEnergyEvent consumeEnergyEvent)
    {
        switch (consumeEnergyEvent.Faction)
        {
            case Faction.Ally:
                _allyInfoView.UpdateEnergy(consumeEnergyEvent);
                break;
            case Faction.Enemy:
                _enemyInfoView.UpdateEnergy(consumeEnergyEvent);
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

    private void _AddBuffView(AddBuffEvent addBuffEvent)
    {
        switch (addBuffEvent.Faction)
        {
            case Faction.Ally:
                _allyInfoView.AddBuff(addBuffEvent);
                break;
            case Faction.Enemy:
                _enemyInfoView.AddBuff(addBuffEvent);
                break;
        }
    }
    private void _UpdateBuffView(UpdateBuffEvent updateBuffEvent)
    {
        switch (updateBuffEvent.Faction)
        {
            case Faction.Ally:
                _allyInfoView.UpdateBuff(updateBuffEvent);
                break;
            case Faction.Enemy:
                _enemyInfoView.UpdateBuff(updateBuffEvent);
                break;
        }
    }
}

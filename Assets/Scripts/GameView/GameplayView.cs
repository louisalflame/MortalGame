using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public interface IGameplayView
{
    void Init(IGameplayActionReciever reciever, IGameplayStatusWatcher statusWatcher);
    void Render(IReadOnlyCollection<IGameEvent> events, IGameplayActionReciever reciever);
    UniTask Run();

    void ClickDeckDetailPanel();
    void ClickGraveyardDetailPanel();
}

public enum GameplayViewState
{
    Idle = 0,
    DeckDetailPanel,
    GraveyardDetailPanel,
}

public class GameplayView : MonoBehaviour, IGameplayView
{
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
    private DeckDetailPanel _deckDetailPanel;
    [BoxGroup("HandView")]
    [SerializeField]
    private GraveyardCardView _graveyardCardView;
    [BoxGroup("HandView")]
    [SerializeField]
    private GraveyardDetailPanel _graveyardDetailPanel;
    [BoxGroup("HandView")]
    [SerializeField]
    private SubmitView _submitView;

    private GameplayViewState _state = GameplayViewState.Idle;

    public void Init(IGameplayActionReciever reciever, IGameplayStatusWatcher statusWatcher)
    {
        _allyInfoView.Init(statusWatcher);
        _enemyInfoView.Init(statusWatcher);
        _allyHandCardView.Init(statusWatcher, reciever);
        _enemySelectedCardView.Init(statusWatcher, reciever);
        _deckCardView.Init(statusWatcher, this);
        _deckDetailPanel.Init(statusWatcher);
        _graveyardCardView.Init(statusWatcher, this);
        _graveyardDetailPanel.Init(statusWatcher);
        _submitView.Init(reciever);
    }

    public void ClickDeckDetailPanel()
    {
        if (_state == GameplayViewState.Idle)
            _state = GameplayViewState.DeckDetailPanel;
    }

    public void ClickGraveyardDetailPanel()
    {
        if (_state == GameplayViewState.Idle)
            _state = GameplayViewState.GraveyardDetailPanel;
    }

    public async UniTask Run()
    {
        while (true)
        {
            switch(_state)
            {
                case GameplayViewState.Idle:
                    await UniTask.NextFrame();
                    break;
                case GameplayViewState.DeckDetailPanel:
                    await _deckDetailPanel.Run();
                    _state = GameplayViewState.Idle;
                    break;
                case GameplayViewState.GraveyardDetailPanel:
                    await _graveyardDetailPanel.Run();
                    _state = GameplayViewState.Idle;
                    break;
            }
        }
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
        _allyHandCardView.DisableHandCardsUseCardAction(playerExecuteEndEvent);
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
                break;
            case Faction.Enemy:
                _enemyInfoView.UpdateHealth(healthEvent);
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

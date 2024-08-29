using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameplayView : MonoBehaviour
{
    [SerializeField]
    private AllyInfoView _allyInfoView;
    [SerializeField]
    private EnemyInfoView  _enemyInfoView;
    [SerializeField]
    private AllyHandCardView _allyHandCardView;
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
        _allyInfoView.Init(statusWatcher);
        _enemyInfoView.Init(statusWatcher);
        _allyHandCardView.Init(statusWatcher, reciever);
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
                case UsedCardEvent usedCardEvent:
                    _UsedCardView(usedCardEvent);
                    break;
                case ConsumeEnergyEvent consumeEnergyEvent:
                    _ConsumeEnergyView(consumeEnergyEvent);
                    break;
                case GainEnergyEvent gainEnergyEvent:
                    _gainEnergyView(gainEnergyEvent);
                    break;
                case TakeDamageEvent takeDamageEvent:
                    _TakeDamageView(takeDamageEvent);
                    break;
                case GetHealEvent getHealEvent:
                    _GetHealView(getHealEvent);
                    break;
                case GetShieldEvent getShieldEvent:
                    _GetShieldView(getShieldEvent);
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

    private void _TakeDamageView(TakeDamageEvent takeDamageEvent)
    {
        switch (takeDamageEvent.Faction)
        {
            case Faction.Ally:
                _allyInfoView.UpdateHealth(takeDamageEvent);
                break;
            case Faction.Enemy:
                _enemyInfoView.UpdateHealth(takeDamageEvent);
                break;
        }
    }
    private void _GetHealView(GetHealEvent getHealEvent)
    {
        switch (getHealEvent.Faction)
        {
            case Faction.Ally:
                _allyInfoView.UpdateHealth(getHealEvent);
                break;
            case Faction.Enemy:
                _enemyInfoView.UpdateHealth(getHealEvent);
                break;
        }
    }
    private void _GetShieldView(GetShieldEvent getShieldEvent)
    {
        switch (getShieldEvent.Faction)
        {
            case Faction.Ally:
                _allyInfoView.UpdateHealth(getShieldEvent);
                break;
            case Faction.Enemy:
                _enemyInfoView.UpdateHealth(getShieldEvent);
                break;
        }
    }
}

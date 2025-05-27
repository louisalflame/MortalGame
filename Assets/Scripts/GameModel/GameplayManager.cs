using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Optional;
using Optional.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public interface IGameplayStatusWatcher
{
    GameStatus GameStatus { get; }
    IGameContextManager ContextManager { get; }
}

public interface IGameplayReactor
{
    void UpdateReactorSessionTiming(UpdateTiming updateTiming);
    void UpdateReactorSessionAction(IActionUnit actionUnit);
}

public interface IGameEventWatcher : IGameplayStatusWatcher
    {
        event Action OnUseCard;
        event Action OneTurnStart;
        event Action OnTurnEnd;
    }

public class GameplayManager : IGameplayStatusWatcher, IGameEventWatcher, IGameplayReactor
{
    public event Action OnUseCard;
    public event Action OneTurnStart;
    public event Action OnTurnEnd;

    private GameStatus _gameStatus;
    private GameResult _gameResult;
    private List<IGameEvent> _gameEvents;
    private Queue<IGameAction> _gameActions;
    private IGameContextManager _contextMgr;
    private GameHistory _gameHistory;

    public bool IsEnd { get{ return _gameResult != null; } }
    public GameResult GameResult { get{ return _gameResult; } }
    GameStatus IGameplayStatusWatcher.GameStatus { get{ return _gameStatus; } }
    IGameContextManager IGameplayStatusWatcher.ContextManager { get{ return _contextMgr; } }

    public GameplayManager(GameStatus initialStatus, GameContextManager contextManager)
    {
        // TODO split gamestatus and gamesnapshot and gameparams
        _gameStatus = initialStatus;
        _contextMgr = contextManager;
        _gameHistory = new GameHistory(this);
    }

    public void Start()
    {
        _gameEvents = new List<IGameEvent>();
        _gameActions = new Queue<IGameAction>();
        _gameResult = null;

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
            case GameState.GameStart:
                UpdateReactorSessionTiming(UpdateTiming.GameStart);
                _GameStart();
                _gameStatus.SetState(GameState.TurnStart);
                break;
            case GameState.TurnStart:
                UpdateReactorSessionTiming(UpdateTiming.TurnStart);
                _TurnStart();
                _gameStatus.SetState(GameState.DrawCard);
                break;
            case GameState.DrawCard:
                UpdateReactorSessionTiming(UpdateTiming.DrawCard);
                _TurnDrawCard();
                _gameStatus.SetState(GameState.EnemyPrepare);
                break;
            case GameState.EnemyPrepare:     
                _EnemyPreapre();
                _gameStatus.SetState(GameState.PlayerPrepare);
                break;
            case GameState.PlayerPrepare:
                _PlayerPrepare();
                _gameStatus.SetState(GameState.PlayerExecute);
                break;
            case GameState.PlayerExecute:
                _PlayerExecute();
                break;
            case GameState.Enemy_Execute:
                _EnemyExecute();
                break;
            case GameState.TurnEnd:
                UpdateReactorSessionTiming(UpdateTiming.TurnEnd);
                _TurnEnd();
                _gameStatus.SetState(GameState.TurnStart);
                break;
            case GameState.GameEnd:
                break;
        }
    }

    private void _GameStart()
    {
        // TODO: summon characters 
        _gameEvents.Add(new AllySummonEvent() {
            Player = _gameStatus.Ally 
        });
        _gameEvents.Add(new EnemySummonEvent() {
            Enemy = _gameStatus.Enemy 
        });
    }

    private void _TurnStart()
    {
        _gameStatus.SetNewTurn();
        _gameEvents.Add(new RoundStartEvent(){
            Round = _gameStatus.TurnCount,
            Player = _gameStatus.Ally,
            Enemy = _gameStatus.Enemy
        });
        
        var recoverEnergyPoint = _contextMgr.DispositionLibrary.GetRecoverEnergyPoint(_gameStatus.Ally.DispositionManager.CurrentDisposition);
        var allyGainEnergyResult = _gameStatus.Ally.EnergyManager.RecoverEnergy(recoverEnergyPoint);
        _gameEvents.Add(new GainEnergyEvent(_gameStatus.Ally, allyGainEnergyResult));

        var enemyGainEnergyResult = _gameStatus.Enemy.EnergyManager.RecoverEnergy(_gameStatus.Enemy.EnergyRecoverPoint);
        _gameEvents.Add(new GainEnergyEvent(_gameStatus.Enemy, enemyGainEnergyResult));
    }

    private void _TurnDrawCard()
    {
        var allyDrawCount = _contextMgr.DispositionLibrary.GetDrawCardCount(_gameStatus.Ally.DispositionManager.CurrentDisposition);
        var enemyDrawCount = _gameStatus.Enemy.TurnStartDrawCardCount;

        var allyDrawEvents = EffectExecutor.DrawCards(this, this, new SystemSource(), _gameStatus.Ally, allyDrawCount);
        _gameEvents.AddRange(allyDrawEvents);

        var enemyDrawEvents = EffectExecutor.DrawCards(this, this, new SystemSource(), _gameStatus.Enemy, enemyDrawCount);
        _gameEvents.AddRange(enemyDrawEvents);

        var triggerEvts = _TriggerTiming(TriggerTiming.DrawCard, new SystemSource());
        _gameEvents.AddRange(triggerEvts);

        _gameActions.Clear();
    }

    private void _EnemyPreapre()
    {
        var recommendCards = _gameStatus.Enemy.GetRecommendCards();
        foreach(var card in recommendCards)
        {
            if(_gameStatus.Enemy.SelectedCards.TryEnqueueCard(card))
            {
                _gameEvents.Add(new EnemySelectCardEvent(){
                    SelectedCardInfo = new CardInfo(card, this),
                    SelectedCardInfos = _gameStatus.Enemy.SelectedCards.Cards.ToCardInfos(this)
                });
            }
        }
    }

    private void _PlayerPrepare()
    {
        _gameEvents.Add(new PlayerExecuteStartEvent() {
            Faction = _gameStatus.Ally.Faction,
            HandCardInfo = _gameStatus.Ally.CardManager.HandCard.ToCardCollectionInfo(this)
        });
    }
    public void _PlayerExecute()
    {
        _TurnExecute(_gameStatus.Ally);
    }
    private void _EnemyExecute()
    {
        UpdateReactorSessionTiming(UpdateTiming.ExecuteStart);
        
        while (_gameStatus.Enemy.SelectedCards.TryDequeueCard(out ICardEntity selectedCard))
        {
            var cardRuntimCost = selectedCard.EvalCost(this);
            if (cardRuntimCost <= _gameStatus.Enemy.CurrentEnergy)
            {
                _gameActions.Enqueue(new UseCardAction(selectedCard.Identity));
                _TurnExecute(_gameStatus.Enemy);
            }
            else
            {
                _gameEvents.Add(new EnemyUnselectedCardEvent()
                {
                    SelectedCardInfo = new CardInfo(selectedCard, this),
                    SelectedCardInfos = _gameStatus.Enemy.SelectedCards.Cards.ToCardInfos(this)
                });
            }
        }
        
        _FinishEnemyExecuteTurn();
    }
    private void _TurnExecute(IPlayerEntity player)
    {
        while(_gameActions.Count > 0)
        {
            var action = _gameActions.Dequeue();
            switch(action)
            {
                case UseCardAction useCardAction:
                    using(_SetUseCardSelectTarget(useCardAction))
                    {
                        _UseCard(player, useCardAction.CardIndentity);
                        _gameEvents.Add(new PlayerExecuteStartEvent(){
                            Faction = player.Faction,
                            HandCardInfo = player.CardManager.HandCard.ToCardCollectionInfo(this)
                        });
                    }
                    break;
                case TurnSubmitAction turnSubmitAction:
                    if (_gameStatus.State == GameState.PlayerExecute &&
                        turnSubmitAction.Faction == Faction.Ally)
                    {
                        _FinishPlayerExecuteTurn();
                    }
                    break;
            }
        }
    }

    private void _FinishPlayerExecuteTurn()
    {
        _gameEvents.Add(new PlayerExecuteEndEvent(){
            Faction = _gameStatus.Ally.Faction,
            HandCardInfo = _gameStatus.Ally.CardManager.HandCard.ToCardCollectionInfo(this)
        });

        var endTurnSource = new SystemExectueEndSource(_gameStatus.Ally);
        var triggerEvts = _TriggerTiming(TriggerTiming.ExecuteEnd, endTurnSource);
        _gameEvents.AddRange(triggerEvts);

        UpdateReactorSessionTiming(UpdateTiming.ExecuteEnd);

        _gameStatus.SetState(GameState.Enemy_Execute);
        _gameActions.Clear();
    }   
    private void _FinishEnemyExecuteTurn()
    {
        var endTurnSource = new SystemExectueEndSource(_gameStatus.Enemy);
        var triggerEvts = _TriggerTiming(TriggerTiming.ExecuteEnd, endTurnSource);
        _gameEvents.AddRange(triggerEvts);

        UpdateReactorSessionTiming(UpdateTiming.ExecuteEnd);

        _gameStatus.SetState(GameState.TurnEnd);
        _gameActions.Clear();
    }

    private void _TurnEnd()
    {
        _gameEvents.AddRange(
            _gameStatus.Ally.CardManager.ClearHandOnTurnEnd(this));
        _gameEvents.AddRange(
            _gameStatus.Enemy.CardManager.ClearHandOnTurnEnd(this));

        UpdateReactorSessionTiming(UpdateTiming.TurnEnd);

        var triggerEvts = _TriggerTiming(TriggerTiming.TurnEnd, new SystemSource());
        _gameEvents.AddRange(triggerEvts);
    }

    private GameContextManager _SetUseCardSelectTarget(UseCardAction useCardAction)
    {
        switch(useCardAction.TargetType)
        { 
            case TargetType.AllyCharacter:
                return useCardAction.SelectedTarget.Match(
                    allyCharacterIdentity => {
                        var allyCharacterOpt = _gameStatus.GetAllyCharacter(allyCharacterIdentity);
                        return allyCharacterOpt.Match(
                            allyCharacter => _contextMgr.SetSelectedCharacter(allyCharacter),
                            () => _contextMgr.SetClone()
                        );
                    },
                    () => _contextMgr.SetClone());
            case TargetType.EnemyCharacter:
                return useCardAction.SelectedTarget.Match(
                    enemyCharacterIdentity => {
                        var enemyCharacterOpt = _gameStatus.GetEnemyCharacter(enemyCharacterIdentity);
                        return enemyCharacterOpt.Match(
                            enemyCharacter => _contextMgr.SetSelectedCharacter(enemyCharacter),
                            () => _contextMgr.SetClone()
                        );
                    },
                    () => _contextMgr.SetClone());
            case TargetType.AllyCard:
                return useCardAction.SelectedTarget.Match(
                    allyCardIndentity => {                        
                        var allyCardOpt = _gameStatus.Ally.CardManager.GetCard(allyCardIndentity);
                        return allyCardOpt.Match(
                            allyCard => _contextMgr.SetSelectedCard(allyCard),
                            () => _contextMgr.SetClone()
                        );
                    },
                    () => _contextMgr.SetClone());
            case TargetType.EnemyCard:
                return useCardAction.SelectedTarget.Match(
                    enemyCardIndentity => {
                        var enemyCardOpt = _gameStatus.Enemy.CardManager.GetCard(enemyCardIndentity);
                        return enemyCardOpt.Match(
                            enemyCard => _contextMgr.SetSelectedCard(enemyCard),
                            () => _contextMgr.SetClone()
                        );
                    },
                    () => _contextMgr.SetClone());
            default:
            case TargetType.None:
                return _contextMgr.SetClone();      
        }
    }
    private void _UseCard(IPlayerEntity player, Guid CardIndentity)
    {
        var usedCard = player.CardManager.HandCard.Cards.FirstOrDefault(c => c.Identity == CardIndentity);
        if (usedCard != null &&
            !usedCard.HasProperty(CardProperty.Sealed))
        {
            var useCardEvents = new List<IGameEvent>();
            var cardRuntimCost = usedCard.EvalCost(this);
            if (cardRuntimCost <= player.CurrentEnergy) 
            {
                var loseEnergyResult = player.EnergyManager.ConsumeEnergy(cardRuntimCost);
                useCardEvents.Add(new LoseEnergyEvent(player, loseEnergyResult));

                if (player.CardManager.TryPlayCard(usedCard, out CardPlaySource cardPlaySource))
                {
                    var cardPlayTrigger = new CardPlayTrigger(cardPlaySource);

                    // Create PlayCardSession
                    UpdateReactorSessionTiming(UpdateTiming.PlayCardStart);

                    UpdateReactorSessionAction(new CardPlayIntentAction(cardPlaySource));

                    useCardEvents.AddRange(_TriggerTiming(TriggerTiming.PlayCardStart, cardPlaySource));

                    foreach (var effect in usedCard.Effects)
                    {
                        var applyCardEvents = EffectExecutor.ApplyCardEffect(
                            this,
                            this,
                            cardPlaySource,
                            cardPlayTrigger,
                            effect);
                        useCardEvents.AddRange(applyCardEvents);
                    }

                    player.CardManager.EndPlayCard();

                    var usedCardInfo = new CardInfo(usedCard, this);
                    var usedCardEvent = new UsedCardEvent()
                    {
                        Faction = player.Faction,
                        UsedCardInfo = usedCardInfo,
                        HandCardInfo = player.CardManager.HandCard.ToCardCollectionInfo(this),
                        GraveyardInfo = player.CardManager.Graveyard.ToCardCollectionInfo(this)
                    };
                    useCardEvents.Add(usedCardEvent);

                    useCardEvents.AddRange(_TriggerTiming(TriggerTiming.PlayCardEnd, cardPlaySource));

                    // Close PlayCardSession
                    UpdateReactorSessionTiming(UpdateTiming.PlayCardEnd);
                }
            }

            OnUseCard?.Invoke(); // pass record to History

            _gameEvents.AddRange(useCardEvents);
        }
    }

    public void UpdateReactorSessionTiming(UpdateTiming updateTiming)
    {
        var timingAction = new UpdateTimingAction(updateTiming);
        _gameStatus.Ally.Update(this, timingAction);
        _gameStatus.Enemy.Update(this, timingAction);
    }

    public void UpdateReactorSessionAction(IActionUnit actionUnit)
    {
        _gameStatus.Ally.Update(this, actionUnit);
        _gameStatus.Enemy.Update(this, actionUnit);
    }

    // TODO: collect reactionEffects created from reactionSessions
    private IEnumerable<IGameEvent> _TriggerTiming(TriggerTiming timing, IActionSource actionSource)
    {
        var triggerBuffEvents = new List<IGameEvent>();
        var timingAction = new TriggerTimingAction(timing, actionSource);
        foreach (var buff in _gameStatus.Ally.BuffManager.Buffs)
        {
            var buffTrigger = new PlayerBuffTrigger(buff);
            var conditionalEffectsOpt = _contextMgr.BuffLibrary.GetBuffEffects(buff.PlayerBuffDataId, timing);
            conditionalEffectsOpt.MatchSome(conditionalEffects =>
            {
                foreach (var conditionalEffect in conditionalEffects)
                {
                    if (conditionalEffect.Conditions.All(c => c.Eval(this, buffTrigger, timingAction)))
                    {
                        var applyEvts = EffectExecutor.ApplyPlayerBuffEffect(
                            this, this, actionSource, buffTrigger, conditionalEffect.Effect);
                        triggerBuffEvents.AddRange(applyEvts);

                        var nextTriggerSource = new PlayerBuffSource(buff);
                        var triggerEndEvents = _TriggerTiming(TriggerTiming.TriggerBuffEnd, nextTriggerSource);
                        triggerBuffEvents.AddRange(triggerEndEvents);
                    }
                }
            });
        }

        foreach(var character in _gameStatus.Ally.Characters)
        { 
        }

        foreach(var card in _gameStatus.Ally.CardManager.HandCard.Cards)
        { 
        }

        foreach(var buff in _gameStatus.Enemy.BuffManager.Buffs)
        {
            var buffTrigger = new PlayerBuffTrigger(buff);
            var conditionalEffectsOpt = _contextMgr.BuffLibrary.GetBuffEffects(buff.PlayerBuffDataId, timing);

            conditionalEffectsOpt.MatchSome(conditionalEffects => 
            {
                foreach(var conditionalEffect in conditionalEffects)
                {
                    if (conditionalEffect.Conditions.All(c => c.Eval(this, buffTrigger, timingAction)))
                    {
                        var applyEvts = EffectExecutor.ApplyPlayerBuffEffect(
                            this, this, actionSource, buffTrigger, conditionalEffect.Effect);
                        triggerBuffEvents.AddRange(applyEvts);

                        var nextTriggerSource = new PlayerBuffSource(buff);
                        var triggerEndEvents = _TriggerTiming(TriggerTiming.TriggerBuffEnd, nextTriggerSource);
                        triggerBuffEvents.AddRange(triggerEndEvents);
                    }
                }
            });
        }

        foreach(var character in _gameStatus.Enemy.Characters)
        { 
        }

        foreach(var card in _gameStatus.Enemy.CardManager.HandCard.Cards)
        { 
        }
        
        return triggerBuffEvents;
    }
}

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
    IEnumerable<IGameEvent> UpdateReactorSessionAction(IActionUnit actionUnit);
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
    private Option<BattleResult> _battleResult;
    private List<IGameEvent> _gameEvents;
    private Queue<IGameAction> _gameActions;
    private IGameContextManager _contextMgr;
    private GameHistory _gameHistory;

    public Option<BattleResult> BattleResult { get { return _battleResult; } }
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
        _battleResult = Option.None<BattleResult>();

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
        switch (gameStatus.State)
        {
            case GameState.GameStart:
                _gameEvents.AddRange(
                    UpdateReactorSessionAction(new UpdateTimingAction(GameTiming.GameStart, new SystemSource())));
                _GameStart();
                _gameStatus.SetState(GameState.TurnStart);
                break;
            case GameState.TurnStart:
                _gameEvents.AddRange(
                    UpdateReactorSessionAction(new UpdateTimingAction(GameTiming.TurnStart, new SystemSource())));
                _TurnStart();
                _gameStatus.SetState(GameState.DrawCard);
                break;
            case GameState.DrawCard:
                _gameEvents.AddRange(
                    UpdateReactorSessionAction(new UpdateTimingAction(GameTiming.DrawCard, new SystemSource())));
                _TurnDrawCard();
                _gameStatus.SetState(GameState.EnemyPrepare);
                break;
            case GameState.EnemyPrepare:
                _EnemyPrepare();
                _gameStatus.SetState(GameState.PlayerPrepare);
                break;
            case GameState.PlayerPrepare:
                _PlayerPrepare();
                _gameStatus.SetState(GameState.PlayerExecute);
                break;
            case GameState.PlayerExecute:
                _PlayerExecute();
                break;
            case GameState.EnemyExecute:
                _EnemyExecute();
                break;
            case GameState.TurnEnd:
                _gameEvents.AddRange(
                    UpdateReactorSessionAction(new UpdateTimingAction(GameTiming.TurnEnd, new SystemSource())));
                _TurnEnd();
                _gameStatus.SetState(GameState.TurnStart);
                break;
            case GameState.GameEnd:
                break;
        }

        if (_IsGameEnd())
        {
            _gameStatus.SetState(GameState.GameEnd);
        }
    }

    private void _GameStart()
    {
        // TODO: summon characters 
        _gameEvents.Add(new AllySummonEvent(Player: _gameStatus.Ally));
        _gameEvents.Add(new EnemySummonEvent(Enemy: _gameStatus.Enemy));
    }

    private void _TurnStart()
    {
        _gameStatus.SetNewTurn();
        _gameEvents.Add(new RoundStartEvent(
            Round: _gameStatus.TurnCount,
            Player: _gameStatus.Ally,
            Enemy: _gameStatus.Enemy
        ));
        
        var recoverEnergyPoint = _contextMgr.DispositionLibrary.GetRecoverEnergyPoint(_gameStatus.Ally.DispositionManager.CurrentDisposition);
        var allyGainEnergyResult = _gameStatus.Ally.EnergyManager.RecoverEnergy(recoverEnergyPoint);
        _gameEvents.Add(new GainEnergyEvent(_gameStatus.Ally.Faction, _gameStatus.Ally.EnergyManager.ToInfo(), allyGainEnergyResult));

        var enemyGainEnergyResult = _gameStatus.Enemy.EnergyManager.RecoverEnergy(_gameStatus.Enemy.EnergyRecoverPoint);
        _gameEvents.Add(new GainEnergyEvent(_gameStatus.Enemy.Faction, _gameStatus.Enemy.EnergyManager.ToInfo(), enemyGainEnergyResult));
    }

    private void _TurnDrawCard()
    {
        var allyDrawCount = _contextMgr.DispositionLibrary.GetDrawCardCount(_gameStatus.Ally.DispositionManager.CurrentDisposition);
        var enemyDrawCount = _gameStatus.Enemy.TurnStartDrawCardCount;

        var allyDrawEvents = EffectExecutor.DrawCards(this, this, SystemSource.Instance, _gameStatus.Ally, allyDrawCount);
        _gameEvents.AddRange(allyDrawEvents.Events);

        var enemyDrawEvents = EffectExecutor.DrawCards(this, this, SystemSource.Instance, _gameStatus.Enemy, enemyDrawCount);
        _gameEvents.AddRange(enemyDrawEvents.Events);

        var triggerEvts = _TriggerTiming(GameTiming.DrawCard, SystemSource.Instance);
        _gameEvents.AddRange(triggerEvts);

        _gameActions.Clear();
    }

    private void _EnemyPrepare()
    {
        while (_gameStatus.Enemy.TryGetRecommandSelectCard(this, out var recommendCard))
        {
            _gameEvents.Add(new EnemySelectCardEvent(
                SelectedCardInfo: recommendCard.ToInfo(this),
                SelectedCardInfos: _gameStatus.Enemy.SelectedCards.Cards.ToCardInfos(this)
            ));
        }
    }

    private void _PlayerPrepare()
    {
        _gameEvents.Add(new PlayerExecuteStartEvent(
            Faction: _gameStatus.Ally.Faction,
            HandCardInfo: _gameStatus.Ally.CardManager.HandCard.ToCardCollectionInfo(this),
            GraveyardInfo: _gameStatus.Ally.CardManager.Graveyard.ToCardCollectionInfo(this),
            ExclusionZoneInfo: _gameStatus.Ally.CardManager.ExclusionZone.ToCardCollectionInfo(this),
            DisposeZoneInfo: _gameStatus.Ally.CardManager.DisposeZone.ToCardCollectionInfo(this)
        ));
    }
    public void _PlayerExecute()
    {
        _TurnExecute(_gameStatus.Ally);
    }
    private void _EnemyExecute()
    {
        _gameEvents.AddRange(
            UpdateReactorSessionAction(new UpdateTimingAction(GameTiming.ExecuteStart, new SystemSource())));

        while (_gameStatus.Enemy.TryGetNextUseCardAction(this, out var useCardAction))
        {
            _gameActions.Enqueue(useCardAction);
            _TurnExecute(_gameStatus.Enemy);
        }

        var unselectedCards = _gameStatus.Enemy.SelectedCards.UnSelectAllCards();
        _gameEvents.Add(new EnemyUnselectedCardEvent(UnselectedCardInfos: unselectedCards.ToCardInfos(this)));
        
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
                        _gameEvents.Add(new PlayerExecuteStartEvent(
                            Faction: player.Faction,
                            HandCardInfo: player.CardManager.HandCard.ToCardCollectionInfo(this),
                            GraveyardInfo: player.CardManager.Graveyard.ToCardCollectionInfo(this),
                            ExclusionZoneInfo: player.CardManager.ExclusionZone.ToCardCollectionInfo(this),
                            DisposeZoneInfo: player.CardManager.DisposeZone.ToCardCollectionInfo(this)
                        ));
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
        _gameEvents.Add(new PlayerExecuteEndEvent(
            Faction: _gameStatus.Ally.Faction,
            HandCardInfo: _gameStatus.Ally.CardManager.HandCard.ToCardCollectionInfo(this)
        ));

        var endTurnSource = new SystemExectueEndSource(_gameStatus.Ally);
        var triggerEvts = _TriggerTiming(GameTiming.ExecuteEnd, endTurnSource);
        _gameEvents.AddRange(triggerEvts);
        
        _gameEvents.AddRange(
            UpdateReactorSessionAction(new UpdateTimingAction(GameTiming.ExecuteEnd, new SystemSource())));

        _gameStatus.SetState(GameState.EnemyExecute);
        _gameActions.Clear();
    }   
    private void _FinishEnemyExecuteTurn()
    {
        var endTurnSource = new SystemExectueEndSource(_gameStatus.Enemy);
        var triggerEvts = _TriggerTiming(GameTiming.ExecuteEnd, endTurnSource);
        _gameEvents.AddRange(triggerEvts);

        _gameEvents.AddRange(
            UpdateReactorSessionAction(new UpdateTimingAction(GameTiming.ExecuteEnd, new SystemSource())));

        _gameStatus.SetState(GameState.TurnEnd);
        _gameActions.Clear();
    }

    private void _TurnEnd()
    {
        _gameEvents.AddRange(
            _gameStatus.Ally.CardManager.ClearHandOnTurnEnd(this));
        _gameEvents.AddRange(
            _gameStatus.Enemy.CardManager.ClearHandOnTurnEnd(this));

        _gameEvents.AddRange(
            UpdateReactorSessionAction(new UpdateTimingAction(GameTiming.TurnEnd, new SystemSource())));

        var triggerEvts = _TriggerTiming(GameTiming.TurnEnd, SystemSource.Instance);
        _gameEvents.AddRange(triggerEvts);
    }

    private GameContextManager _SetUseCardSelectTarget(UseCardAction useCardAction)
    {
        switch(useCardAction.TargetType)
        {
            case TargetType.AllyCharacter:
            case TargetType.EnemyCharacter:
                var enemyCharacterOpt = useCardAction.SelectedTarget
                    .FlatMap(enemyCharacterIdentity => this.GetCharacter(enemyCharacterIdentity));
                return _contextMgr.SetSelectedCharacter(enemyCharacterOpt);
            case TargetType.AllyCard:
            case TargetType.EnemyCard:
                var enemyCardOpt = useCardAction.SelectedTarget
                    .FlatMap(enemyCardIndentity => this.GetCard(enemyCardIndentity));
                return _contextMgr.SetSelectedCard(enemyCardOpt);
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
            
            var cardRuntimCost = GameFormula.CardCost(this, usedCard, new CardLookIntentAction(usedCard));
            if (cardRuntimCost <= player.CurrentEnergy) 
            {
                var loseEnergyResult = player.EnergyManager.ConsumeEnergy(cardRuntimCost);
                useCardEvents.Add(new LoseEnergyEvent(player.Faction, player.EnergyManager.ToInfo(), loseEnergyResult));

                if (player.CardManager.TryPlayCard(usedCard, out int handCardIndex, out int handCardsCount))
                {
                    var cardPlaySource = new CardPlaySource(usedCard, handCardIndex, handCardsCount, loseEnergyResult, new CardPlayAttributeEntity());
                    var cardPlayTrigger = new CardPlayTrigger(cardPlaySource);

                    // Create PlayCardSession
                    useCardEvents.AddRange(
                        UpdateReactorSessionAction(new UpdateTimingAction(GameTiming.PlayCardStart, new SystemSource())));

                    useCardEvents.AddRange(
                        UpdateReactorSessionAction(new CardPlayIntentAction(cardPlaySource)));

                    //TODO: check and remove expired buffs
                    //      trigger events while remove buffs

                    useCardEvents.AddRange(_TriggerTiming(GameTiming.PlayCardStart, cardPlaySource));

                    var effectActionResults = new List<BaseResultAction>();
                    foreach (var effect in usedCard.Effects)
                    {
                        var effectResult = EffectExecutor.ApplyCardEffect(
                            this,
                            this,
                            cardPlaySource,
                            cardPlayTrigger,
                            effect);
                        useCardEvents.AddRange(effectResult.Events);
                        effectActionResults.AddRange(effectResult.ResultActions);
                    }
                    var cardPlayResultSource = cardPlaySource.CreateResultSource(effectActionResults);                   

                    player.CardManager.EndPlayCard();

                    var usedCardInfo = usedCard.ToInfo(this);
                    var usedCardEvent = new UsedCardEvent(
                        Faction: player.Faction,
                        UsedCardInfo: usedCardInfo,
                        HandCardInfo: player.CardManager.HandCard.ToCardCollectionInfo(this),
                        GraveyardInfo: player.CardManager.Graveyard.ToCardCollectionInfo(this),
                        ExclusionZoneInfo: player.CardManager.ExclusionZone.ToCardCollectionInfo(this),
                        DisposeZoneInfo: player.CardManager.DisposeZone.ToCardCollectionInfo(this)
                    );
                    useCardEvents.Add(usedCardEvent);

                    useCardEvents.AddRange(_TriggerTiming(GameTiming.PlayCardEnd, cardPlayResultSource));

                    useCardEvents.AddRange(
                        UpdateReactorSessionAction(new CardPlayResultAction(cardPlayResultSource)));

                    // Close PlayCardSession
                    useCardEvents.AddRange(
                        UpdateReactorSessionAction(new UpdateTimingAction(GameTiming.PlayCardEnd, new SystemSource())));
                }
            }

            OnUseCard?.Invoke(); // pass record to History

            _gameEvents.AddRange(useCardEvents);
        }
    }

    public IEnumerable<IGameEvent> UpdateReactorSessionAction(IActionUnit actionUnit)
    {
        var allyEvt = _gameStatus.Ally.Update(this, actionUnit);
        var enemyEvt = _gameStatus.Enemy.Update(this, actionUnit);

        return new List<IGameEvent> { allyEvt, enemyEvt };
    }

    // TODO: collect reactionEffects created from reactionSessions
    private IEnumerable<IGameEvent> _TriggerTiming(GameTiming timing, IActionSource actionSource)
    {
        var triggerBuffEvents = new List<IGameEvent>();
        var timingAction = new UpdateTimingAction(timing, actionSource);
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
                        triggerBuffEvents.AddRange(applyEvts.Events);

                        var nextTriggerSource = new PlayerBuffSource(buff);
                        var triggerEndEvents = _TriggerTiming(GameTiming.TriggerBuffEnd, nextTriggerSource);
                        triggerBuffEvents.AddRange(triggerEndEvents);
                    }
                }
            });
        }

        foreach (var character in _gameStatus.Ally.Characters)
        {
        }

        foreach (var card in _gameStatus.Ally.CardManager.HandCard.Cards)
        {
        }

        foreach (var buff in _gameStatus.Enemy.BuffManager.Buffs)
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
                        triggerBuffEvents.AddRange(applyEvts.Events);

                        var nextTriggerSource = new PlayerBuffSource(buff);
                        var triggerEndEvents = _TriggerTiming(GameTiming.TriggerBuffEnd, nextTriggerSource);
                        triggerBuffEvents.AddRange(triggerEndEvents);
                    }
                }
            });
        }

        foreach (var character in _gameStatus.Enemy.Characters)
        {
        }

        foreach (var card in _gameStatus.Enemy.CardManager.HandCard.Cards)
        {
        }

        return triggerBuffEvents;
    }
    
    private bool _IsGameEnd()
    {
        if (_gameStatus.Ally.IsDead)
        {
            _battleResult = new BattleResult(false).Some();
            return true;
        }
        else if (_gameStatus.Enemy.IsDead)
        {
            _battleResult = new BattleResult(true).Some();
            return true;
        }
        return false;
    }
}

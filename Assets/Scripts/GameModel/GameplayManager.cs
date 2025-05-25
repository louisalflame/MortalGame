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
    void UpdateTiming(UpdateTiming updateTiming);
    void UpdateAction(IActionUnit actionUnit);
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
                UpdateTiming(global::UpdateTiming.GameStart);
                _GameStart();
                _gameStatus.SetState(GameState.TurnStart);
                break;
            case GameState.TurnStart:
                UpdateTiming(global::UpdateTiming.TurnStart);
                _TurnStart();
                _gameStatus.SetState(GameState.DrawCard);
                break;
            case GameState.DrawCard:
                UpdateTiming(global::UpdateTiming.DrawCard);
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
                UpdateTiming(global::UpdateTiming.TurnEnd);
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

        var triggerEvts = _TriggerTiming(TriggerTiming.DrawCard);
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
        while(_gameStatus.Enemy.SelectedCards.TryDequeueCard(out ICardEntity selectedCard))
        {
            var cardRuntimCost = selectedCard.EvalCost(this);
            if (cardRuntimCost <= _gameStatus.Enemy.CurrentEnergy)
            {
                _gameActions.Enqueue(new UseCardAction(selectedCard.Identity));
                _TurnExecute(_gameStatus.Enemy);
            }
            else
            {
                _gameEvents.Add(new EnemyUnselectedCardEvent(){
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

        UpdateTiming(global::UpdateTiming.ExecuteEnd);
        var triggerEvts = _TriggerTiming(TriggerTiming.ExecuteEnd);
        _gameEvents.AddRange(triggerEvts);

        _gameStatus.SetState(GameState.Enemy_Execute);
        _gameActions.Clear();
    }   
    private void _FinishEnemyExecuteTurn()
    {
        UpdateTiming(global::UpdateTiming.ExecuteEnd);
        var triggerEvts = _TriggerTiming(TriggerTiming.ExecuteEnd);
        _gameEvents.AddRange(triggerEvts);

        _gameStatus.SetState(GameState.TurnEnd);
        _gameActions.Clear();
    }

    private void _TurnEnd()
    {
        _gameEvents.AddRange(
            _gameStatus.Ally.CardManager.ClearHandOnTurnEnd(this));
        _gameEvents.AddRange(
            _gameStatus.Enemy.CardManager.ClearHandOnTurnEnd(this));

        UpdateTiming(global::UpdateTiming.TurnEnd);
        var triggerEvts = _TriggerTiming(TriggerTiming.TurnEnd);
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

                if (player.CardManager.HandCard.TryRemoveCard(usedCard, out int handCardIndex, out int handCardsCount))
                {
                    var cardPlaySource = new CardPlaySource(usedCard, handCardIndex, handCardsCount);
                    var cardPlayTrigger = new CardPlayTrigger(cardPlaySource);

                    // Create PlayCardSession
                    UpdateTiming(global::UpdateTiming.PlayCardStart);

                    UpdateAction(new CardPlayIntentAction(cardPlaySource));

                    var triggerPlayCardStartEvts = _TriggerTiming(TriggerTiming.PlayCardStart);
                    useCardEvents.AddRange(triggerPlayCardStartEvts);

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

                    ICardColletionZone destination =
                        (usedCard.HasProperty(CardProperty.Dispose) || usedCard.HasProperty(CardProperty.AutoDispose)) ?
                        player.CardManager.ExclusionZone : player.CardManager.Graveyard;
                    destination.AddCard(usedCard);

                    var usedCardInfo = new CardInfo(usedCard, this);
                    var usedCardEvent = new UsedCardEvent()
                    {
                        Faction = player.Faction,
                        UsedCardInfo = usedCardInfo,
                        HandCardInfo = player.CardManager.HandCard.ToCardCollectionInfo(this),
                        GraveyardInfo = player.CardManager.Graveyard.ToCardCollectionInfo(this)
                    };
                    useCardEvents.Add(usedCardEvent);

                    // Close PlayCardSession
                    UpdateTiming(global::UpdateTiming.PlayCardEnd);

                    var triggerPlayCardEndEvts = _TriggerTiming(TriggerTiming.PlayCardEnd);
                    useCardEvents.AddRange(triggerPlayCardEndEvts);
                }
            }

            OnUseCard?.Invoke(); // pass record to History

            _gameEvents.AddRange(useCardEvents);
        }
    }

    public void UpdateTiming(UpdateTiming updateTiming)
    {
        var timingAction = new UpdateTimingAction(updateTiming);
        _gameStatus.Ally.Update(this, timingAction);
        _gameStatus.Enemy.Update(this, timingAction);
    }

    public void UpdateAction(IActionUnit actionUnit)
    {
        _gameStatus.Ally.Update(this, actionUnit);
        _gameStatus.Enemy.Update(this, actionUnit);
    }

    // TODO: collect reactionEffects created from reactionSessions
    private IEnumerable<IGameEvent> _TriggerTiming(TriggerTiming timing)
    {
        var triggerBuffEvents = new List<IGameEvent>();
        var timingAction = new TriggerTimingAction(timing);
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
                        var applySource = new PlayerBuffSource(buff);
                        var applyEvts = _ApplyBuffEffect(applySource, buffTrigger, conditionalEffect.Effect);
                        triggerBuffEvents.AddRange(applyEvts);
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
                        var applySource = new PlayerBuffSource(buff);
                        var applyEvts = _ApplyBuffEffect(applySource, buffTrigger, conditionalEffect.Effect);
                        triggerBuffEvents.AddRange(applyEvts);
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

    // TODO: apply character buff <-> player buff
    private IEnumerable<IGameEvent> _ApplyBuffEffect(
        IActionSource actionSource, ITriggerSource triggerSource, IPlayerBuffEffect buffEffect)
    {
        var appleBuffEffectEvents = new List<IGameEvent>();
        UpdateTiming(global::UpdateTiming.TriggerBuffStart);

        switch(buffEffect)
        {
            case EffectiveDamagePlayerBuffEffect effectiveDamageBuffEffect:
            {
                var intent = new DamageIntentAction(actionSource, DamageType.Effective);
                var targets = effectiveDamageBuffEffect.Targets.Eval(this, triggerSource, intent);
                foreach(var target in targets)
                {
                    var characterTarget = new CharacterTarget(target);
                    var targetIntent = new DamageIntentTargetAction(actionSource, characterTarget, DamageType.Effective);
                    var damagePoint = effectiveDamageBuffEffect.Value.Eval(this, triggerSource, targetIntent);
                    var damageResult = target.HealthManager.TakeEffectiveDamage(damagePoint, _contextMgr.Context);
                    var damageStyle = DamageStyle.None;

                    UpdateAction(new DamageResultAction(actionSource, characterTarget, damageResult, damageStyle));
                    appleBuffEffectEvents.Add(new DamageEvent(target.Faction(this), target, damageResult, damageStyle));
                }
                break;
            }
            case AddCardBuffPlayerBuffEffect addCardBuffPlayerBuffEffect:
            {
                var intent = new AddCardBuffIntentAction(actionSource);
                var cards = addCardBuffPlayerBuffEffect.Targets.Eval(this, triggerSource, intent);
                foreach(var card in cards)
                {
                    var cardTarget = new CardTarget(card);
                    var targetIntent = new AddCardBuffIntentTargetAction(actionSource, cardTarget);
                    foreach(var addCardBuffData in addCardBuffPlayerBuffEffect.AddCardBuffDatas)
                    {
                        var addLevel = addCardBuffData.Level.Eval(this, triggerSource, targetIntent);
                        var addResult = card.BuffManager.AddBuff(
                            _contextMgr.CardBuffLibrary,
                            this,
                            triggerSource,
                            targetIntent,
                            addCardBuffData.CardBuffId,
                            addLevel);

                        UpdateAction(new AddCardBuffResultAction(actionSource, cardTarget, addResult));
                        appleBuffEffectEvents.Add(new AddCardBuffEvent(card, this));
                    }
                }
                break;
            }
        }

        var triggerEndEvents = _TriggerTiming(TriggerTiming.TriggerBuffEnd);
        appleBuffEffectEvents.AddRange(triggerEndEvents);

        return appleBuffEffectEvents;
    }
}

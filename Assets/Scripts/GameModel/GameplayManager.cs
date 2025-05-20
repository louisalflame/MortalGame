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
    GameContext GameContext { get; }
}

public interface IGameEventWatcher : IGameplayStatusWatcher
{
    event Action OnUseCard;
    event Action OneTurnStart;
    event Action OnTurnEnd;
}

public class GameplayManager : IGameplayStatusWatcher, IGameEventWatcher
{
    public event Action OnUseCard;
    public event Action OneTurnStart;
    public event Action OnTurnEnd;

    private GameStatus _gameStatus;
    private GameResult _gameResult;
    private List<IGameEvent> _gameEvents;
    private Queue<IGameAction> _gameActions;
    private GameContextManager _contextMgr;
    private GameHistory _gameHistory;

    public bool IsEnd { get{ return _gameResult != null; } }
    public GameResult GameResult { get{ return _gameResult; } }
    GameStatus IGameplayStatusWatcher.GameStatus { get{ return _gameStatus; } }
    GameContext IGameplayStatusWatcher.GameContext { get{ return _contextMgr.Context; } }

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
                _UpdateTiming(UpdateTiming.GameStart);
                _GameStart();
                _gameStatus.SetState(GameState.TurnStart);
                break;
            case GameState.TurnStart:
                _UpdateTiming(UpdateTiming.TurnStart);
                _TurnStart();
                _gameStatus.SetState(GameState.DrawCard);
                break;
            case GameState.DrawCard:
                _UpdateTiming(UpdateTiming.DrawCard);
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
                _UpdateTiming(UpdateTiming.TurnEnd);
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

        var allyDrawEvents = _DrawCards(new SystemSource(), _gameStatus.Ally, allyDrawCount);
        _gameEvents.AddRange(allyDrawEvents);

        var enemyDrawEvents = _DrawCards(new SystemSource(), _gameStatus.Enemy, enemyDrawCount);
        _gameEvents.AddRange(enemyDrawEvents);

        _TriggerTiming(TriggerTiming.DrawCard);

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
        
        _UpdateTiming(UpdateTiming.ExecuteEnd);
        var triggerEvts = _TriggerTiming(TriggerTiming.ExecuteEnd);
        _gameEvents.AddRange(triggerEvts);

        _gameStatus.SetState(GameState.Enemy_Execute);
        _gameActions.Clear();
    }   
    private void _FinishEnemyExecuteTurn()
    {
        _UpdateTiming(UpdateTiming.ExecuteEnd);
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

        _UpdateTiming(UpdateTiming.TurnEnd);
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

                if (player.CardManager.HandCard.TryRemoveCard(usedCard, out int playCardIndex)) 
                {
                    // Create PlayCardSession
                    _UpdateTiming(UpdateTiming.PlayCardStart);

                    var cardPlaySource = new CardPlaySource(usedCard, playCardIndex);
                    _UpdateAction(new CardPlayIntentAction(cardPlaySource));

                    _TriggerTiming(TriggerTiming.PlayCardStart);

                    var trigger = new CardPlayTrigger(cardPlaySource);
                    foreach(var effect in usedCard.Effects)
                    {
                        var applyCardEvents = _ApplyCardEffect(cardPlaySource, trigger, effect);
                        useCardEvents.AddRange(applyCardEvents);
                    }
                
                    ICardColletionZone destination =  
                        (usedCard.HasProperty(CardProperty.Dispose) || usedCard.HasProperty(CardProperty.AutoDispose)) ?
                        player.CardManager.ExclusionZone : player.CardManager.Graveyard;
                    destination.AddCard(usedCard);
                    
                    var usedCardInfo = new CardInfo(usedCard, this);
                    var usedCardEvent = new UsedCardEvent() {
                        Faction = player.Faction,
                        UsedCardInfo = usedCardInfo,
                        HandCardInfo = player.CardManager.HandCard.ToCardCollectionInfo(this),
                        GraveyardInfo = player.CardManager.Graveyard.ToCardCollectionInfo(this)
                    };
                    useCardEvents.Add(usedCardEvent);

                    // Close PlayCardSession
                    _UpdateTiming(UpdateTiming.PlayCardEnd);

                    _TriggerTiming(TriggerTiming.PlayCardEnd);
                }
            }

            OnUseCard?.Invoke(); // pass record to History

            _gameEvents.AddRange(useCardEvents);
        }
    }

    private IEnumerable<IGameEvent> _DrawCards(IActionSource source, IPlayerEntity player, int drawCount)
    {
        var drawCardEvents = new List<IGameEvent>();
        for (int i = 0; i < drawCount; i++)
        {
            if( player.CardManager.Deck.Cards.Count == 0 &&
                player.CardManager.Graveyard.Cards.Count > 0)
            {
                var graveyardCards = player.CardManager.Graveyard.PopAllCards();
                player.CardManager.Deck.EnqueueCardsThenShuffle(graveyardCards);

                _UpdateAction(new RecycleDeckResultAction(new PlayerTarget(player)));
                _gameEvents.Add(new RecycleGraveyardEvent() {
                    Faction = player.Faction,
                    DeckInfo = player.CardManager.Deck.ToCardCollectionInfo(this),
                    GraveyardInfo = player.CardManager.Graveyard.ToCardCollectionInfo(this)
                });
            }

            if (player.CardManager.Deck.Cards.Count > 0)
            {
                _DrawCard(source, player).MatchSome(drawCardEvent => {
                    drawCardEvents.Add(drawCardEvent);
                });
            }
        }

        return drawCardEvents;
    }

    private Option<IGameEvent> _DrawCard(IActionSource source, IPlayerEntity player)
    {
        if (player.CardManager.Deck.PopCard(out ICardEntity newCard))
        {
            player.CardManager.HandCard.AddCard(newCard);
            _UpdateAction(new DrawCardResultAction(source, new PlayerTarget(player), newCard));

            var newCardInfo = new CardInfo(newCard, this);
            IGameEvent drawCardEvent = new DrawCardEvent(){
                Faction = player.Faction,
                NewCardInfo = newCardInfo,
                HandCardInfo = player.CardManager.HandCard.ToCardCollectionInfo(this),
                DeckInfo = player.CardManager.Deck.ToCardCollectionInfo(this)
            };
            return drawCardEvent.Some();
        }
        return Option.None<IGameEvent>();
    }

#region Formula
    private int _DamagePoint(
        int rawDamagePoint,
        IActionSource actionSource,
        IActionTarget actionTarget)
    {
        switch(actionSource)
        {
            case CardPlaySource cardSource:
                var playerBuffAttackIncrease = _gameStatus.CurrentPlayer
                    .Map(player => player.GetPlayerBuffProperty(this, PlayerBuffProperty.Attack))
                    .ValueOr(0);

                return rawDamagePoint + playerBuffAttackIncrease;
            default:
                return rawDamagePoint;
        }
    }
#endregion

    private IEnumerable<IGameEvent> _ApplyCardEffect(
        IActionSource actionSource, ITriggerSource trigger, ICardEffect cardEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        switch(cardEffect)
        {
            case DamageEffect damageEffect:
            {
                var intent = new DamageIntentAction(actionSource, DamageType.Normal);
                var targets = damageEffect.Targets.Eval(this, trigger, intent);
                foreach(var target in targets)
                {
                    var characterTarget = new CharacterTarget(target);
                    var targetIntent = new DamageIntentTargetAction(actionSource, characterTarget, DamageType.Normal);
                    var damagePoint = damageEffect.Value.Eval(this, trigger, targetIntent);
                    damagePoint = _DamagePoint(damagePoint, actionSource, characterTarget);

                    var damageResult = target.HealthManager.TakeDamage(damagePoint, _contextMgr.Context);
                    var damageStyle = DamageStyle.None; // TODO: pass style from action source

                    _UpdateAction(new DamageResultAction(actionSource, characterTarget, damageResult, damageStyle));
                    cardEffectEvents.Add(new DamageEvent(target.Faction(this), target, damageResult, damageStyle));
                }
                break;
            }
            case PenetrateDamageEffect penetrateDamageEffect:
            {
                var intent = new DamageIntentAction(actionSource, DamageType.Penetrate);
                var targets = penetrateDamageEffect.Targets.Eval(this, trigger, intent);
                foreach(var target in targets)
                {
                    var characterTarget = new CharacterTarget(target);
                    var targetIntent = new DamageIntentTargetAction(actionSource, characterTarget, DamageType.Penetrate);
                    var damagePoint = penetrateDamageEffect.Value.Eval(this, trigger, targetIntent);
                    var damageResult = target.HealthManager.TakePenetrateDamage(damagePoint, _contextMgr.Context);
                    var damageStyle = DamageStyle.None;

                    _UpdateAction(new DamageResultAction(actionSource, characterTarget, damageResult, damageStyle));
                    cardEffectEvents.Add(new DamageEvent(target.Faction(this), target, damageResult, damageStyle));
                }
                break;
            }
            case AdditionalAttackEffect additionalAttackEffect:
            {
                var intent = new DamageIntentAction(actionSource, DamageType.Additional);
                var targets = additionalAttackEffect.Targets.Eval(this, trigger, intent);
                foreach(var target in targets)
                {
                    var characterTarget = new CharacterTarget(target);
                    var targetIntent = new DamageIntentTargetAction(actionSource, characterTarget, DamageType.Additional);
                    var damagePoint = additionalAttackEffect.Value.Eval(this, trigger, targetIntent);
                    var damageResult = target.HealthManager.TakeAdditionalDamage(damagePoint, _contextMgr.Context);
                    var damageStyle = DamageStyle.None;

                    _UpdateAction(new DamageResultAction(actionSource, characterTarget, damageResult, damageStyle));
                    cardEffectEvents.Add(new DamageEvent(target.Faction(this), target, damageResult, damageStyle));
                }
                break;
            }
            case EffectiveAttackEffect effectiveAttackEffect:
            {
                var intent = new DamageIntentAction(actionSource, DamageType.Effective);
                var targets = effectiveAttackEffect.Targets.Eval(this, trigger, intent);
                foreach(var target in targets)
                {
                    var characterTarget = new CharacterTarget(target);
                    var targetIntent = new DamageIntentTargetAction(actionSource, characterTarget, DamageType.Effective);
                    var damagePoint = effectiveAttackEffect.Value.Eval(this, trigger, targetIntent);
                    var damageResult = target.HealthManager.TakeEffectiveDamage(damagePoint, _contextMgr.Context);
                    var damageStyle = DamageStyle.None;

                    _UpdateAction(new DamageResultAction(actionSource, characterTarget, damageResult, damageStyle));
                    cardEffectEvents.Add(new DamageEvent(target.Faction(this), target, damageResult, damageStyle));
                }
                break;
            }
            case HealEffect healEffect: 
            {
                var intent = new HealIntentAction(actionSource);
                var targets = healEffect.Targets.Eval(this, trigger, intent);
                foreach(var target in targets)
                {
                    var characterTarget = new CharacterTarget(target);
                    var targetIntent = new HealIntentTargetAction(actionSource, characterTarget);
                    var healPoint = healEffect.Value.Eval(this, trigger, targetIntent);
                    var healResult = target.HealthManager.GetHeal(healPoint, _contextMgr.Context);

                    _UpdateAction(new HealResultAction(actionSource, characterTarget, healResult));
                    cardEffectEvents.Add(new GetHealEvent(target.Faction(this), target, healResult));
                }
                break;
            }
            case ShieldEffect shieldEffect:
            {
                var intent = new ShieldIntentAction(actionSource);
                var targets = shieldEffect.Targets.Eval(this, trigger, intent);
                foreach(var target in targets)
                {
                    var characterTarget = new CharacterTarget(target);
                    var targetIntent = new ShieldIntentTargetAction(actionSource, characterTarget);
                    var shieldPoint = shieldEffect.Value.Eval(this, trigger, targetIntent);
                    var shieldResult = target.HealthManager.GetShield(shieldPoint, _contextMgr.Context);

                    _UpdateAction(new ShieldResultAction(actionSource, characterTarget, shieldResult));
                    cardEffectEvents.Add(new GetShieldEvent(target.Faction(this), target, shieldResult));
                }
                break;
            }
            case GainEnergyEffect gainEnergyEffect:
            {
                var intent = new GainEnergyIntentAction(actionSource);
                var targets = gainEnergyEffect.Targets.Eval(this, trigger, intent);
                foreach(var target in targets)
                {
                    var playerTarget = new PlayerTarget(target);
                    var targetIntent = new GainEnergyIntentTargetAction(actionSource, playerTarget);
                    var gainEnergy = gainEnergyEffect.Value.Eval(this, trigger, targetIntent);
                    var gainEnergyResult = target.EnergyManager.GainEnergy(gainEnergy);

                    _UpdateAction(new GainEnergyResultAction(actionSource, playerTarget, gainEnergyResult));
                    cardEffectEvents.Add(new GainEnergyEvent(target, gainEnergyResult));
                }
                break;
            }
            case LoseEnegyEffect loseEnegyEffect:
            {
                var intent = new LoseEnergyIntentAction(actionSource);
                var targets = loseEnegyEffect.Targets.Eval(this, trigger, intent);
                foreach(var target in targets)
                {
                    var playerTarget = new PlayerTarget(target);
                    var targetIntent = new LoseEnergyIntentTargetAction(actionSource, playerTarget);
                    var loseEnergy = loseEnegyEffect.Value.Eval(this, trigger, targetIntent);
                    var loseEnergyResult = target.EnergyManager.LoseEnergy(loseEnergy);

                    _UpdateAction(new LoseEnergyResultAction(actionSource, playerTarget, loseEnergyResult));
                    cardEffectEvents.Add(new LoseEnergyEvent(target, loseEnergyResult));
                }
                break;
            }

            // === BUFF EFFECT ===
            case AddPlayerBuffEffect addBuffEffect:
            {
                var intent = new AddPlayerBuffIntentAction(actionSource);
                var targets = addBuffEffect.Targets.Eval(this, trigger, intent);
                foreach (var target in targets)
                {
                    var playerTarget = new PlayerTarget(target);
                    var targetIntent = new AddPlayerBuffIntentTargetAction(actionSource, playerTarget);
                    var level = addBuffEffect.Level.Eval(this, trigger, targetIntent);
                    var addResult = target.BuffManager.AddBuff(
                        _contextMgr.BuffLibrary,
                        this,
                        trigger,
                        targetIntent,
                        addBuffEffect.BuffId,
                        level);

                    _UpdateAction(new AddPlayerBuffResultAction(actionSource, playerTarget, addResult));
                    IGameEvent buffEvent = addResult.IsNewBuff ?
                        new AddPlayerBuffEvent(target, addResult.Buff.ToInfo()) :
                        new UpdatePlayerBuffEvent(target, addResult.Buff.ToInfo());
                    cardEffectEvents.Add(buffEvent);
                }
                break;
            }
            case RemovePlayerBuffEffect removeBuffEffect:
            {
                var intent = new RemovePlayerBuffIntentAction(actionSource);
                var targets = removeBuffEffect.Targets.Eval(this, trigger, intent);
                foreach(var target in targets)
                {
                    var playerTarget = new PlayerTarget(target);
                    var targetIntent = new RemovePlayerBuffIntentTargetAction(actionSource, playerTarget);
                    var removeResult = target.BuffManager.RemoveBuff(
                        _contextMgr.BuffLibrary,
                        this,
                        targetIntent,
                        removeBuffEffect.BuffId);

                    removeResult.Buff.MatchSome(resultBuff => {
                        _UpdateAction(new RemovePlayerBuffResultAction(actionSource, playerTarget, removeResult));
                        cardEffectEvents.Add(new RemovePlayerBuffEvent(target, resultBuff.ToInfo()));
                    });
                }
                break;
            }

            // === CARD EFFECT ===
            case DrawCardEffect drawCardEffect:
            {
                var intent = new DrawCardIntentAction(actionSource);
                var targets = drawCardEffect.Targets.Eval(this, trigger, intent);
                foreach(var target in targets)
                {
                    var playerTarget = new PlayerTarget(target);
                    var targetIntent = new DrawCardIntentTargetAction(actionSource, playerTarget);
                    var drawCount = drawCardEffect.Value.Eval(this, trigger, targetIntent);
                    var drawEvents = _DrawCards(actionSource, target, drawCount);
                    cardEffectEvents.AddRange(drawEvents);
                }
                break;
            }
            case DiscardCardEffect discardCardEffect:
            {
                var intent = new DiscardCardIntentAction(actionSource);
                var cards = discardCardEffect.TargetCards.Eval(this, trigger, intent).ToList();
                for(var i = 0; i < cards.Count; i++)
                {
                    var card = cards[i];
                    card.Owner(this).MatchSome(cardOwner => {                                
                        if (cardOwner.CardManager.TryDiscardCard(
                            card.Identity, out var discardedCard, out var start, out var destination)) 
                        {
                            var cardTarget = new CardTarget(card);
                            _UpdateAction(new DiscardCardResultAction(actionSource, cardTarget, discardedCard));
                            cardEffectEvents.Add(new DiscardCardEvent(discardedCard, this, start, destination));
                        }
                    });
                }
                break;
            }
            case ConsumeCardEffect consumeCardEffect:
            {
                var intent = new ConsumeCardIntentAction(actionSource);
                var cards = consumeCardEffect.TargetCards.Eval(this, trigger, intent).ToList();
                for(var i = 0; i < cards.Count; i++)
                {   
                    var card = cards[i];
                    card.Owner(this).MatchSome(cardOwner => {
                        if (cardOwner.CardManager.TryConsumeCard(
                            card.Identity, out var consumedCard, out var start, out var destination)) 
                        {
                            var cardTarget = new CardTarget(card);
                            _UpdateAction(new ConsumeCardResultAction(actionSource, cardTarget, consumedCard));
                            cardEffectEvents.Add(new ConsumeCardEvent(consumedCard, this, start, destination));
                        }
                    });
                }
                break;
            }
            case DisposeCardEffect disposeCardEffect:
            {
                var intent = new DisposeCardIntentAction(actionSource);
                var cards = disposeCardEffect.TargetCards.Eval(this, trigger, intent).ToList();
                for(var i = 0; i < cards.Count; i++)
                {
                    var card = cards[i];
                    card.Owner(this).MatchSome(cardOwner => {
                        if (cardOwner.CardManager.TryDisposeCard(
                            card.Identity, out var disposedCard, out var start, out var destination)) 
                        {
                            var cardTarget = new CardTarget(card);
                            _UpdateAction(new DisposeCardResultAction(actionSource, cardTarget, disposedCard));
                            cardEffectEvents.Add(new DisposeCardEvent(disposedCard, this, start, destination));
                        }
                    });
                }
                break;
            }
            case CreateCardEffect createCardEffect:
            {
                var intent = new CreateCardIntentAction(actionSource);
                var target = createCardEffect.Target.Eval(this, trigger, intent);
                target.MatchSome(targetPlayer => {
                    foreach(var cardData in createCardEffect.CardDatas)
                    {
                        var playerTarget = new PlayerTarget(targetPlayer);
                        var targetIntent = new CreateCardIntentTargetAction(actionSource, playerTarget);

                        var createResult = targetPlayer
                            .CardManager.CreateNewCard(
                                this,
                                trigger,
                                targetIntent,
                                cardData.Data,
                                createCardEffect.CreateDestination,
                                _contextMgr.CardBuffLibrary,
                                createCardEffect.AddCardBuffDatas);

                        _UpdateAction(new CreateCardResultAction(actionSource, playerTarget, createResult));
                        cardEffectEvents.Add(new CreateCardEvent(createResult.Card, this, createResult.Zone));
                    }
                });
                break;             
            }
            case CloneCardEffect cloneCardEffect:
            {
                var intent = new CloneCardIntentAction(actionSource);
                var target = cloneCardEffect.Target.Eval(this, trigger, intent);
                target.MatchSome(targetPlayer => {
                    var playerTarget = new PlayerTarget(targetPlayer);
                    var targetIntent = new CloneCardIntentTargetAction(actionSource, playerTarget);
                    var cards = cloneCardEffect.ClonedCards.Eval(this, trigger, intent);
                    foreach(var card in cards)
                    {
                        var playerCardTarget = new PlayerAndCardTarget(targetPlayer, card);
                        targetIntent = new CloneCardIntentTargetAction(actionSource, playerCardTarget);
                        var cloneResult = targetPlayer
                            .CardManager.CloneNewCard(
                                this,
                                trigger,
                                targetIntent,
                                card,
                                cloneCardEffect.CloneDestination,
                                _contextMgr.CardBuffLibrary,
                                cloneCardEffect.AddCardBuffDatas);

                        _UpdateAction(new CloneCardResultAction(actionSource, playerCardTarget, cloneResult));
                        cardEffectEvents.Add(new CloneCardEvent(cloneResult.Card, this, cloneResult.Zone));
                    }
                });                
                break;
            }
            case AddCardBuffEffect addCardBuffEffect:
            {
                var intent = new AddCardBuffIntentAction(actionSource);
                var cards = addCardBuffEffect.TargetCards.Eval(this, trigger, intent).ToList();
                for(var i = 0; i < cards.Count; i++)
                {   
                    var card = cards[i];
                    foreach(var addCardBuff in addCardBuffEffect.AddCardBuffDatas)
                    {
                        var cardTarget = new CardTarget(card);
                        var targetIntent = new AddCardBuffIntentTargetAction(actionSource, cardTarget);
                        var addLevel = addCardBuff.Level.Eval(this, trigger, targetIntent);
                        var addBuffResult = card.BuffManager.AddBuff(
                            _contextMgr.CardBuffLibrary,
                            this,
                            trigger,
                            targetIntent,
                            addCardBuff.CardBuffId,
                            addLevel);
                        
                        _UpdateAction(new AddCardBuffResultAction(actionSource, cardTarget, addBuffResult));
                        cardEffectEvents.Add(new AddCardBuffEvent(card, this));
                    }                       
                }
                break;
            }
        }

        return cardEffectEvents;
    }

    private void _UpdateTiming(UpdateTiming updateTiming)
    {
        var timingAction = new UpdateTimingAction(updateTiming);
        _gameStatus.Ally.Update(this, timingAction);
        _gameStatus.Enemy.Update(this, timingAction);
    }

    private void _UpdateAction(IActionUnit actionUnit)
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
        _UpdateTiming(UpdateTiming.TriggerBuffStart);

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

                    _UpdateAction(new DamageResultAction(actionSource, characterTarget, damageResult, damageStyle));
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

                        _UpdateAction(new AddCardBuffResultAction(actionSource, cardTarget, addResult));
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

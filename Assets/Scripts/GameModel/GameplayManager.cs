using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Optional;
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
        Debug.Log($"SetUseCardSelectTarget targettype:{useCardAction.TargetType}");
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

                if (player.CardManager.HandCard.RemoveCard(usedCard)) 
                {
                    if(usedCard.Effects.TryGetValue(TriggerTiming.PlayCard, out var onPlayEffects))
                    {
                        foreach(var effect in onPlayEffects)
                        {
                            var applyCardEvents = _ApplyCardEffect(new CardSource(usedCard), new CardPlay(usedCard), effect);
                            useCardEvents.AddRange(applyCardEvents);
                        }
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
                }
            }

            // TODO : apply reactionEffects

            // TODO : figure out pass how many value is enough?
            // TODO : figure out is it needed to invork more action during every function? 
            //        or how to check how many action happened in 1 card?
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

                _UpdateAction(new RecycleDeckAction(new PlayerTarget(player)));
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
            _UpdateAction(new DrawCardAction(source, new PlayerTarget(player), newCard));

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

    private IEnumerable<IGameEvent> _ApplyCardEffect(
        IActionSource actionSource, ITriggerSource triggerSource, ICardEffect cardEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        switch(cardEffect)
        {
            case DamageEffect damageEffect:
            {
                var targets = damageEffect.Targets.Eval(this, triggerSource);
                foreach(var target in targets)
                {
                    var characterTarget = new CharacterTarget(target);
                    var damagePoint = damageEffect.Value.Eval(this, triggerSource);
                    var damageResult = target.HealthManager.TakeDamage(damagePoint, _contextMgr.Context);
                    var damageStyle = DamageStyle.None;

                    _UpdateAction(new DamageAction(actionSource, characterTarget, damageResult, damageStyle));
                    cardEffectEvents.Add(new DamageEvent(target.Faction(this), target, damageResult, damageStyle));
                }
                break;
            }
            case PenetrateDamageEffect penetrateDamageEffect:
            {
                var targets = penetrateDamageEffect.Targets.Eval(this, triggerSource);
                foreach(var target in targets)
                {
                    var characterTarget = new CharacterTarget(target);
                    var damagePoint = penetrateDamageEffect.Value.Eval(this, triggerSource);
                    var damageResult = target.HealthManager.TakePenetrateDamage(damagePoint, _contextMgr.Context);
                    var damageStyle = DamageStyle.None;

                    _UpdateAction(new DamageAction(actionSource, characterTarget, damageResult, damageStyle));
                    cardEffectEvents.Add(new DamageEvent(target.Faction(this), target, damageResult, damageStyle));
                }
                break;
            }
            case AdditionalAttackEffect additionalAttackEffect:
            {
                var targets = additionalAttackEffect.Targets.Eval(this, triggerSource);
                foreach(var target in targets)
                {
                    var characterTarget = new CharacterTarget(target);
                    var damagePoint = additionalAttackEffect.Value.Eval(this, triggerSource);
                    var damageResult = target.HealthManager.TakeAdditionalDamage(damagePoint, _contextMgr.Context);
                    var damageStyle = DamageStyle.None;

                    _UpdateAction(new DamageAction(actionSource, characterTarget, damageResult, damageStyle));
                    cardEffectEvents.Add(new DamageEvent(target.Faction(this), target, damageResult, damageStyle));
                }
                break;
            }
            case EffectiveAttackEffect effectiveAttackEffect:
            {
                var targets = effectiveAttackEffect.Targets.Eval(this, triggerSource);
                foreach(var target in targets)
                {
                    var characterTarget = new CharacterTarget(target);
                    var damagePoint = effectiveAttackEffect.Value.Eval(this, triggerSource);
                    var damageResult = target.HealthManager.TakeEffectiveDamage(damagePoint, _contextMgr.Context);
                    var damageStyle = DamageStyle.None;

                    _UpdateAction(new DamageAction(actionSource, characterTarget, damageResult, damageStyle));
                    cardEffectEvents.Add(new DamageEvent(target.Faction(this), target, damageResult, damageStyle));
                }
                break;
            }
            case HealEffect healEffect: 
            {
                var targets = healEffect.Targets.Eval(this, triggerSource);
                foreach(var target in targets)
                {
                    var characterTarget = new CharacterTarget(target);
                    var healPoint = healEffect.Value.Eval(this, triggerSource);
                    var healResult = target.HealthManager.GetHeal(healPoint, _contextMgr.Context);

                    _UpdateAction(new GetHealAction(actionSource, characterTarget, healResult));
                    cardEffectEvents.Add(new GetHealEvent(target.Faction(this), target, healResult));
                }
                break;
            }
            case ShieldEffect shieldEffect:
            {
                var targets = shieldEffect.Targets.Eval(this, triggerSource);
                foreach(var target in targets)
                {
                    var characterTarget = new CharacterTarget(target);
                    var shieldPoint = shieldEffect.Value.Eval(this, triggerSource);
                    var shieldResult = target.HealthManager.GetShield(shieldPoint, _contextMgr.Context);

                    _UpdateAction(new GetShieldAction(actionSource, characterTarget, shieldResult));
                    cardEffectEvents.Add(new GetShieldEvent(target.Faction(this), target, shieldResult));
                }
                break;
            }
            case GainEnergyEffect gainEnergyEffect:
            {
                var targets = gainEnergyEffect.Targets.Eval(this, triggerSource);
                foreach(var target in targets)
                {
                    var playerTarget = new PlayerTarget(target);
                    var gainEnergy = gainEnergyEffect.Value.Eval(this, triggerSource);
                    var gainEnergyResult = target.EnergyManager.GainEnergy(gainEnergy);

                    _UpdateAction(new GainEnergyAction(actionSource, playerTarget, gainEnergyResult));
                    cardEffectEvents.Add(new GainEnergyEvent(target, gainEnergyResult));
                }
                break;
            }
            case LoseEnegyEffect loseEnegyEffect:
            {
                var targets = loseEnegyEffect.Targets.Eval(this, triggerSource);
                foreach(var target in targets)
                {
                    var playerTarget = new PlayerTarget(target);
                    var loseEnergy = loseEnegyEffect.Value.Eval(this, triggerSource);
                    var loseEnergyResult = target.EnergyManager.LoseEnergy(loseEnergy);

                    _UpdateAction(new LoseEnergyAction(actionSource, playerTarget, loseEnergyResult));
                    cardEffectEvents.Add(new LoseEnergyEvent(target, loseEnergyResult));
                }
                break;
            }

            // === BUFF EFFECT ===
            case AddBuffEffect addBuffEffect:
            {
                var targets = addBuffEffect.Targets.Eval(this, triggerSource);
                foreach(var target in targets)
                {
                    var playerTarget = new PlayerTarget(target);
                    var level = addBuffEffect.Level.Eval(this, triggerSource);
                    if (target.BuffManager.AddBuff(
                        _contextMgr.BuffLibrary, 
                        this, 
                        actionSource,
                        addBuffEffect.BuffId, 
                        level,
                        out IPlayerBuffEntity resultBuff))
                    {
                        _UpdateAction(new AddPlayerBuffAction(actionSource, playerTarget, resultBuff));
                        cardEffectEvents.Add(new AddPlayerBuffEvent(target, resultBuff.ToInfo()));
                    }
                    else
                    {
                        _UpdateAction(new StackPlayerBuffAction(actionSource, playerTarget, resultBuff));
                        cardEffectEvents.Add(new UpdatePlayerBuffEvent(target, resultBuff.ToInfo()));
                    }
                }
                break;
            }
            case RemoveBuffEffect removeBuffEffect:
            {
                var targets = removeBuffEffect.Targets.Eval(this, triggerSource);
                foreach(var target in targets)
                {
                    var playerTarget = new PlayerTarget(target);
                    if(target.BuffManager.RemoveBuff(
                        _contextMgr.BuffLibrary, 
                        this, 
                        actionSource,
                        removeBuffEffect.BuffId,
                        out IPlayerBuffEntity resultBuff))
                    {
                        _UpdateAction(new RemovePlayerBuffAction(actionSource, playerTarget, resultBuff));
                        cardEffectEvents.Add(new RemovePlayerBuffEvent(target, resultBuff.ToInfo()));
                    }
                }
                break;
            }

            // === CARD EFFECT ===
            case DrawCardEffect drawCardEffect:
            {
                var targets = drawCardEffect.Targets.Eval(this, triggerSource);
                foreach(var target in targets)
                {
                    var playerTarget = new PlayerTarget(target);
                    var drawCount = drawCardEffect.Value.Eval(this, triggerSource);
                    var drawEvents = _DrawCards(actionSource, target, drawCount);
                    cardEffectEvents.AddRange(drawEvents);
                }
                break;
            }
            case DiscardCardEffect discardCardEffect:
            {
                var cards = discardCardEffect.TargetCards.Eval(this, triggerSource).ToList();
                for(var i = 0; i < cards.Count; i++)
                {
                    var card = cards[i];
                    card.Owner(this).MatchSome(cardOwner => {                                
                        if (cardOwner.CardManager.TryDiscardCard(
                            card.Identity, out var discardedCard, out var start, out var destination)) 
                        {
                            var cardTarget = new CardTarget(discardedCard);
                            _UpdateAction(new DiscardCardAction(actionSource, cardTarget, discardedCard));
                            cardEffectEvents.Add(new DiscardCardEvent(discardedCard, this, start, destination));
                        }
                    });
                }
                break;
            }
            case ConsumeCardEffect consumeCardEffect:
            {
                var cards = consumeCardEffect.TargetCards.Eval(this, triggerSource).ToList();
                for(var i = 0; i < cards.Count; i++)
                {   
                    var card = cards[i];
                    card.Owner(this).MatchSome(cardOwner => {
                        if (cardOwner.CardManager.TryConsumeCard(
                            card.Identity, out var consumedCard, out var start, out var destination)) 
                        {
                            var cardTarget = new CardTarget(consumedCard);
                            _UpdateAction(new ConsumeCardAction(actionSource, cardTarget, consumedCard));
                            cardEffectEvents.Add(new ConsumeCardEvent(consumedCard, this, start, destination));
                        }
                    });
                }
                break;
            }
            case DisposeCardEffect disposeCardEffect:
            {
                var cards = disposeCardEffect.TargetCards.Eval(this, triggerSource).ToList();
                for(var i = 0; i < cards.Count; i++)
                {
                    var card = cards[i];
                    card.Owner(this).MatchSome(cardOwner => {
                        if (cardOwner.CardManager.TryDisposeCard(
                            card.Identity, out var disposedCard, out var start, out var destination)) 
                        {
                            var cardTarget = new CardTarget(disposedCard);
                            _UpdateAction(new DisposeCardAction(actionSource, cardTarget, disposedCard));
                            cardEffectEvents.Add(new DisposeCardEvent(disposedCard, this, start, destination));
                        }
                    });
                }
                break;
            }
            case CreateCardEffect createCardEffect:
            {
                var target = createCardEffect.Target.Eval(this, triggerSource);
                target.MatchSome(targetPlayer => {
                    foreach(var cardData in createCardEffect.CardDatas)
                    {
                        var addCardBuffs = createCardEffect.AddCardBuffDatas
                            .Select(addData => {
                                var cardBuffData = _contextMgr.CardBuffLibrary.GetCardBuffData(addData.CardBuffId);
                                return CardBuffEntity.CreateEntity(cardBuffData);
                            });
                        var cardEntity = CardEntity.CreateFromData(cardData.Data, addCardBuffs);
                        targetPlayer.CardManager.AddNewCard(cardEntity, createCardEffect.CreateDestination);

                        var destination = targetPlayer.CardManager.GetCardCollectionZone( createCardEffect.CreateDestination);
                        cardEffectEvents.Add(new CreateCardEvent(cardEntity, this, destination));
                    }
                });
                break;             
            }
            case CloneCardEffect cloneCardEffect:
            {
                var target = cloneCardEffect.Target.Eval(this, triggerSource);
                target.MatchSome(targetPlayer => {
                    var cards = cloneCardEffect.ClonedCards.Eval(this, triggerSource);
                    foreach(var card in cards)
                    {
                        var addCardBuffs = cloneCardEffect.AddCardBuffDatas
                            .Select(addData => {
                                var cardBuffData = _contextMgr.CardBuffLibrary.GetCardBuffData(addData.CardBuffId);
                                return CardBuffEntity.CreateEntity(cardBuffData);
                            });
                        var cloneCard = card.Clone(addCardBuffs);     
                        targetPlayer.CardManager.AddNewCard(cloneCard, cloneCardEffect.CloneDestination);                            
                        var destinationZone = targetPlayer.CardManager.GetCardCollectionZone(cloneCardEffect.CloneDestination);
                        cardEffectEvents.Add(new CloneCardEvent(cloneCard, this, destinationZone));
                    }
                });                
                break;
            }
            case AppendCardBuffEffect appendCardBuffEffect:
            {
                var cards = appendCardBuffEffect.TargetCards.Eval(this, triggerSource).ToList();
                for(var i = 0; i < cards.Count; i++)
                {   
                    var card = cards[i];
                    var addCardBuffs = appendCardBuffEffect.AddCardBuffDatas
                        .Select(addData => {
                            var cardBuffData = _contextMgr.CardBuffLibrary.GetCardBuffData(addData.CardBuffId);
                            return CardBuffEntity.CreateEntity(cardBuffData);
                        });
                    
                    foreach(var addCardBuff in addCardBuffs)
                    {
                        card.AddNewStatus(addCardBuff);
                    }                            

                    cardEffectEvents.Add(new AppendCardBuffEvent(card, this));
                }
                break;
            }
        }

        return cardEffectEvents;
    }

    private void _UpdateTiming(UpdateTiming updateTiming)
    {

    }

    private void _UpdateAction(IResultAction resulAction)
    {              
        foreach(var buff in _gameStatus.Ally.BuffManager.Buffs)
        {
            foreach(var session in buff.ReactionSessions)
            {
                session.UpdateResult(this, resulAction);
            }
            foreach(var propertyEntity in buff.Properties)
            {
                propertyEntity.Update(this);
            }
            buff.LifeTime.UpdateResult(this, resulAction);
        }

        foreach(var character in _gameStatus.Ally.Characters)
        { 
        }

        foreach(var card in _gameStatus.Ally.CardManager.HandCard.Cards)
        { 
        }

        foreach(var buff in _gameStatus.Enemy.BuffManager.Buffs)
        {
            foreach(var session in buff.ReactionSessions)
            {
                session.UpdateResult(this, resulAction);
            }
            foreach(var propertyEntity in buff.Properties)
            {
                propertyEntity.Update(this);
            }
            buff.LifeTime.UpdateResult(this, resulAction);
        }

        foreach(var character in _gameStatus.Enemy.Characters)
        { 
        }

        foreach(var card in _gameStatus.Enemy.CardManager.HandCard.Cards)
        { 
        }
    }

    // TODO: collect reactionEffects created from reactionSessions
    private IEnumerable<IGameEvent> _TriggerTiming(TriggerTiming timing)
    {
        var triggerBuffEvents = new List<IGameEvent>();
        foreach(var buff in _gameStatus.Ally.BuffManager.Buffs)
        {
            var buffTrigger = new PlayerBuffTrigger(buff);
            var conditionalEffectsOpt = _contextMgr.BuffLibrary.GetBuffEffects(buff.Id, timing);
            conditionalEffectsOpt.MatchSome(conditionalEffects => 
            {
                foreach(var conditionalEffect in conditionalEffects)
                {
                    if (conditionalEffect.Conditions.All(c => c.Eval(this, buffTrigger)))
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
            var conditionalEffectsOpt = _contextMgr.BuffLibrary.GetBuffEffects(buff.Id, timing);

            conditionalEffectsOpt.MatchSome(conditionalEffects => 
            {
                foreach(var conditionalEffect in conditionalEffects)
                {
                    if (conditionalEffect.Conditions.All(c => c.Eval(this, buffTrigger)))
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
                var targets = effectiveDamageBuffEffect.Targets.Eval(this, triggerSource);
                foreach(var target in targets)
                {
                    var characterTarget = new CharacterTarget(target);
                    var damagePoint = effectiveDamageBuffEffect.Value.Eval(this, triggerSource);
                    var damageResult = target.HealthManager.TakeEffectiveDamage(damagePoint, _contextMgr.Context);
                    var damageStyle = DamageStyle.None;

                    _UpdateAction(new DamageAction(actionSource, characterTarget, damageResult, damageStyle));
                    appleBuffEffectEvents.Add(new DamageEvent(target.Faction(this), target, damageResult, damageStyle));
                }
                break;
            }
        }

        var triggerEndEvents = _TriggerTiming(TriggerTiming.TriggerBuffEnd);
        appleBuffEffectEvents.AddRange(triggerEndEvents);

        return appleBuffEffectEvents;
    }
}

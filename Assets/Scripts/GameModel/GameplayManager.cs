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
                _GameStart();
                break;
            case GameState.TurnStart:
                _TurnStart();
                break;
            case GameState.DrawCard:
                _TurnDrawCard();
                break;
            case GameState.EnemyPrepare:
                _EnemyPreapre();
                break;
            case GameState.PlayerPrepare:
                _PlayerPrepare();
                break;
            case GameState.PlayerExecute:
                _PlayerExecute();
                break;
            case GameState.Enemy_Execute:
                _EnemyExecute();
                break;
            case GameState.TurnEnd:
                _TurnEnd();
                break;
            case GameState.GameEnd:
                break;
        }
    }

    private void _GameStart()
    {
        _gameEvents.Add(new AllySummonEvent() {
            Player = _gameStatus.Ally 
        });
        _gameEvents.Add(new EnemySummonEvent() {
            Enemy = _gameStatus.Enemy 
        });

        _gameStatus.SetState(GameState.TurnStart);
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
        _gameEvents.Add(new RecoverEnergyEvent(_gameStatus.Ally, allyGainEnergyResult));

        var enemyGainEnergyResult = _gameStatus.Enemy.EnergyManager.RecoverEnergy(_gameStatus.Enemy.EnergyRecoverPoint);
        _gameEvents.Add(new RecoverEnergyEvent(_gameStatus.Enemy, enemyGainEnergyResult));

        _gameStatus.SetState(GameState.DrawCard);
    }

    private void _TurnDrawCard()
    {
        var allyDrawCount = _contextMgr.DispositionLibrary.GetDrawCardCount(_gameStatus.Ally.DispositionManager.CurrentDisposition);
        var enemyDrawCount = _gameStatus.Enemy.TurnStartDrawCardCount;

        var allyDrawEvents = _DrawCards(_gameStatus.Ally, allyDrawCount);
        _gameEvents.AddRange(allyDrawEvents.Evts);

        // TODO : apply allyDrawEvents.Reactions

        var enemyDrawEvents = _DrawCards(_gameStatus.Enemy, enemyDrawCount);
        _gameEvents.AddRange(enemyDrawEvents.Evts);

        // TODO : apply enemyDrawEvents.Reactions

        _gameStatus.SetState(GameState.EnemyPrepare);
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

        _gameStatus.SetState(GameState.PlayerPrepare);
    }

    private void _PlayerPrepare()
    {
        _gameEvents.Add(new PlayerExecuteStartEvent() {
            Faction = _gameStatus.Ally.Faction,
            HandCardInfo = _gameStatus.Ally.CardManager.HandCard.ToCardCollectionInfo(this)
        });

        _gameStatus.SetState(GameState.PlayerExecute);
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
        using(_contextMgr.SetExecutePlayer(player))
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
    }

    private void _FinishPlayerExecuteTurn()
    {
        _gameEvents.Add(new PlayerExecuteEndEvent(){
            Faction = _gameStatus.Ally.Faction,
            HandCardInfo = _gameStatus.Ally.CardManager.HandCard.ToCardCollectionInfo(this)
        });
        
        _TriggerBuffs(_gameStatus.Ally, GameTiming.ExecuteEnd);

        _gameStatus.SetState(GameState.Enemy_Execute);
        _gameActions.Clear();
    }   
    private void _FinishEnemyExecuteTurn()
    {
        _TriggerBuffs(_gameStatus.Enemy, GameTiming.ExecuteEnd);

        _gameStatus.SetState(GameState.TurnEnd);
        _gameActions.Clear();
    }

    private void _TurnEnd()
    {
        using(_contextMgr.SetExecutePlayer(_gameStatus.Ally))
        {
            _gameEvents.AddRange(
                _gameStatus.Ally.CardManager.ClearHandOnTurnEnd(this));
            _gameEvents.AddRange(
                _gameStatus.Ally.CardManager.UpdateCardsOnTiming(this, GameTiming.TurnEnd));
        }

        using(_contextMgr.SetExecutePlayer(_gameStatus.Enemy))
        {
            _gameEvents.AddRange(
                _gameStatus.Enemy.CardManager.ClearHandOnTurnEnd(this));
            _gameEvents.AddRange(
                _gameStatus.Enemy.CardManager.UpdateCardsOnTiming(this, GameTiming.TurnEnd));
        }

        _gameStatus.SetState(GameState.TurnStart);
    }

    private GameContextManager _SetUseCardSelectTarget(UseCardAction useCardAction)
    {
        Debug.Log($"SetUseCardSelectTarget targettype:{useCardAction.TargetType}");
        switch(useCardAction.TargetType)
        { 
            case TargetType.Character:
                return useCardAction.SelectedTarget.Match(
                    targetIdentity => {
                        var targetCharacterOption = _gameStatus.CharacterManager.GetCharacter(targetIdentity);
                        return targetCharacterOption.Match(
                            targetCharacter => {
                                return _contextMgr.SetSelectedCharacter(targetCharacter); 
                            },
                            () => { return _contextMgr.SetClone(); }
                        );
                    },
                    () => _contextMgr.SetClone());
            case TargetType.Card:
                return useCardAction.SelectedTarget.Match(
                    targetCardIndentity => {
                        var targetCardOption = _gameStatus.Ally.CardManager.GetCard(targetCardIndentity);
                        return targetCardOption.Match(
                            targetCard => { return _contextMgr.SetSelectedCard(targetCard); },
                            ()         => { return _contextMgr.SetClone(); }
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
        using(_contextMgr.SetCardCaster(player))
        {
            var usedCard = player.CardManager.HandCard.Cards.FirstOrDefault(c => c.Identity == CardIndentity);
            if (usedCard != null &&
                !usedCard.HasProperty(CardProperty.Sealed))
            {
                var useCardEvents = new List<IGameEvent>();
                var reactionEffects = new List<IReactionEffect>();
                Debug.Log($"SetUsingCard card:{usedCard.CardDataId}");
                using(_contextMgr.SetUsingCard(usedCard))
                {
                    var cardRuntimCost = usedCard.EvalCost(this);
                    if (cardRuntimCost <= player.CurrentEnergy)
                    {
                        var loseEnergyResult = player.EnergyManager.ConsumeEnergy(cardRuntimCost);
                        useCardEvents.Add(new ConsumeEnergyEvent(player, loseEnergyResult));

                        if (player.CardManager.HandCard.RemoveCard(usedCard)) 
                        {
                            if(usedCard.Effects.TryGetValue(GameTiming.PlayCard, out var onPlayEffects))
                            {
                                using(_contextMgr.SetCardTiming(GameTiming.PlayCard))
                                {
                                    foreach(var effect in onPlayEffects)
                                    {
                                        var applyCardEvents = _ApplyCardEffect(effect);
                                        useCardEvents.AddRange(applyCardEvents.Evts);
                                        reactionEffects.AddRange(applyCardEvents.Reactions);
                                    }
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
                            reactionEffects.AddRange(_TriggerReactionEffect(usedCardEvent));
                        }
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
    }

    private (IEnumerable<IGameEvent> Evts, IEnumerable<IReactionEffect> Reactions) _DrawCards(IPlayerEntity player, int drawCount)
    {
        var drawCardEvents = new List<IGameEvent>();
        var reactionEffects = new List<IReactionEffect>();
        for (int i = 0; i < drawCount; i++)
        {
            if( player.CardManager.Deck.Cards.Count == 0 &&
                player.CardManager.Graveyard.Cards.Count > 0)
            {
                var graveyardCards = player.CardManager.Graveyard.PopAllCards();
                player.CardManager.Deck.EnqueueCardsThenShuffle(graveyardCards);
                _gameEvents.Add(new RecycleGraveyardEvent() {
                    Faction = player.Faction,
                    DeckInfo = player.CardManager.Deck.ToCardCollectionInfo(this),
                    GraveyardInfo = player.CardManager.Graveyard.ToCardCollectionInfo(this)
                });
            }

            if (player.CardManager.Deck.Cards.Count > 0)
            {
                _DrawCard(player).MatchSome(drawCardEvent => {
                    drawCardEvents.Add(drawCardEvent);
                    reactionEffects.AddRange(_TriggerReactionEffect(drawCardEvent));
                });
            }
        }

        return (drawCardEvents, reactionEffects);
    }

    private Option<IGameEvent> _DrawCard(IPlayerEntity player)
    {
        if (player.CardManager.Deck.PopCard(out ICardEntity newCard))
        {
            player.CardManager.HandCard.AddCard(newCard);

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

    private (IEnumerable<IGameEvent> Evts, IEnumerable<IReactionEffect> Reactions) _ApplyCardEffect(ICardEffect cardEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        var reactionEffects = new List<IReactionEffect>();
        using(_contextMgr.SetUsingCardEffect(cardEffect))
        {
            switch(_contextMgr.Context.UsingCardEffect)
            {
                case DamageEffect damageEffect:
                {
                    var targets = damageEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetCharacter(target))
                        {
                            var damagePoint = damageEffect.Value.Eval(this);
                            var takeDamageResult = target.HealthManager.TakeDamage(damagePoint, _contextMgr.Context);
                            var result = new NormalDamageEvent(target, takeDamageResult);

                            cardEffectEvents.Add(result);
                            reactionEffects.AddRange(_TriggerReactionEffect(result));
                        }
                    }
                    break;
                }
                case PenetrateDamageEffect penetrateDamageEffect:
                {
                    var targets = penetrateDamageEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetCharacter(target))
                        {
                            var damagePoint = penetrateDamageEffect.Value.Eval(this);
                            var takeDamageResult = target.HealthManager.TakePenetrateDamage(damagePoint, _contextMgr.Context);
                            var result = new PenetrateDamageEvent(target, takeDamageResult);

                            cardEffectEvents.Add(result);
                            reactionEffects.AddRange(_TriggerReactionEffect(result));
                        }
                    }
                    break;
                }
                case AdditionalAttackEffect additionalAttackEffect:
                {
                    var targets = additionalAttackEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetCharacter(target))
                        {
                            var damagePoint = additionalAttackEffect.Value.Eval(this);
                            var takeDamageResult = target.HealthManager.TakeAdditionalDamage(damagePoint, _contextMgr.Context);
                            var result = new AdditionalAttackEvent(target, takeDamageResult);

                            cardEffectEvents.Add(result);
                            reactionEffects.AddRange(_TriggerReactionEffect(result));
                        }
                    }
                    break;
                }
                case EffectiveAttackEffect effectiveAttackEffect:
                {
                    var targets = effectiveAttackEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetCharacter(target))
                        {
                            var damagePoint = effectiveAttackEffect.Value.Eval(this);
                            var takeDamageResult = target.HealthManager.TakeEffectiveDamage(damagePoint, _contextMgr.Context);
                            var result = new EffectiveAttackEvent(target, takeDamageResult);
                            
                            cardEffectEvents.Add(result);
                            reactionEffects.AddRange(_TriggerReactionEffect(result));
                        }
                    }
                    break;
                }
                case HealEffect healEffect: 
                {
                    var targets = healEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetCharacter(target))
                        {
                            var healPoint = healEffect.Value.Eval(this);
                            var getHealResult = target.HealthManager.GetHeal(healPoint, _contextMgr.Context);
                            var result = new GetHealEvent(target, getHealResult);
                            
                            cardEffectEvents.Add(result);
                            reactionEffects.AddRange(_TriggerReactionEffect(result));
                        }
                    }
                    break;
                }
                case ShieldEffect shieldEffect:
                {
                    var targets = shieldEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetCharacter(target))
                        {
                            var shieldPoint = shieldEffect.Value.Eval(this);
                            var getShieldResult = target.HealthManager.GetShield(shieldPoint, _contextMgr.Context);
                            var result = new GetShieldEvent(target, getShieldResult);
                            
                            cardEffectEvents.Add(result);
                            reactionEffects.AddRange(_TriggerReactionEffect(result));
                        }
                    }
                    break;
                }
                case GainEnergyEffect gainEnergyEffect:
                {
                    var targets = gainEnergyEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetPlayer(target))
                        {
                            var gainEnergy = gainEnergyEffect.Value.Eval(this);
                            var gainEnergyResult = target.EnergyManager.GainEnergy(gainEnergy);
                            var result = new GainEnergyEvent(target, gainEnergyResult);
                            
                            cardEffectEvents.Add(result);
                            reactionEffects.AddRange(_TriggerReactionEffect(result));
                        }
                    }
                    break;
                }
                case LoseEnegyEffect loseEnegyEffect:
                {
                    var targets = loseEnegyEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetPlayer(target))
                        {
                            var loseEnergy = loseEnegyEffect.Value.Eval(this);
                            var loseEnergyResult = target.EnergyManager.LoseEnergy(loseEnergy);
                            var result = new LoseEnergyEvent(target, loseEnergyResult);
                            
                            cardEffectEvents.Add(result);
                            reactionEffects.AddRange(_TriggerReactionEffect(result));
                        }
                    }
                    break;
                }

                // === BUFF EFFECT ===
                case AddBuffEffect addBuffEffect:
                {
                    var targets = addBuffEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetPlayer(target))
                        {
                            var level = addBuffEffect.Level.Eval(this);
                            if (target.BuffManager.AddBuff(
                                _contextMgr.BuffLibrary, 
                                _contextMgr.Context, 
                                addBuffEffect.BuffId, 
                                level,
                                out IPlayerBuffEntity resultBuff))
                            {
                                var result = new AddBuffEvent(target, resultBuff.ToInfo());
                                
                                cardEffectEvents.Add(result);
                                reactionEffects.AddRange(_TriggerReactionEffect(result));
                            }
                            else
                            {
                                var result = new UpdateBuffEvent(target, resultBuff.ToInfo());
                                
                                cardEffectEvents.Add(result);
                                reactionEffects.AddRange(_TriggerReactionEffect(result));
                            }
                        }
                    }
                    break;
                }
                case RemoveBuffEffect removeBuffEffect:
                {
                    var targets = removeBuffEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetPlayer(target))
                        {
                            if(target.BuffManager.RemoveBuff(
                                _contextMgr.BuffLibrary, 
                                _contextMgr.Context, 
                                removeBuffEffect.BuffId,
                                out IPlayerBuffEntity resultBuff))
                            {
                                var result = new RemoveBuffEvent(target, resultBuff.ToInfo());
                                
                                cardEffectEvents.Add(result);
                                reactionEffects.AddRange(_TriggerReactionEffect(result));
                            }
                        }
                    }
                    break;
                }

                // === CARD EFFECT ===
                case DrawCardEffect drawCardEffect:
                {
                    var targets = drawCardEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetPlayer(target))
                        {
                            var drawCount = drawCardEffect.Value.Eval(this);
                            var drawEvents = _DrawCards(target, drawCount);
                            cardEffectEvents.AddRange(drawEvents.Evts);
                            reactionEffects.AddRange(drawEvents.Reactions);
                        }
                    }
                    break;
                }
                case DiscardCardEffect discardCardEffect:
                {
                    var cards = discardCardEffect.TargetCards.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var card in cards)
                    {
                        using(_contextMgr.SetEffectTargetPlayer(card.Owner))
                        using(_contextMgr.SetEffectTargetCard(card))
                        {
                            if (card.Owner.CardManager.TryDiscardCard(
                                card.Identity, out var discardedCard, out var start, out var destination)) 
                            {
                                var result = new DiscardCardEvent(discardedCard, this, start, destination);
                                cardEffectEvents.Add(result);
                                reactionEffects.AddRange(_TriggerReactionEffect(result));
                            }
                        }
                    }
                    break;
                }
                case ConsumeCardEffect consumeCardEffect:
                {
                    var cards = consumeCardEffect.TargetCards.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var card in cards)
                    {
                        using(_contextMgr.SetEffectTargetPlayer(card.Owner))
                        using(_contextMgr.SetEffectTargetCard(card))
                        {
                            if (card.Owner.CardManager.TryConsumeCard(card.Identity, out var consumedCard, out var start, out var destination)) 
                            {
                                var result = new ConsumeCardEvent(consumedCard, this, start, destination);
                                cardEffectEvents.Add(result);
                                reactionEffects.AddRange(_TriggerReactionEffect(result));
                            }
                        }
                    }
                    break;
                }
                case DisposeCardEffect disposeCardEffect:
                {
                    var cards = disposeCardEffect.TargetCards.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var card in cards.ToArray())
                    {
                        using(_contextMgr.SetEffectTargetPlayer(card.Owner))
                        using(_contextMgr.SetEffectTargetCard(card))
                        {
                            if (card.Owner.CardManager.TryDisposeCard(card.Identity, out var disposedCard, out var start, out var destination)) 
                            {
                                var result = new DisposeCardEvent(disposedCard, this, start, destination);
                                cardEffectEvents.Add(result);
                                reactionEffects.AddRange(_TriggerReactionEffect(result));
                            }
                        }
                    }
                    break;
                }
                case CreateCardEffect createCardEffect:
                {
                    var target = createCardEffect.Target.Eval(_gameStatus, _contextMgr.Context);
                    using(_contextMgr.SetEffectTargetPlayer(target))
                    {
                        foreach(var cardData in createCardEffect.CardDatas)
                        {
                            var addCardStatuses = createCardEffect.AddCardStatusDatas
                                .Select(addData => {
                                    var cardStatusData = _contextMgr.CardStatusLibrary.GetCardStatusData(addData.CardStatusId);
                                    return CardStatusEntity.CreateEntity(cardStatusData);
                                });
                            var cardEntity = CardEntity.CreateFromData(cardData.Data, target, addCardStatuses);
                            target.CardManager.AddNewCard(cardEntity, createCardEffect.CreateDestination);

                            var destination = target.CardManager.GetCardCollectionZone( createCardEffect.CreateDestination);
                            var result = new CreateCardEvent(cardEntity, this, destination);
                            cardEffectEvents.Add(result);
                            reactionEffects.AddRange(_TriggerReactionEffect(result));
                        }
                        break;                        
                    }
                }
                case CloneCardEffect cloneCardEffect:
                {
                    var cards = cloneCardEffect.TargetCards.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var card in cards)
                    {
                        using(_contextMgr.SetEffectTargetPlayer(card.Owner))
                        using(_contextMgr.SetEffectTargetCard(card))
                        {
                            var addCardStatuses = cloneCardEffect.AddCardStatusDatas
                                .Select(addData => {
                                    var cardStatusData = _contextMgr.CardStatusLibrary.GetCardStatusData(addData.CardStatusId);
                                    return CardStatusEntity.CreateEntity(cardStatusData);
                                });
                            var cloneCard = card.Clone(card.Owner, addCardStatuses);
                            card.Owner.CardManager.AddNewCard(cloneCard, cloneCardEffect.CloneDestination);
                            
                            var destination = card.Owner.CardManager.GetCardCollectionZone(cloneCardEffect.CloneDestination);
                            var result = new CloneCardEvent(cloneCard, this, destination);
                            cardEffectEvents.Add(result);
                            reactionEffects.AddRange(_TriggerReactionEffect(result));
                        }
                    }
                    break;
                }
                case AppendCardStatusEffect appendCardStatusEffect:
                {
                    var cards = appendCardStatusEffect.TargetCards.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var card in cards)
                    {
                        using(_contextMgr.SetEffectTargetPlayer(card.Owner))
                        using(_contextMgr.SetEffectTargetCard(card))
                        {
                            var addCardStatuses = appendCardStatusEffect.AddCardStatusDatas
                                .Select(addData => {
                                    var cardStatusData = _contextMgr.CardStatusLibrary.GetCardStatusData(addData.CardStatusId);
                                    return CardStatusEntity.CreateEntity(cardStatusData);
                                });
                            
                            foreach(var addCardStatus in addCardStatuses)
                            {
                                card.AddNewStatus(addCardStatus);
                            }
                            
                            var result = new AppendCardStatusEvent(card, this);
                            cardEffectEvents.Add(result);
                            reactionEffects.AddRange(_TriggerReactionEffect(result));
                        }
                    }
                    break;
                }
            }
        }

        return (cardEffectEvents, reactionEffects);
    }

    private void _TriggerBuffs(PlayerEntity player, GameTiming timing)
    {
        var buffs = player.BuffManager.Buffs;
        foreach(var buff in buffs)
        {
            using(_contextMgr.SetTriggeredPlayerBuff(buff))
            {
                Debug.Log($"_TriggerBuffs buff:{buff} Timing:{timing}");
                if (buff.Effects.TryGetValue(timing, out var buffEffects))
                {
                    foreach(var effect in buffEffects)
                    {
                        _ApplyBuffEffect(effect);
                    }
                }
            }
        }
    }

    // TODO: applu character buff <-> player buff
    private void _ApplyBuffEffect(IPlayerBuffEffect buffEffect)
    {
        using(_contextMgr.SetTriggeredPlayerBuffEffect(buffEffect))
        {
            switch(_contextMgr.Context.TriggeredBuffEffect)
            {
                case EffectiveDamageBuffEffect effectiveDamageBuffEffect:
                {
                    var targets = effectiveDamageBuffEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetCharacter(target))
                        {
                            var damagePoint = effectiveDamageBuffEffect.Value.Eval(this);
                            var takeDamageResult = target.HealthManager.TakeEffectiveDamage(damagePoint, _contextMgr.Context);
                            _gameEvents.Add(new EffectiveAttackEvent(target, takeDamageResult));
                        }
                    }
                    break;
                }
            }
        }
    }

    // TODO: 
    private IEnumerable<IReactionEffect> _TriggerReactionEffect(IGameEvent gameEvent)
    {
        return new List<IReactionEffect>();
    }
}

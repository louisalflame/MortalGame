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
                _gameStatus.SetState(GameState.TurnStart);
                break;
            case GameState.TurnStart:
                _TurnStart();
                _gameStatus.SetState(GameState.DrawCard);
                break;
            case GameState.DrawCard:
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
                _TurnEnd();
                _gameStatus.SetState(GameState.TurnStart);
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
    }

    private void _TurnDrawCard()
    {
        var allyDrawCount = _contextMgr.DispositionLibrary.GetDrawCardCount(_gameStatus.Ally.DispositionManager.CurrentDisposition);
        var enemyDrawCount = _gameStatus.Enemy.TurnStartDrawCardCount;

        var allyDrawEvents = _DrawCards(_gameStatus.Ally, allyDrawCount);
        _gameEvents.AddRange(allyDrawEvents);

        var enemyDrawEvents = _DrawCards(_gameStatus.Enemy, enemyDrawCount);
        _gameEvents.AddRange(enemyDrawEvents);

        _TriggerTiming(GameTiming.DrawCard);

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
        
        var triggerEvts = _TriggerTiming(GameTiming.ExecuteEnd);
        _gameEvents.AddRange(triggerEvts);

        _gameStatus.SetState(GameState.Enemy_Execute);
        _gameActions.Clear();
    }   
    private void _FinishEnemyExecuteTurn()
    {
        var triggerEvts = _TriggerTiming(GameTiming.ExecuteEnd);
        _gameEvents.AddRange(triggerEvts);

        _gameStatus.SetState(GameState.TurnEnd);
        _gameActions.Clear();
    }

    private void _TurnEnd()
    {
        using(_contextMgr.SetGameTiming(GameTiming.TurnEnd))
        using(_contextMgr.SetExecutePlayer(_gameStatus.Ally))
        {
            _gameEvents.AddRange(
                _gameStatus.Ally.CardManager.ClearHandOnTurnEnd(this));
            _gameEvents.AddRange(
                _gameStatus.Ally.CardManager.UpdateCards(this));
        }

        using(_contextMgr.SetGameTiming(GameTiming.TurnEnd))
        using(_contextMgr.SetExecutePlayer(_gameStatus.Enemy))
        {
            _gameEvents.AddRange(
                _gameStatus.Enemy.CardManager.ClearHandOnTurnEnd(this));
            _gameEvents.AddRange(
                _gameStatus.Enemy.CardManager.UpdateCards(this));
        }
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
                                foreach(var effect in onPlayEffects)
                                {
                                    var applyCardEvents = _ApplyCardEffect(effect);
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

    private IEnumerable<IGameEvent> _DrawCards(IPlayerEntity player, int drawCount)
    {
        var drawCardEvents = new List<IGameEvent>();
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
                });
            }
        }

        return drawCardEvents;
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

    private IEnumerable<IGameEvent> _ApplyCardEffect(ICardEffect cardEffect)
    {
        var cardEffectEvents = new List<IGameEvent>();
        using(_contextMgr.SetUsingCardEffect(cardEffect))
        {
            switch(_contextMgr.Context.UsingCardEffect)
            {
                case DamageEffect damageEffect:
                {
                    var targets = damageEffect.Targets.Eval(this);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetCharacter(target))
                        {
                            var damagePoint = damageEffect.Value.Eval(this);
                            var takeDamageResult = target.HealthManager.TakeDamage(damagePoint, _contextMgr.Context);
                            cardEffectEvents.Add(new NormalDamageEvent(target, takeDamageResult));
                        }
                    }
                    break;
                }
                case PenetrateDamageEffect penetrateDamageEffect:
                {
                    var targets = penetrateDamageEffect.Targets.Eval(this);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetCharacter(target))
                        {
                            var damagePoint = penetrateDamageEffect.Value.Eval(this);
                            var takeDamageResult = target.HealthManager.TakePenetrateDamage(damagePoint, _contextMgr.Context);
                            cardEffectEvents.Add(new PenetrateDamageEvent(target, takeDamageResult));
                        }
                    }
                    break;
                }
                case AdditionalAttackEffect additionalAttackEffect:
                {
                    var targets = additionalAttackEffect.Targets.Eval(this);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetCharacter(target))
                        {
                            var damagePoint = additionalAttackEffect.Value.Eval(this);
                            var takeDamageResult = target.HealthManager.TakeAdditionalDamage(damagePoint, _contextMgr.Context);
                            cardEffectEvents.Add(new AdditionalAttackEvent(target, takeDamageResult));
                        }
                    }
                    break;
                }
                case EffectiveAttackEffect effectiveAttackEffect:
                {
                    var targets = effectiveAttackEffect.Targets.Eval(this);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetCharacter(target))
                        {
                            var damagePoint = effectiveAttackEffect.Value.Eval(this);
                            var takeDamageResult = target.HealthManager.TakeEffectiveDamage(damagePoint, _contextMgr.Context);
                            cardEffectEvents.Add(new EffectiveAttackEvent(target, takeDamageResult));
                        }
                    }
                    break;
                }
                case HealEffect healEffect: 
                {
                    var targets = healEffect.Targets.Eval(this);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetCharacter(target))
                        {
                            var healPoint = healEffect.Value.Eval(this);
                            var getHealResult = target.HealthManager.GetHeal(healPoint, _contextMgr.Context);
                            cardEffectEvents.Add(new GetHealEvent(target, getHealResult));
                        }
                    }
                    break;
                }
                case ShieldEffect shieldEffect:
                {
                    var targets = shieldEffect.Targets.Eval(this);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetCharacter(target))
                        {
                            var shieldPoint = shieldEffect.Value.Eval(this);
                            var getShieldResult = target.HealthManager.GetShield(shieldPoint, _contextMgr.Context);
                            cardEffectEvents.Add(new GetShieldEvent(target, getShieldResult));
                        }
                    }
                    break;
                }
                case GainEnergyEffect gainEnergyEffect:
                {
                    var targets = gainEnergyEffect.Targets.Eval(this);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetPlayer(target))
                        {
                            var gainEnergy = gainEnergyEffect.Value.Eval(this);
                            var gainEnergyResult = target.EnergyManager.GainEnergy(gainEnergy);
                            cardEffectEvents.Add(new GainEnergyEvent(target, gainEnergyResult));
                        }
                    }
                    break;
                }
                case LoseEnegyEffect loseEnegyEffect:
                {
                    var targets = loseEnegyEffect.Targets.Eval(this);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetPlayer(target))
                        {
                            var loseEnergy = loseEnegyEffect.Value.Eval(this);
                            var loseEnergyResult = target.EnergyManager.LoseEnergy(loseEnergy);
                            cardEffectEvents.Add(new LoseEnergyEvent(target, loseEnergyResult));
                        }
                    }
                    break;
                }

                // === BUFF EFFECT ===
                case AddBuffEffect addBuffEffect:
                {
                    var targets = addBuffEffect.Targets.Eval(this);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetPlayer(target))
                        {
                            var level = addBuffEffect.Level.Eval(this);
                            if (target.BuffManager.AddBuff(
                                _contextMgr.BuffLibrary, 
                                this, 
                                addBuffEffect.BuffId, 
                                level,
                                out IPlayerBuffEntity resultBuff))
                            {
                                cardEffectEvents.Add(new AddBuffEvent(target, resultBuff.ToInfo()));
                            }
                            else
                            {
                                cardEffectEvents.Add(new UpdateBuffEvent(target, resultBuff.ToInfo()));
                            }
                        }
                    }
                    break;
                }
                case RemoveBuffEffect removeBuffEffect:
                {
                    var targets = removeBuffEffect.Targets.Eval(this);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetPlayer(target))
                        {
                            if(target.BuffManager.RemoveBuff(
                                _contextMgr.BuffLibrary, 
                                this, 
                                removeBuffEffect.BuffId,
                                out IPlayerBuffEntity resultBuff))
                            {
                                cardEffectEvents.Add(new RemoveBuffEvent(target, resultBuff.ToInfo()));
                            }
                        }
                    }
                    break;
                }

                // === CARD EFFECT ===
                case DrawCardEffect drawCardEffect:
                {
                    var targets = drawCardEffect.Targets.Eval(this);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetPlayer(target))
                        {
                            var drawCount = drawCardEffect.Value.Eval(this);
                            var drawEvents = _DrawCards(target, drawCount);
                            cardEffectEvents.AddRange(drawEvents);
                        }
                    }
                    break;
                }
                case DiscardCardEffect discardCardEffect:
                {
                    var cards = discardCardEffect.TargetCards.Eval(this);
                    foreach(var card in cards)
                    {
                        using(_contextMgr.SetEffectTargetPlayer(card.Owner))
                        using(_contextMgr.SetEffectTargetCard(card))
                        {
                            if (card.Owner.CardManager.TryDiscardCard(
                                card.Identity, out var discardedCard, out var start, out var destination)) 
                            {
                                cardEffectEvents.Add(new DiscardCardEvent(discardedCard, this, start, destination));
                            }
                        }
                    }
                    break;
                }
                case ConsumeCardEffect consumeCardEffect:
                {
                    var cards = consumeCardEffect.TargetCards.Eval(this);
                    foreach(var card in cards)
                    {
                        using(_contextMgr.SetEffectTargetPlayer(card.Owner))
                        using(_contextMgr.SetEffectTargetCard(card))
                        {
                            if (card.Owner.CardManager.TryConsumeCard(card.Identity, out var consumedCard, out var start, out var destination)) 
                            {
                                cardEffectEvents.Add(new ConsumeCardEvent(consumedCard, this, start, destination));
                            }
                        }
                    }
                    break;
                }
                case DisposeCardEffect disposeCardEffect:
                {
                    var cards = disposeCardEffect.TargetCards.Eval(this);
                    foreach(var card in cards.ToArray())
                    {
                        using(_contextMgr.SetEffectTargetPlayer(card.Owner))
                        using(_contextMgr.SetEffectTargetCard(card))
                        {
                            if (card.Owner.CardManager.TryDisposeCard(card.Identity, out var disposedCard, out var start, out var destination)) 
                            {
                                cardEffectEvents.Add(new DisposeCardEvent(disposedCard, this, start, destination));
                            }
                        }
                    }
                    break;
                }
                case CreateCardEffect createCardEffect:
                {
                    var target = createCardEffect.Target.Eval(this);
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
                            cardEffectEvents.Add(new CreateCardEvent(cardEntity, this, destination));
                        }
                        break;                        
                    }
                }
                case CloneCardEffect cloneCardEffect:
                {
                    var cards = cloneCardEffect.TargetCards.Eval(this);
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
                            cardEffectEvents.Add(new CloneCardEvent(cloneCard, this, destination));
                        }
                    }
                    break;
                }
                case AppendCardStatusEffect appendCardStatusEffect:
                {
                    var cards = appendCardStatusEffect.TargetCards.Eval(this);
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

                            cardEffectEvents.Add(new AppendCardStatusEvent(card, this));
                        }
                    }
                    break;
                }
            }
        }

        return cardEffectEvents;
    }

    // TODO:
    // -1 update all session in playerBuff/ characterBuff/ cardStatus
    // -2 update all property in playerBuff/ characterBuff/ cardStatus
    // -3 update all lifeTime in playerBuff/ characterBuff/ cardStatus
    // -4 remove all expired playerBuff/ characterBuff/ cardStatus and push into event
    // -5 pass OBJECT from any time of any thing for any kind of session?
    private void _UpdateTiming(GameTiming gameTiming)
    {
        using(_contextMgr.SetGameTiming(gameTiming))
        {                
            foreach(var buff in _gameStatus.Ally.BuffManager.Buffs)
            {
                using(_contextMgr.SetTriggeredPlayerBuff(buff))
                {
                    foreach(var session in buff.ReactionSessions)
                    {
                        var data = _contextMgr.BuffLibrary.GetBuffSessionData(buff.Id, session.Id);
                        session.Update(this, data);
                    }
                    foreach(var propertyEntity in buff.Properties)
                    {
                        propertyEntity.Update(this);
                    }
                    buff.LifeTime.Update(this);
                }
            }

            foreach(var character in _gameStatus.Ally.Characters)
            { 
            }

            foreach(var card in _gameStatus.Ally.CardManager.HandCard.Cards)
            { 
            }

            foreach(var buff in _gameStatus.Enemy.BuffManager.Buffs)
            { 
                using(_contextMgr.SetTriggeredPlayerBuff(buff))
                {
                    foreach(var session in buff.ReactionSessions)
                    {
                        var data = _contextMgr.BuffLibrary.GetBuffSessionData(buff.Id, session.Id);
                        session.Update(this, data);
                    }
                    foreach(var propertyEntity in buff.Properties)
                    {
                        propertyEntity.Update(this);
                    }
                    buff.LifeTime.Update(this);
                }
            }

            foreach(var character in _gameStatus.Enemy.Characters)
            { 
            }

            foreach(var card in _gameStatus.Enemy.CardManager.HandCard.Cards)
            { 
            }
        }
    }

    // TODO: collect reactionEffects created from reactionSessions
    private IEnumerable<IGameEvent> _TriggerTiming(GameTiming timing)
    {
        var triggerBuffEvents = new List<IGameEvent>();
        using(_contextMgr.SetGameTiming(timing))
        {
            foreach(var buff in _gameStatus.Ally.BuffManager.Buffs)
            {
                using(_contextMgr.SetTriggeredPlayerBuff(buff))
                {
                    Debug.Log($"_TriggerBuffs buff:{buff} Timing:{timing}");
                    var conditionalEffectsOpt = _contextMgr.BuffLibrary.GetBuffEffects(buff.Id, timing);

                    conditionalEffectsOpt.MatchSome(conditionalEffects => 
                    {
                        foreach(var conditionalEffect in conditionalEffects)
                        {
                            if (conditionalEffect.Conditions.All(c => c.Eval(this)))
                            {
                                var applyEvts = _ApplyBuffEffect(conditionalEffect.Effect);
                                triggerBuffEvents.AddRange(applyEvts);
                            }
                        }
                    });
                }
            }

            foreach(var character in _gameStatus.Ally.Characters)
            { 
            }

            foreach(var card in _gameStatus.Ally.CardManager.HandCard.Cards)
            { 
            }

            foreach(var buff in _gameStatus.Enemy.BuffManager.Buffs)
            {
                using(_contextMgr.SetTriggeredPlayerBuff(buff))
                {
                    Debug.Log($"_TriggerBuffs buff:{buff} Timing:{timing}");
                    var conditionalEffectsOpt = _contextMgr.BuffLibrary.GetBuffEffects(buff.Id, timing);

                    conditionalEffectsOpt.MatchSome(conditionalEffects => 
                    {
                        foreach(var conditionalEffect in conditionalEffects)
                        {
                            if (conditionalEffect.Conditions.All(c => c.Eval(this)))
                            {
                                var applyEvts = _ApplyBuffEffect(conditionalEffect.Effect);
                                triggerBuffEvents.AddRange(applyEvts);
                            }
                        }
                    });
                }
            }

            foreach(var character in _gameStatus.Enemy.Characters)
            { 
            }

            foreach(var card in _gameStatus.Enemy.CardManager.HandCard.Cards)
            { 
            }
        }
        
        return triggerBuffEvents;
    }

    // TODO: apply character buff <-> player buff
    private IEnumerable<IGameEvent> _ApplyBuffEffect(IPlayerBuffEffect buffEffect)
    {
        var appleBuffEffectEvents = new List<IGameEvent>();

        using(_contextMgr.SetTriggeredPlayerBuffEffect(buffEffect))
        {
            switch(_contextMgr.Context.TriggeredBuffEffect)
            {
                case EffectiveDamageBuffEffect effectiveDamageBuffEffect:
                {
                    var targets = effectiveDamageBuffEffect.Targets.Eval(this);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetCharacter(target))
                        {
                            var damagePoint = effectiveDamageBuffEffect.Value.Eval(this);
                            var takeDamageResult = target.HealthManager.TakeEffectiveDamage(damagePoint, _contextMgr.Context);

                            var result = new EffectiveAttackEvent(target, takeDamageResult);
                            appleBuffEffectEvents.Add(result);
                        }
                    }
                    break;
                }
            }
        }

        return appleBuffEffectEvents;
    }
}

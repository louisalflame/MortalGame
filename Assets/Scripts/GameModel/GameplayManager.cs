using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IGameplayStatusWatcher
{
    GameStatus GameStatus { get; }
    GameContext GameContext { get; }
}

public class GameplayManager : IGameplayStatusWatcher
{
    private GameStatus _gameStatus;

    private GameResult _gameResult;
    private List<IGameEvent> _gameEvents;
    private Queue<IGameAction> _gameActions;
    private GameContextManager _contextMgr;

    public bool IsEnd { get{ return _gameResult != null; } }
    public GameResult GameResult { get{ return _gameResult; } }
    GameStatus IGameplayStatusWatcher.GameStatus { get{ return _gameStatus; } }
    GameContext IGameplayStatusWatcher.GameContext { get{ return _contextMgr.Context; } }

    public GameplayManager(GameStatus initialStatus, GameContextManager contextManager)
    {
        // TODO split gamestatus and gamesnapshot and gameparams
        _gameStatus = initialStatus;
        _contextMgr = contextManager;
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

        _gameStatus = _gameStatus.With(
            state: GameState.TurnStart
        );
    }

    private void _TurnStart()
    {
        _gameEvents.Add(new RoundStartEvent(){
            Round = _gameStatus.Round,
            Player = _gameStatus.Ally,
            Enemy = _gameStatus.Enemy
        });
        
        var recoverEnergyPoint = _contextMgr.DispositionLibrary.GetRecoverEnergyPoint(_gameStatus.Ally.DispositionManager.CurrentDisposition);
        var allyGainEnergyResult = _gameStatus.Ally.Character.EnergyManager.RecoverEnergy(recoverEnergyPoint);
        _gameEvents.Add(new RecoverEnergyEvent(_gameStatus.Ally, allyGainEnergyResult));

        var enemyGainEnergyResult = _gameStatus.Enemy.Character.EnergyManager.RecoverEnergy(_gameStatus.Enemy.EnergyRecoverPoint);
        _gameEvents.Add(new RecoverEnergyEvent(_gameStatus.Enemy, enemyGainEnergyResult));

        _gameStatus = _gameStatus.With(
            round: _gameStatus.Round + 1,
            state: GameState.DrawCard
        );
    }

    private void _TurnDrawCard()
    {
        var allyDrawCount = _contextMgr.DispositionLibrary.GetDrawCardCount(_gameStatus.Ally.DispositionManager.CurrentDisposition);
        var enemyDrawCount = _gameStatus.Enemy.TurnStartDrawCardCount;
        _DrawCards(_gameStatus.Ally, allyDrawCount);
        _DrawCards(_gameStatus.Enemy, enemyDrawCount);

        _gameStatus = _gameStatus.With(
            state: GameState.EnemyPrepare
        );
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
                    SelectedCardInfo = new CardInfo(card, _contextMgr.Context),
                    SelectedCardInfos = _gameStatus.Enemy.SelectedCards.Cards.ToCardInfos(_contextMgr.Context)
                });
            }
        }

        _gameStatus = _gameStatus.With(
            state: GameState.PlayerPrepare
        );
    }

    private void _PlayerPrepare()
    {
        _gameEvents.Add(new PlayerExecuteStartEvent() {
            Faction = _gameStatus.Ally.Faction,
            HandCardInfo = _gameStatus.Ally.CardManager.HandCard.Cards.ToCardCollectionInfo(_contextMgr.Context)
        });

        _gameStatus = _gameStatus.With(
            state: GameState.PlayerExecute
        );
    }
    public void _PlayerExecute()
    {
        _TurnExecute(_gameStatus.Ally);
    }
    private void _EnemyExecute()
    {
        while(_gameStatus.Enemy.SelectedCards.TryDequeueCard(out ICardEntity selectedCard))
        {
            var cardRuntimCost = selectedCard.EvalCost(_contextMgr.Context);
            if (cardRuntimCost <= _gameStatus.Enemy.Character.CurrentEnergy)
            {
                _gameActions.Enqueue(new UseCardAction(){
                    CardIndentity = selectedCard.Indentity
                });
                _TurnExecute(_gameStatus.Enemy);
            }
            else
            {
                _gameEvents.Add(new EnemyUnselectedCardEvent(){
                    SelectedCardInfo = new CardInfo(selectedCard, _contextMgr.Context),
                    SelectedCardInfos = _gameStatus.Enemy.SelectedCards.Cards.ToCardInfos(_contextMgr.Context)
                });
            }
        }
        
        _FinishEnemyExecuteTurn();

        _gameStatus = _gameStatus.With(
            state: GameState.TurnEnd
        );
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
                        _UseCard(player, useCardAction.CardIndentity);
                        _gameEvents.Add(new PlayerExecuteStartEvent(){
                            Faction = player.Faction,
                            HandCardInfo = player.CardManager.HandCard.Cards.ToCardCollectionInfo(_contextMgr.Context)
                        });
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
            HandCardInfo = _gameStatus.Ally.CardManager.HandCard.Cards.ToCardCollectionInfo(_contextMgr.Context)
        });
        
        _TriggerBuffs(_gameStatus.Ally, BuffTiming.OnExecuteEnd);

        _gameActions.Clear();
        _gameStatus = _gameStatus.With(
            state: GameState.Enemy_Execute
        );
    }   
    private void _FinishEnemyExecuteTurn()
    {
        _TriggerBuffs(_gameStatus.Enemy, BuffTiming.OnExecuteEnd);

        _gameActions.Clear();
        _gameStatus = _gameStatus.With(
            state: GameState.TurnEnd
        );
    }

    private void _TurnEnd()
    {
        using(_contextMgr.SetExecutePlayer(_gameStatus.Ally))
        {
            _gameEvents.AddRange(
                _gameStatus.Ally.CardManager.ClearHandOnTurnEnd(_contextMgr));
            _gameEvents.AddRange(
                _gameStatus.Ally.CardManager.UpdateCardsOnTiming(_contextMgr, CardTiming.TurnEnd));
        }

        using(_contextMgr.SetExecutePlayer(_gameStatus.Enemy))
        {
            _gameEvents.AddRange(
                _gameStatus.Enemy.CardManager.ClearHandOnTurnEnd(_contextMgr));
            _gameEvents.AddRange(
                _gameStatus.Enemy.CardManager.UpdateCardsOnTiming(_contextMgr, CardTiming.TurnEnd));
        }

        _gameStatus = _gameStatus.With(
            state: GameState.TurnStart
        );
    }

    private void _UseCard(IPlayerEntity player, Guid CardIndentity)
    {
        using(_contextMgr.SetCardCaster(player))
        {
            var usedCard = player.CardManager.HandCard.Cards.FirstOrDefault(c => c.Indentity == CardIndentity);
            if (usedCard != null &&
                !usedCard.HasProperty(CardProperty.Sealed))
            {
                using(_contextMgr.SetUsingCard(usedCard))
                {
                    var cardRuntimCost = usedCard.EvalCost(_contextMgr.Context);
                    if (cardRuntimCost <= player.Character.CurrentEnergy)
                    {
                        var loseEnergyResult = player.Character.EnergyManager.ConsumeEnergy(cardRuntimCost);
                        _gameEvents.Add(new ConsumeEnergyEvent(player, loseEnergyResult));

                        if (_TryDiscardCard(player, usedCard)) 
                        {
                            if(usedCard.Effects.TryGetValue(CardTiming.OnPlayCard, out var onPlayEffects))
                            {
                                using(_contextMgr.SetCardTiming(CardTiming.OnPlayCard))
                                {
                                    foreach(var effect in onPlayEffects)
                                    {
                                        _ApplyCardEffect(effect);
                                    }
                                }
                            }

                            var usedCardInfo = new CardInfo(usedCard, _contextMgr.Context);
                            _gameEvents.Add(new UsedCardEvent() {
                                Faction = player.Faction,
                                UsedCardInfo = usedCardInfo,
                                HandCardInfo = player.CardManager.HandCard.Cards.ToCardCollectionInfo(_contextMgr.Context),
                                GraveyardInfo = player.CardManager.Graveyard.Cards.ToCardCollectionInfo(_contextMgr.Context)
                            });
                        }
                    }
                }
            }
        }
    }

    private void _DrawCards(IPlayerEntity player, int drawCount)
    {
        for (int i = 0; i < drawCount; i++)
        {
            if( player.CardManager.Deck.Cards.Count == 0 &&
                player.CardManager.Graveyard.Cards.Count > 0)
            {
                var graveyardCards = player.CardManager.Graveyard.PopAllCards();
                player.CardManager.Deck.EnqueueCardsThenShuffle(graveyardCards);
                _gameEvents.Add(new RecycleGraveyardEvent() {
                    Faction = player.Faction,
                    DeckInfo = player.CardManager.Deck.Cards.ToCardCollectionInfo(_contextMgr.Context),
                    GraveyardInfo = player.CardManager.Graveyard.Cards.ToCardCollectionInfo(_contextMgr.Context)
                });
            }

            if (player.CardManager.Deck.Cards.Count > 0)
                _DrawCard(player);
        }
    }

    private void _DrawCard(IPlayerEntity player)
    {
        if (player.CardManager.Deck.PopCard(out ICardEntity newCard))
        {
            player.CardManager.HandCard.AddCard(newCard);

            var newCardInfo = new CardInfo(newCard, _contextMgr.Context);
            _gameEvents.Add(new DrawCardEvent(){
                Faction = player.Faction,
                NewCardInfo = newCardInfo,
                HandCardInfo = player.CardManager.HandCard.Cards.ToCardCollectionInfo(_contextMgr.Context),
                DeckInfo = player.CardManager.Deck.Cards.ToCardCollectionInfo(_contextMgr.Context)
            });
        }
    }

    private bool _TryDiscardCard(IPlayerEntity player, ICardEntity targetCard)
    {
        if (player.CardManager.HandCard.RemoveCard(targetCard)) 
        {
            if (targetCard.HasProperty(CardProperty.Dispose) ||
                targetCard.HasProperty(CardProperty.AutoDispose))
            {
                player.CardManager.ExclusionZone.AddCard(targetCard);
            }
            else
            {
                player.CardManager.Graveyard.AddCard(targetCard);
            }

            return true;
        }

        return false;
    }

    private void _ApplyCardEffect(ICardEffect cardEffect)
    {
        using(_contextMgr.SetUsingCardEffect(cardEffect))
        {
            switch(_contextMgr.Context.UsingCardEffect)
            {
                case DamageEffect damageEffect:
                {
                    var targets = damageEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetPlayer(target))
                        {
                            var damagePoint = damageEffect.Value.Eval(_gameStatus, _contextMgr.Context);
                            var takeDamageResult = target.Character.HealthManager.TakeDamage(damagePoint, _contextMgr.Context);
                            _gameEvents.Add(new DamageEvent(target, takeDamageResult));
                        }
                    }
                    break;
                }
                case PenetrateDamageEffect penetrateDamageEffect:
                {
                    var targets = penetrateDamageEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetPlayer(target))
                        {
                            var damagePoint = penetrateDamageEffect.Value.Eval(_gameStatus, _contextMgr.Context);

                            var takeDamageResult = target.Character.HealthManager.TakePenetrateDamage(damagePoint, _contextMgr.Context);
                            _gameEvents.Add(new DamageEvent(target, takeDamageResult));
                        }
                    }
                    break;
                }
                case AdditionalAttackEffect additionalAttackEffect:
                {
                    var targets = additionalAttackEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetPlayer(target))
                        {
                            var damagePoint = additionalAttackEffect.Value.Eval(_gameStatus, _contextMgr.Context);

                            var takeDamageResult = target.Character.HealthManager.TakeAdditionalDamage(damagePoint, _contextMgr.Context);
                            _gameEvents.Add(new DamageEvent(target, takeDamageResult));
                        }
                    }
                    break;
                }
                case EffectiveAttackEffect effectiveAttackEffect:
                {
                    var targets = effectiveAttackEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetPlayer(target))
                        {
                            var damagePoint = effectiveAttackEffect.Value.Eval(_gameStatus, _contextMgr.Context);

                            var takeDamageResult = target.Character.HealthManager.TakeEffectiveDamage(damagePoint, _contextMgr.Context);
                            _gameEvents.Add(new DamageEvent(target, takeDamageResult));
                        }
                    }
                    break;
                }
                case HealEffect healEffect: 
                {
                    var targets = healEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetPlayer(target))
                        {
                            var healPoint = healEffect.Value.Eval(_gameStatus, _contextMgr.Context);

                            var getHealResult = target.Character.HealthManager.GetHeal(healPoint, _contextMgr.Context);
                            _gameEvents.Add(new GetHealEvent(target, getHealResult));
                        }
                    }
                    break;
                }
                case ShieldEffect shieldEffect:
                {
                    var targets = shieldEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetPlayer(target))
                        {
                            var shieldPoint = shieldEffect.Value.Eval(_gameStatus, _contextMgr.Context);

                            var getShieldResult = target.Character.HealthManager.GetShield(shieldPoint, _contextMgr.Context);
                            _gameEvents.Add(new GetShieldEvent(target, getShieldResult));
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
                            var gainEnergy = gainEnergyEffect.Value.Eval(_gameStatus, _contextMgr.Context);

                            var gainEnergyResult = target.Character.EnergyManager.GainEnergy(gainEnergy);
                            _gameEvents.Add(new GainEnergyEvent(target, gainEnergyResult));
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
                            var loseEnergy = loseEnegyEffect.Value.Eval(_gameStatus, _contextMgr.Context);

                            var loseEnergyResult = target.Character.EnergyManager.LoseEnergy(loseEnergy);
                            _gameEvents.Add(new LoseEnergyEvent(target, loseEnergyResult));
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
                            var drawCount = drawCardEffect.Value.Eval(_gameStatus, _contextMgr.Context);
                            _DrawCards(target, drawCount);
                        }
                    }
                    break;
                }
                case DiscardCardEffect discardCardEffect:
                {
                    var cards = discardCardEffect.TargetCards.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var card in cards)
                    {
                        card.Owner.MatchSome(cardOwner =>
                        {
                            using(_contextMgr.SetEffectTargetPlayer(cardOwner))
                            using(_contextMgr.SetEffectTargetCard(card))
                            {
                                if(_TryDiscardCard(cardOwner, card))
                                {
                                    _gameEvents.Add(new DiscardCardEvent(){
                                        Faction = cardOwner.Faction,
                                        DiscardedCardInfo = new CardInfo(card, _contextMgr.Context),
                                        HandCardInfo = cardOwner.CardManager.HandCard.Cards.ToCardCollectionInfo(_contextMgr.Context),
                                        GraveyardInfo = cardOwner.CardManager.Graveyard.Cards.ToCardCollectionInfo(_contextMgr.Context)
                                    });
                                }
                            }
                        });
                    }
                    break;
                }

                case AddBuffEffect addBuffEffect:
                {
                    var targets = addBuffEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetPlayer(target))
                        {
                            var level = addBuffEffect.Level.Eval(_gameStatus, _contextMgr.Context);
                            if (target.Character.BuffManager.AddBuff(
                                _contextMgr.BuffLibrary, 
                                _contextMgr.Context, 
                                addBuffEffect.BuffId, 
                                level,
                                out BuffEntity resultBuff))
                            {
                                _gameEvents.Add(new AddBuffEvent(target, resultBuff.ToInfo()));
                            }
                            else
                            {
                                _gameEvents.Add(new UpdateBuffEvent(target, resultBuff.ToInfo()));
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
                            if(target.Character.BuffManager.RemoveBuff(
                                _contextMgr.BuffLibrary, 
                                _contextMgr.Context, 
                                removeBuffEffect.BuffId,
                                out BuffEntity resultBuff))
                            {
                                _gameEvents.Add(new RemoveBuffEvent(target, resultBuff.ToInfo()));
                            }
                        }
                    }
                    break;
                }
            }
        }
    }

    private void _TriggerBuffs(PlayerEntity player, BuffTiming buffTiming)
    {
        var buffs = player.Character.BuffManager.Buffs;
        foreach(var buff in buffs)
        {
            using(_contextMgr.SetUsingBuff(buff))
            {
        Debug.Log($"_TriggerBuffs buff:{buff} buffTiming:{buffTiming}");
                if (buff.Effects.TryGetValue(buffTiming, out var buffEffects))
                {
                    foreach(var effect in buffEffects)
                    {
                        _ApplyBuffEffect(effect);
                    }
                }
            }
        }
    }
    private void _ApplyBuffEffect(IBuffEffect buffEffect)
    {
        using(_contextMgr.SetUsingBuffEffect(buffEffect))
        {
            switch(_contextMgr.Context.UsingBuffEffect)
            {
                case EffectiveDamageBuffEffect effectiveDamageBuffEffect:
                {
                    var targets = effectiveDamageBuffEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTargetPlayer(target))
                        {
                            var damagePoint = effectiveDamageBuffEffect.Value.Eval(_gameStatus, _contextMgr.Context);
                            var takeDamageResult = target.Character.HealthManager.TakeEffectiveDamage(damagePoint, _contextMgr.Context);
                            _gameEvents.Add(new DamageEvent(target, takeDamageResult));
                        }
                    }
                    break;
                }
            }
        }
    }
}

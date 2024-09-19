using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IGameplayStatusWatcher
{
    GameStatus GameStatus { get; }
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
    public GameStatus GameStatus { get{ return _gameStatus; } }

    public GameplayManager(GameStatus initialState, GameContextManager contextManager)
    {
        _gameStatus = initialState;
        _contextMgr = contextManager;
    }

    public void Start()
    {
        _gameEvents = new List<IGameEvent>();
        _gameActions = new Queue<IGameAction>();
        _gameResult = null;

        _gameStatus = _gameStatus.With(state: GameState.TurnStart);
        Debug.Log($"-- goto state:{_gameStatus.State} --");
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
            case GameState.TurnStart:
                _TurnStart();
                break;
            case GameState.DrawCard:
                _TurnDrawCard();
                break;
            case GameState.EnemyPrepare:
                _EnemyPreapre();
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

    private void _TurnStart()
    {
        _gameStatus = _gameStatus.With(
            round: _gameStatus.Round + 1,
            state: GameState.DrawCard
        );

        _gameEvents.Add(new RoundStartEvent(){
            Round = _gameStatus.Round,
            Player = _gameStatus.Ally,
            Enemy = _gameStatus.Enemy
        });
        Debug.Log($"-- goto state:{_gameStatus.State} --");   

        var allyGainEnergyResult = _gameStatus.Ally.Character.EnergyManager.RecoverEnergy(3);
        _gameEvents.Add(new RecoverEnergyEvent(_gameStatus.Ally, allyGainEnergyResult));

        var enemyGainEnergyResult = _gameStatus.Enemy.Character.EnergyManager.RecoverEnergy(_gameStatus.Enemy.EnergyRecoverPoint);
        _gameEvents.Add(new RecoverEnergyEvent(_gameStatus.Enemy, enemyGainEnergyResult));
    }

    private void _TurnDrawCard()
    {
        _DrawCardToMaxCount(_gameStatus.Ally);
        _DrawCardToMaxCount(_gameStatus.Enemy);

        _gameStatus = _gameStatus.With(
            state: GameState.EnemyPrepare
        );
        _gameActions.Clear();
        Debug.Log($"-- goto state:{_gameStatus.State} --");
    }

    private void _EnemyPreapre()
    {
        var recommendCards = _gameStatus.Enemy.GetRecommendCards();
        foreach(var card in recommendCards)
        {
            _gameStatus.Enemy.SelectedCards = _gameStatus.Enemy.SelectedCards.EnqueueCard(card);
            _gameEvents.Add(new EnemySelectCardEvent(){
                SelectedCardInfo = new CardInfo(card),
                SelectedCardInfos = _gameStatus.Enemy.SelectedCards.CardInfos
            });
        }

        _gameStatus = _gameStatus.With(
            state: GameState.PlayerExecute
        );
        Debug.Log($"-- goto state:{_gameStatus.State} --");
    }

    public void _PlayerExecute()
    {
        _TurnExecute(_gameStatus.Ally);
    }
    private void _EnemyExecute()
    {
        while(_gameStatus.Enemy.SelectedCards.Cards.Count > 0)
        {
            _gameStatus.Enemy.SelectedCards = _gameStatus.Enemy.SelectedCards.DequeueCard(out CardEntity selectedCard);
            
            if (selectedCard.Cost > _gameStatus.Enemy.Character.CurrentEnergy)
            {
                _gameEvents.Add(new EnemyUnselectedCardEvent(){
                    SelectedCardInfo = new CardInfo(selectedCard),
                    SelectedCardInfos = _gameStatus.Enemy.SelectedCards.CardInfos
                });
            }
            else
            {
                _gameActions.Enqueue(new UseCardAction(){
                    CardIndentity = selectedCard.Indentity
                });
                _TurnExecute(_gameStatus.Enemy);
            }
        }
        
        _FinishEnemyExecuteTurn();

        _gameStatus = _gameStatus.With(
            state: GameState.TurnEnd
        );
        Debug.Log($"-- goto state:{_gameStatus.State} --");
    }
    private void _TurnExecute(PlayerEntity player)
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
        _TriggerBuffs(_gameStatus.Ally, BuffTiming.OnExecuteEnd);

        _gameActions.Clear();
        _gameStatus = _gameStatus.With(
            state: GameState.Enemy_Execute
        );
        Debug.Log($"-- goto state:{_gameStatus.State} --");
    }   
    private void _FinishEnemyExecuteTurn()
    {
        _TriggerBuffs(_gameStatus.Enemy, BuffTiming.OnExecuteEnd);

        _gameActions.Clear();
        _gameStatus = _gameStatus.With(
            state: GameState.TurnEnd
        );
        Debug.Log($"-- goto state:{_gameStatus.State} --");
    }

    private void _TurnEnd()
    {
        var recyclAllyCards = _gameStatus.Ally.HandCard.ClearHand();
        _gameStatus.Ally.Graveyard.AddCards(recyclAllyCards);
        _gameEvents.Add(new RecycleHandCardEvent(){
            Faction = _gameStatus.Ally.Faction,
            RecycledCardInfos = recyclAllyCards.Select(c => new CardInfo(c)).ToArray(),
            HandCardInfos = _gameStatus.Ally.HandCard.CardInfos,
            GraveyardCardInfos = _gameStatus.Ally.Graveyard.CardInfos
        });

        var recyclEnemyCards = _gameStatus.Enemy.HandCard.ClearHand();
        _gameStatus.Enemy.Graveyard.AddCards(recyclEnemyCards);
        _gameEvents.Add(new RecycleHandCardEvent(){
            Faction = _gameStatus.Enemy.Faction,
            RecycledCardInfos = recyclEnemyCards.Select(c => new CardInfo(c)).ToArray(),
            HandCardInfos = _gameStatus.Enemy.HandCard.CardInfos,
            GraveyardCardInfos = _gameStatus.Enemy.Graveyard.CardInfos
        });

        //update card timing
        foreach(var c in _gameStatus.Ally.HandCard.Cards)
        {
            foreach(var property in c.Properties.Keys)
            {
                var propertyEntities = c.Properties[property].ToList();
                foreach(var propertyEntity in propertyEntities)
                {
                    if (!propertyEntity.Lifetime.NextTurn())
                    {
                        Debug.Log($"remove property:{property} from card:{c.Title}");
                        c.Properties[property].Remove(propertyEntity);
                    }
                }
            }

            c.Properties = c.Properties.Where(p => p.Value.Count > 0).ToDictionary(p => p.Key, p => p.Value);
        }
        foreach(var c in _gameStatus.Enemy.HandCard.Cards)
        {
            foreach(var property in c.Properties.Keys)
            {
                var propertyEntities = c.Properties[property].ToList();
                foreach(var propertyEntity in propertyEntities)
                {
                    if (!propertyEntity.Lifetime.NextTurn())
                    {
                        c.Properties[property].Remove(propertyEntity);
                    }
                }
            }

            c.Properties = c.Properties.Where(p => p.Value.Count > 0).ToDictionary(p => p.Key, p => p.Value);
        }

        _gameStatus = _gameStatus.With(
            state: GameState.TurnStart
        );
        Debug.Log($"-- goto state:{_gameStatus.State} --");
    }

    private void _UseCard(PlayerEntity player, Guid CardIndentity)
    {
        using(_contextMgr.SetCardCaster(player))
        {
            var usedCard = player.HandCard.Cards.FirstOrDefault(c => c.Indentity == CardIndentity);
            if (usedCard != null)
            {
                using(_contextMgr.SetUsingCard(usedCard))
                {
                    var cardRuntimCost = usedCard.Cost;
                    if (cardRuntimCost <= player.Character.CurrentEnergy)
                    {
                        var loseEnergyResult = player.Character.EnergyManager.ConsumeEnergy(cardRuntimCost);
                        _gameEvents.Add(new ConsumeEnergyEvent(player, loseEnergyResult));

                        if (player.HandCard.RemoveCard(usedCard)) 
                        {
                            player.Graveyard.AddCard(usedCard);
                            var usedCardInfo = new CardInfo(usedCard);
                            _gameEvents.Add(new UsedCardEvent() {
                                Faction = player.Faction,
                                UsedCardInfo = usedCardInfo,
                                HandCardInfos = player.HandCard.CardInfos,
                                GraveyardCardInfos = player.Graveyard.CardInfos
                            });

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
                        }
                    }
                }
            }
        }
    }

    private void _DrawCardToMaxCount(PlayerEntity player)
    {
        var needDrawCount = player.HandCard.MaxCount - player.HandCard.Cards.Count;
        _DrawCards(player, needDrawCount);
    }

    private void _DrawCards(PlayerEntity player, int drawCount)
    {
        for (int i = 0; i < drawCount; i++)
        {
            if( player.Deck.Cards.Count == 0 &&
                player.Graveyard.Cards.Count > 0)
            {
                var graveyardCards = player.Graveyard.PopAllCards();
                player.Deck.EnqueueCardsThenShuffle(graveyardCards);
                _gameEvents.Add(new RecycleGraveyardEvent() {
                    Faction = player.Faction,
                    DeckCardInfos = player.Deck.CardInfos,
                    GraveyardCardInfos = player.Graveyard.CardInfos
                });
            }

            if (player.Deck.Cards.Count > 0)
                _DrawCard(player);
        }
    }

    private void _DrawCard(PlayerEntity player)
    {
        if (player.Deck.PopCard(out CardEntity newCard))
        {
            player.HandCard.AddCard(newCard);

            var newCardInfo = new CardInfo(newCard);
            _gameEvents.Add(new DrawCardEvent(){
                Faction = player.Faction,
                NewCardInfo = newCardInfo,
                HandCardInfos = player.HandCard.CardInfos,
                DeckCardInfos = player.Deck.CardInfos,
            });
        }
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
                        using(_contextMgr.SetEffectTarget(target))
                        {
                            var damagePoint = damageEffect.Value.Eval(_gameStatus, _contextMgr.Context);
                            var takeDamageResult = target.Character.HealthManager.TakeDamage(damagePoint, _contextMgr.Context);
                            _gameEvents.Add(new TakeDamageEvent(target, takeDamageResult));
                        }
                    }
                    break;
                }
                case PenetrateDamageEffect penetrateDamageEffect:
                {
                    var targets = penetrateDamageEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTarget(target))
                        {
                            var damagePoint = penetrateDamageEffect.Value.Eval(_gameStatus, _contextMgr.Context);

                            var takeDamageResult = target.Character.HealthManager.TakePenetrateDamage(damagePoint, _contextMgr.Context);
                            _gameEvents.Add(new TakePenetrateDamageEvent(target, takeDamageResult));
                        }
                    }
                    break;
                }
                case AdditionalAttackEffect additionalAttackEffect:
                {
                    var targets = additionalAttackEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTarget(target))
                        {
                            var damagePoint = additionalAttackEffect.Value.Eval(_gameStatus, _contextMgr.Context);

                            var takeDamageResult = target.Character.HealthManager.TakeAdditionalDamage(damagePoint, _contextMgr.Context);
                            _gameEvents.Add(new TakeAdditionalDamageEvent(target, takeDamageResult));
                        }
                    }
                    break;
                }
                case EffectiveAttackEffect effectiveAttackEffect:
                {
                    var targets = effectiveAttackEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTarget(target))
                        {
                            var damagePoint = effectiveAttackEffect.Value.Eval(_gameStatus, _contextMgr.Context);

                            var takeDamageResult = target.Character.HealthManager.TakeEffectiveDamage(damagePoint, _contextMgr.Context);
                            _gameEvents.Add(new TakeEffectiveDamageEvent(target, takeDamageResult));
                        }
                    }
                    break;
                }
                case HealEffect healEffect: 
                {
                    var targets = healEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTarget(target))
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
                        using(_contextMgr.SetEffectTarget(target))
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
                        using(_contextMgr.SetEffectTarget(target))
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
                        using(_contextMgr.SetEffectTarget(target))
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
                        using(_contextMgr.SetEffectTarget(target))
                        {
                            var drawCount = drawCardEffect.Value.Eval(_gameStatus, _contextMgr.Context);
                            _DrawCards(target, drawCount);
                        }
                    }
                    break;
                }

                case AddBuffEffect addBuffEffect:
                {
                    var targets = addBuffEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                    foreach(var target in targets)
                    {
                        using(_contextMgr.SetEffectTarget(target))
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
                        using(_contextMgr.SetEffectTarget(target))
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
                        using(_contextMgr.SetEffectTarget(target))
                        {
                            var damagePoint = effectiveDamageBuffEffect.Value.Eval(_gameStatus, _contextMgr.Context);
                            var takeDamageResult = target.Character.HealthManager.TakeEffectiveDamage(damagePoint, _contextMgr.Context);
        Debug.Log($"_ApplyBuffEffect:{buffEffect} target:{target} damagePoint:{damagePoint} takeDamageResult:{takeDamageResult.DeltaHp}");
                            _gameEvents.Add(new TakeEffectiveDamageEvent(target, takeDamageResult));
                        }
                    }
                    break;
                }
            }
        }
    }
}

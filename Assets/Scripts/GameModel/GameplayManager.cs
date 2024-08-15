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

    public GameplayManager(GameStatus initialState)
    {
        _gameStatus = initialState;
    }

    public void Start()
    {
        _gameEvents = new List<IGameEvent>();
        _gameActions = new Queue<IGameAction>();
        _gameResult = null;
        _contextMgr = new GameContextManager();

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
                _TurnExecute(gameStatus.Ally);
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

        _gameStatus.Ally.Character.EnergyManager = _gameStatus.Ally.Character.EnergyManager.RecoverEnergy(3, out int deltaEnergy);
        _gameEvents.Add(new RecoverEnergyEvent(_gameStatus.Ally, deltaEnergy));

        _gameStatus.Enemy.Character.EnergyManager = _gameStatus.Enemy.Character.EnergyManager.RecoverEnergy(
            _gameStatus.Enemy.EnergyRecoverPoint, out deltaEnergy);
        _gameEvents.Add(new RecoverEnergyEvent(_gameStatus.Enemy, deltaEnergy));
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

    private void _TurnExecute(PlayerEntity player)
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
                    _FinishExecuteTurn(turnSubmitAction);
                    break;
            }
        }
    }

    private void _EnemyExecute()
    {
        while(_gameStatus.Enemy.SelectedCards.Cards.Count > 0)
        {
            _gameStatus.Enemy.SelectedCards = _gameStatus.Enemy.SelectedCards.DequeueCard(out CardEntity selectedCard);
            
            if (selectedCard.Cost > _gameStatus.Enemy.Character.EnergyManager.Energy)
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

        _gameStatus = _gameStatus.With(
            state: GameState.TurnEnd
        );
        Debug.Log($"-- goto state:{_gameStatus.State} --");
    }

    private void _TurnEnd()
    {
        _gameStatus = _gameStatus.With(
            state: GameState.TurnStart
        );
        Debug.Log($"-- goto state:{_gameStatus.State} --");
    }

    private void _UseCard(PlayerEntity player, string CardIndentity)
    {
        _contextMgr.SetCaster(player);

        var usedCard = player.HandCard.Cards.FirstOrDefault(c => c.Indentity == CardIndentity);
        if (usedCard != null)
        {
            _contextMgr.SetUsingCard(usedCard);
            var cardRuntimCost = usedCard.Cost;

            if (cardRuntimCost <= player.Character.EnergyManager.Energy)
            {
                player.Character.EnergyManager = player.Character.EnergyManager.ConsumeEnergy(cardRuntimCost, out int deltaEnergy);
                _gameEvents.Add(new ConsumeEnergyEvent(player, deltaEnergy));

                player.HandCard = player.HandCard.RemoveCard(usedCard);
                player.Graveyard = player.Graveyard.AddCard(usedCard);
                var usedCardInfo = new CardInfo(usedCard);
                _gameEvents.Add(new UsedCardEvent() {
                    Faction = player.Faction,
                    UsedCardInfo = usedCardInfo,
                    HandCardInfos = player.HandCard.CardInfos,
                    GraveyardCardInfos = player.Graveyard.CardInfos
                });

                foreach(var effect in usedCard.OnUseEffects)
                {
                    _contextMgr.SetUsingEffect(effect);
                    _ApplyOnUseEffect();
                    _contextMgr.Popout();
                }
            }

            _contextMgr.Popout();
        }

        _contextMgr.Popout();
    }

    private void _FinishExecuteTurn(TurnSubmitAction turnSubmitAction)
    {
        if (_gameStatus.State == GameState.PlayerExecute &&
            turnSubmitAction.Faction == Faction.Ally)
        {
            _gameStatus = _gameStatus.With(
                state: GameState.Enemy_Execute
            );
        }
        else if (_gameStatus.State == GameState.Enemy_Execute &&
            turnSubmitAction.Faction == Faction.Enemy)
        {
            _gameStatus = _gameStatus.With(
                state: GameState.TurnEnd
            );
        }
        Debug.Log($"-- goto state:{_gameStatus.State} --");
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
                player.Graveyard = player.Graveyard.PopAllCards(out IReadOnlyCollection<CardEntity> graveyardCards);
                player.Deck = player.Deck.EnqueueCards(graveyardCards).Shuffle();
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
        player.Deck = player.Deck.PopCard(out CardEntity newCard);
        player.HandCard = player.HandCard.AddCard(newCard);

        var newCardInfo = new CardInfo(newCard);
        _gameEvents.Add(new DrawCardEvent(){
            Faction = player.Faction,
            NewCardInfo = newCardInfo,
            HandCardInfos = player.HandCard.CardInfos,
            DeckCardInfos = player.Deck.CardInfos,
        });
    }

    private void _ApplyOnUseEffect()
    {
        switch(_contextMgr.Context.UsingEffect)
        {
            case DamageEffect damageEffect:
            {
                var targets = damageEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                foreach(var target in targets)
                {
                    _contextMgr.SetEffectTarget(target);
                    var damagePoint = damageEffect.Value.Eval(_gameStatus, _contextMgr.Context);

                    target.Character.HealthManager = target.Character.HealthManager.TakeDamage(
                        damagePoint, _contextMgr.Context, out int deltaHp, out int deltaDp);
                    _gameEvents.Add(new TakeDamageEvent(target, damagePoint, deltaHp, deltaDp));

                    _contextMgr.Popout();
                }
                break;
            }
            case PenetrateDamageEffect penetrateDamageEffect:
            {
                var targets = penetrateDamageEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                foreach(var target in targets)
                {
                    _contextMgr.SetEffectTarget(target);
                    var damagePoint = penetrateDamageEffect.Value.Eval(_gameStatus, _contextMgr.Context);

                    target.Character.HealthManager = target.Character.HealthManager.TakePenetrateDamage(
                        damagePoint, _contextMgr.Context, out int deltaHp);
                    _gameEvents.Add(new TakePenetrateDamageEvent(target, damagePoint, deltaHp));

                    _contextMgr.Popout();
                }
                break;
            }
            case AdditionalAttackEffect additionalAttackEffect:
            {
                var targets = additionalAttackEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                foreach(var target in targets)
                {
                    _contextMgr.SetEffectTarget(target);
                    var damagePoint = additionalAttackEffect.Value.Eval(_gameStatus, _contextMgr.Context);

                    target.Character.HealthManager = target.Character.HealthManager.TakeAdditionalDamage(
                        damagePoint, _contextMgr.Context, out int deltaHp, out int deltaDp);
                    _gameEvents.Add(new TakeAdditionalDamageEvent(target, damagePoint, deltaHp, deltaDp));
                    _contextMgr.Popout();
                }
                break;
            }
            case EffectiveAttackEffect effectiveAttackEffect:
            {
                var targets = effectiveAttackEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                foreach(var target in targets)
                {
                    _contextMgr.SetEffectTarget(target);
                    var damagePoint = effectiveAttackEffect.Value.Eval(_gameStatus, _contextMgr.Context);

                    target.Character.HealthManager = target.Character.HealthManager.TakeEffectiveDamage(
                        damagePoint, _contextMgr.Context, out int deltaHp);
                    _gameEvents.Add(new TakeEffectiveDamageEvent(target, damagePoint, deltaHp));
                    _contextMgr.Popout();
                }
                break;
            }
            case HealEffect healEffect: 
            {
                var targets = healEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                foreach(var target in targets)
                {
                    _contextMgr.SetEffectTarget(target);
                    var healPoint = healEffect.Value.Eval(_gameStatus, _contextMgr.Context);

                    target.Character.HealthManager = target.Character.HealthManager.GetHeal(
                        healPoint, _contextMgr.Context, out int deltaHp);
                    _gameEvents.Add(new GetHealEvent(target, healPoint, deltaHp));
                    _contextMgr.Popout();
                }
                break;
            }
            case ShieldEffect shieldEffect:
            {
                var targets = shieldEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                foreach(var target in targets)
                {
                    _contextMgr.SetEffectTarget(target);
                    var shieldPoint = shieldEffect.Value.Eval(_gameStatus, _contextMgr.Context);

                    target.Character.HealthManager = target.Character.HealthManager.GetShield(
                        shieldPoint, _contextMgr.Context, out int delatDp);
                    _gameEvents.Add(new GetShieldEvent(target, shieldPoint, delatDp));
                    _contextMgr.Popout();
                }
                break;
            }
            case GainEnergyEffect gainEnergyEffect:
            {
                var targets = gainEnergyEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                foreach(var target in targets)
                {
                    _contextMgr.SetEffectTarget(target);
                    var gainEnergy = gainEnergyEffect.Value.Eval(_gameStatus, _contextMgr.Context);

                    target.Character.EnergyManager = target.Character.EnergyManager.GainEnergy(
                        gainEnergy, out int deltaEnergy);
                    _gameEvents.Add(new GainEnergyEvent(target, deltaEnergy));
                    _contextMgr.Popout();
                }
                break;
            }
            case LoseEnegyEffect loseEnegyEffect:
            {
                var targets = loseEnegyEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                foreach(var target in targets)
                {
                    _contextMgr.SetEffectTarget(target);
                    var loseEnergy = loseEnegyEffect.Value.Eval(_gameStatus, _contextMgr.Context);

                    target.Character.EnergyManager = target.Character.EnergyManager.LoseEnergy(
                        loseEnergy, out int deltaEnergy);
                    _gameEvents.Add(new LoseEnergyEvent(target, deltaEnergy));
                    _contextMgr.Popout();
                }
                break;
            }

            // === CARD EFFECT ===
            case DrawCardEffect drawCardEffect:
            {
                var targets = drawCardEffect.Targets.Eval(_gameStatus, _contextMgr.Context);
                foreach(var target in targets)
                {
                    _contextMgr.SetEffectTarget(target);
                    var drawCount = drawCardEffect.Value.Eval(_gameStatus, _contextMgr.Context);
                    _DrawCards(target, drawCount);
                    _contextMgr.Popout();
                }
                break;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using Unity.VisualScripting;
using UnityEngine;

public interface IPlayerEntity
{
    Faction Faction { get; }
    IReadOnlyCollection<ICharacterEntity> Characters { get; }
    IPlayerCardManager CardManager { get; }
    int CurrentEnergy { get; }
    int MaxEnergy { get; }

    IEnergyManager EnergyManager { get; }
    IPlayerBuffManager BuffManager { get; }
    ICharacterEntity MainCharacter { get; }

    IGameEvent Update(IGameplayStatusWatcher gameWatcher, IActionUnit actionUnit);
}

public abstract class PlayerEntity : IPlayerEntity
{
    private readonly Faction _faction;
    private readonly IEnergyManager _energyManager;
    private readonly IPlayerBuffManager _buffManager;
    protected Option<Guid> _originPlayerInstanceGuid;
    protected IPlayerCardManager _cardManager;
    protected IReadOnlyCollection<CharacterEntity> _characters;

    public Faction Faction => _faction;
    public IEnergyManager EnergyManager => _energyManager;
    public IPlayerBuffManager BuffManager => _buffManager;
    public int CurrentEnergy => EnergyManager.Energy;
    public int MaxEnergy => EnergyManager.MaxEnergy;
    public Option<Guid> OriginPlayerInstanceGuid => _originPlayerInstanceGuid;
    public IReadOnlyCollection<ICharacterEntity> Characters => _characters;
    public IPlayerCardManager CardManager => _cardManager;


    // TODO: Implement main character with skills/assistant character
    public ICharacterEntity MainCharacter => Characters.First();

    public bool IsDummy => this == DummyPlayer;
    public static IPlayerEntity DummyPlayer = new DummyPlayer();

    public PlayerEntity(
        Faction faction,
        int currentEnergy,
        int maxEnergy
    )
    {
        _faction = faction;
        _energyManager = new EnergyManager(currentEnergy, maxEnergy);
        _buffManager = new PlayerBuffManager();
    }

    public IGameEvent Update(IGameplayStatusWatcher gameWatcher, IActionUnit actionUnit)
    {
        var updatedPlayerBuffInfos = _buffManager
            .Update(gameWatcher, actionUnit)
            .Select(buff => buff.ToInfo(gameWatcher));

        var updatedCharacterBuffInfos = _characters
            .Select(character => character.BuffManager)
            .SelectMany(buffManager => buffManager.Update(gameWatcher, actionUnit))
            .Select(buff => buff.ToInfo(gameWatcher));

        var updatedCardInfos = _cardManager
            .Update(gameWatcher, actionUnit)
            .Select(card => card.ToInfo(gameWatcher));

        return new GeneralUpdateEvent(
            updatedPlayerBuffInfos.ToList(),
            updatedCharacterBuffInfos.ToList(),
            updatedCardInfos.ToList());
    }
}

public class AllyEntity : PlayerEntity
{
    public IDispositionManager DispositionManager;

    public AllyEntity(
        Guid originPlayerInstanceGuid,
        CharacterParameter[] characterParams,
        int currentEnergy,
        int maxEnergy,
        int handCardMaxCount,
        int currentDisposition,
        int maxDisposition,
        IEnumerable<CardInstance> deckInstance) : 
        base(
            Faction.Ally, 
            currentEnergy,
            maxEnergy
        )
    {
        _originPlayerInstanceGuid = originPlayerInstanceGuid.Some();
        _characters = characterParams
            .Select(param => CharacterEntity.Create(param))
            .ToList();
        _cardManager = new PlayerCardManager(handCardMaxCount, deckInstance);
        DispositionManager = new DispositionManager(currentDisposition, maxDisposition);
    }
}

public class EnemyEntity : PlayerEntity
{
    public ISelectedCardEntity SelectedCards;
    public int EnergyRecoverPoint;
    public int TurnStartDrawCardCount;

    public EnemyEntity(
        CharacterParameter[] characterParams,
        int currentEnergy,
        int maxEnergy,
        int handCardMaxCount,
        IEnumerable<CardInstance> enemyCardInstances,
        int selectedCardMaxCount,
        int turnStartDrawCardCount,
        int energyRecoverPoint) : 
        base(
            Faction.Enemy,
            currentEnergy, 
            maxEnergy
        )
    {
        _originPlayerInstanceGuid = Option.None<Guid>();
        _characters = characterParams
            .Select(param => CharacterEntity.Create(param))
            .ToList();
        _cardManager = new PlayerCardManager(handCardMaxCount, enemyCardInstances);
        SelectedCards = new SelectedCardEntity(selectedCardMaxCount, new List<ICardEntity>());
        TurnStartDrawCardCount = turnStartDrawCardCount;
        EnergyRecoverPoint = energyRecoverPoint;
    }

    public bool TryGetRecommandSelectCard(IGameplayStatusWatcher gameplayWatcher, out ICardEntity cardEntity)
    {
        if (UseCardLogic.TryGetRecommandSelectCard(gameplayWatcher, this, out cardEntity))
        {
            return SelectedCards.TryAddCard(cardEntity);
        }
        
        cardEntity = null;
        return false;
    }

    public bool TryGetNextUseCardAction(IGameplayStatusWatcher gameplayWatcher, out UseCardAction useCardAction)
    {
        if (UseCardLogic.TryGetNextUseCardAction(gameplayWatcher, this, out useCardAction))
        {
            return CardManager.GetCard(useCardAction.CardIndentity)
                .Map(card => SelectedCards.RemoveCard(card))
                .ValueOr(false);
        }

        useCardAction = null;
        return false;
    }
}

public class DummyPlayer : PlayerEntity
{
    public DummyPlayer() : base(Faction.None, 0, 0)
    { }
}

public static class PlayerEntityExtensions
{
    public static int GetPlayerBuffAdditionProperty(
        this IPlayerEntity player, IGameplayStatusWatcher watcher, PlayerBuffProperty targetProperty)
    {
        var value = 0;
        foreach (var playerBuff in player.BuffManager.Buffs)
        {
            var triggerBuff = new PlayerBuffTrigger(playerBuff);
            foreach (var property in playerBuff.Properties)
            {
                if (property is IPlayerBuffIntegerPropertyEntity integerEntity &&
                    property.Property == targetProperty)
                {
                    value += integerEntity.Eval(watcher, triggerBuff);
                }
            }
        }
        return value;
    }
    public static float GetPlayerBuffRatioProperty(
        this IPlayerEntity player, IGameplayStatusWatcher watcher, PlayerBuffProperty targetProperty)
    {
        float ratio = 0f;
        foreach (var playerBuff in player.BuffManager.Buffs)
        {
            var triggerBuff = new PlayerBuffTrigger(playerBuff);
            foreach (var property in playerBuff.Properties)
            {
                if (property is IPlayerBuffRatioPropertyEntity ratioEntity &&
                    property.Property == targetProperty)
                {
                    ratio += ratioEntity.Eval(watcher, triggerBuff);
                }
            }
        }
        return ratio;
    }
}
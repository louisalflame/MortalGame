using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
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

    void UpdateTiming(IGameplayStatusWatcher gameWatcher, UpdateTiming timing);
    void UpdateIntent(IGameplayStatusWatcher gameWatcher, IIntentAction intent);
    void UpdateResult(IGameplayStatusWatcher gameWatcher, IResultAction result);
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

    public void UpdateTiming(IGameplayStatusWatcher gameWatcher, UpdateTiming timing)
    {
        _buffManager.UpdateTiming(gameWatcher, timing);
        _cardManager.UpdateTiming(gameWatcher, timing);

        foreach (var character in _characters.ToList())
        {
            character.BuffManager.UpdateTiming(gameWatcher, timing);
        }
    }
    public void UpdateIntent(IGameplayStatusWatcher gameWatcher, IIntentAction intent)
    {
        _buffManager.UpdateIntent(gameWatcher, intent);
        _cardManager.UpdateIntent(gameWatcher, intent);

        foreach (var character in _characters.ToList())
        {
            character.BuffManager.UpdateIntent(gameWatcher, intent);
        }
    }
    public void UpdateResult(IGameplayStatusWatcher gameWatcher, IResultAction result)
    {
        _buffManager.UpdateResult(gameWatcher, result);
        _cardManager.UpdateResult(gameWatcher, result);
        
        foreach (var character in _characters.ToList())
        {
            character.BuffManager.UpdateResult(gameWatcher, result);
        }
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

    public IReadOnlyCollection<ICardEntity> GetRecommendCards()
    {
        // TODO: Implement AI logic
        var cardList = new List<ICardEntity>();
        if(CardManager.HandCard.Cards.Count > 0) 
            cardList.Add(CardManager.HandCard.Cards.ElementAt(0));
        if(CardManager.HandCard.Cards.Count > 1) 
            cardList.Add(CardManager.HandCard.Cards.ElementAt(1));
        return cardList;
    }
}

public class DummyPlayer : PlayerEntity
{
    public DummyPlayer() : base(Faction.None, 0, 0)
    { }
}

public static class PlayerEntityExtensions
{
    public static int GetPlayerBuffProperty(this IPlayerEntity player, IGameplayStatusWatcher watcher, PlayerBuffProperty targetProperty)
    {
        var value = 0;
        foreach(var playerBuff in player.BuffManager.Buffs)
        {
            var triggerBuff = new PlayerBuffTrigger(playerBuff);
            foreach(var property in playerBuff.Properties)
            {
                if (property.Property == targetProperty)
                {
                    value += property.Eval(watcher, triggerBuff);
                }
            }
        }
        return value;
    }
}
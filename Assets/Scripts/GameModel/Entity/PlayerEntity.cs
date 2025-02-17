using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using UnityEngine;

public interface IPlayerEntity
{
    Faction Faction { get; }
    IReadOnlyCollection<CharacterEntity> Characters { get; }
    IPlayerCardManager CardManager { get; }
    int CurrentEnergy { get; }
    int MaxEnergy { get; }

    IEnergyManager EnergyManager { get; }
    IBuffManager BuffManager { get; }
    ICharacterEntity MainCharacter { get; }
}

public abstract class PlayerEntity : IPlayerEntity
{
    private readonly Faction _faction;
    private readonly IEnergyManager _energyManager;
    private readonly IBuffManager _buffManager;
    protected Option<Guid> _originPlayerInstanceGuid;
    protected IPlayerCardManager _cardManager;
    protected IReadOnlyCollection<CharacterEntity> _characters;
    
    public Faction Faction => _faction;
    public IEnergyManager EnergyManager => _energyManager;
    public IBuffManager BuffManager => _buffManager;
    public int CurrentEnergy => EnergyManager.Energy;
    public int MaxEnergy => EnergyManager.MaxEnergy;
    public Option<Guid> OriginPlayerInstanceGuid => _originPlayerInstanceGuid;
    public IReadOnlyCollection<CharacterEntity> Characters => _characters;
    public IPlayerCardManager CardManager => _cardManager;


    // TODO: Implement main character with skills/assistant character
    public ICharacterEntity MainCharacter => Characters.First();

    public bool IsDummy => this == DummyPlayer;
    public static PlayerEntity DummyPlayer = new DummyPlayer();

    public PlayerEntity(
        Faction faction,
        int currentEnergy,
        int maxEnergy
    )
    {
        _faction = faction;
        _energyManager = new EnergyManager(currentEnergy, maxEnergy);
        _buffManager = new BuffManager();
    }
}

public class AllyEntity : PlayerEntity
{
    public IDispositionManager DispositionManager;

    public AllyEntity(
        Guid originPlayerInstanceGuid,
        CharacterEntity[] characters,
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
        _characters = characters;
        _cardManager = new PlayerCardManager(handCardMaxCount, deckInstance, this);
        DispositionManager = new DispositionManager(currentDisposition, maxDisposition);
    }
}

public class EnemyEntity : PlayerEntity
{
    public ISelectedCardEntity SelectedCards;
    public int EnergyRecoverPoint;
    public int TurnStartDrawCardCount;

    public EnemyEntity(
        CharacterEntity[] characters,
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
        _characters = characters;
        _cardManager = new PlayerCardManager(handCardMaxCount, enemyCardInstances, this);
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
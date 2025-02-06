using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using UnityEngine;

public interface IPlayerEntity
{
    Faction Faction { get; }
    string NameKey { get; }
    CharacterEntity Character { get; }
    IPlayerCardManager CardManager { get; }
}

public abstract class PlayerEntity : IPlayerEntity
{
    public Guid Identity { get; protected set; }
    public Option<Guid> OriginPlayerInstanceGuid { get; protected set; }
    public Faction Faction { get; protected set; }
    public string NameKey { get; protected set; }

    public CharacterEntity Character { get; protected set; }
    public IPlayerCardManager CardManager { get; protected set; }

    public bool IsDummy => this == DummyPlayer;
    public static PlayerEntity DummyPlayer = new DummyPlayer();
}

public class AllyEntity : PlayerEntity
{
    public IDispositionManager DispositionManager;

    public AllyEntity(
        Guid originPlayerInstanceGuid,
        string nameKey,
        int currentHealth,
        int maxHealth,
        int currentEnergy,
        int maxEnergy,
        int handCardMaxCount,
        int currentDisposition,
        int maxDisposition,
        IEnumerable<CardInstance> deckInstance)
    {
        Identity = Guid.NewGuid();
        OriginPlayerInstanceGuid = originPlayerInstanceGuid.Some();
        Faction = Faction.Ally;
        NameKey = nameKey;
        Character = new CharacterEntity(currentHealth, maxHealth, currentEnergy, maxEnergy);
        CardManager = new PlayerCardManager(handCardMaxCount, deckInstance, this);
        DispositionManager = new DispositionManager(currentDisposition, maxDisposition);
    }
}

public class EnemyEntity : PlayerEntity
{
    public ISelectedCardEntity SelectedCards;
    public int EnergyRecoverPoint;
    public int TurnStartDrawCardCount;

    public EnemyEntity(
        string nameKey,
        int initialHealth,
        int maxHealth,
        int initialEnergy,
        int maxEnergy,
        int handCardMaxCount,
        IEnumerable<CardInstance> enemyCardInstances,
        int selectedCardMaxCount,
        int turnStartDrawCardCount,
        int energyRecoverPoint)
    {
        Identity = Guid.NewGuid();
        OriginPlayerInstanceGuid = Option.None<Guid>(); 
        Faction = Faction.Enemy;
        NameKey = nameKey;
        Character = new CharacterEntity(initialHealth, maxHealth, initialEnergy, maxEnergy);
        CardManager = new PlayerCardManager(handCardMaxCount, enemyCardInstances, this);
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
    public DummyPlayer()
    {
        Faction = Faction.None;
    }
}
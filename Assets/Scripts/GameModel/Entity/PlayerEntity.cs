using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IPlayerEntity
{
    Faction Faction { get; }
    string Name { get; }
    CharacterEntity Character { get; }
    IPlayerCardManager CardManager { get; }
}

public abstract class PlayerEntity : IPlayerEntity
{
    public Faction Faction { get; protected set; }
    public string Name { get; protected set; }

    public CharacterEntity Character { get; protected set; }
    public IPlayerCardManager CardManager { get; protected set; }

    public bool IsDummy => this == DummyPlayer;
    public static PlayerEntity DummyPlayer = new DummyPlayer();
}

public class AllyEntity : PlayerEntity
{
    public IDispositionManager DispositionManager;

    public AllyEntity(
        string nameKey,
        int currentHealth,
        int maxHealth,
        int currentEnergy,
        int maxEnergy,
        int handCardMaxCount,
        int currentDisposition,
        IEnumerable<CardInstance> deckInstance)
    {
        Faction = Faction.Ally;
        Name = nameKey;
        Character = new CharacterEntity(currentHealth, maxHealth, currentEnergy, maxEnergy);
        CardManager = new PlayerCardManager(handCardMaxCount, deckInstance);
        DispositionManager = new DispositionManager(currentDisposition);
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
        Faction = Faction.Enemy;
        Name = nameKey;
        Character = new CharacterEntity(initialHealth, maxHealth, initialEnergy, maxEnergy);
        CardManager = new PlayerCardManager(handCardMaxCount, enemyCardInstances);
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
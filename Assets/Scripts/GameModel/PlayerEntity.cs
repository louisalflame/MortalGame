using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PlayerEntity
{
    public Faction Faction { get; protected set; }
    public string Name;

    public CharacterEntity Character;
    public IHandCardEntity HandCard;
    public IDeckEntity Deck;
    public IGraveyardEntity Graveyard;

    public bool IsDummy => this == DummyPlayer;
    public static PlayerEntity DummyPlayer = new DummyPlayer();
}

public class AllyEntity : PlayerEntity
{
    public DispositionManager DispositionManager;

    public AllyEntity()
    {
        Faction = Faction.Ally;
    }
}

public class EnemyEntity : PlayerEntity
{
    public SelectedCardEntity SelectedCards;
    public int EnergyRecoverPoint;

    public EnemyEntity()
    {
        Faction = Faction.Enemy;
    }

    public IReadOnlyCollection<CardEntity> GetRecommendCards()
    {
        // TODO: Implement AI logic
        var cardList = new List<CardEntity>();
        if(HandCard.Cards.Count > 0) 
            cardList.Add(HandCard.Cards.ElementAt(0));
        if(HandCard.Cards.Count > 1) 
            cardList.Add(HandCard.Cards.ElementAt(1));
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
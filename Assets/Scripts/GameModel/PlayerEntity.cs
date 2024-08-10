using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Faction
{
    None = 0,
    Ally,
    Enemy
}

public abstract class PlayerEntity
{
    public Faction Faction { get; protected set; }
    public string Name;

    public CharacterEntity Character;
    public HandCardEntity HandCard;
    public DeckEntity Deck;
    public GraveyardEntity Graveyard;
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
    public IReadOnlyCollection<CardEntity> SelectedCards;
    public int EnergyRecoverPoint;

    public EnemyEntity()
    {
        Faction = Faction.Enemy;
        SelectedCards = new List<CardEntity>();
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

    public void PreparedSelectedCards()
    {
        if(HandCard.Cards.Count == 0) 
            return;

        var cards = new List<CardEntity>();
        cards.Add(HandCard.Cards.ElementAt(0));
        SelectedCards = cards;
    }
}
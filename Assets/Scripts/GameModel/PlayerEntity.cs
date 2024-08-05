using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Faction
{
    None = 0,
    Ally,
    Enemy}

public abstract class PlayerEntity
{
    public Faction Faction { get; protected set; }
    public string Name;

    public CharacterEntity Character;
    public HandCardEntity HandCard;
    public DeckEntity Deck;
    public CardGraveyardEntity Graveyard;
}

public class AllyEntity : PlayerEntity
{
    public AllyEntity()
    {
        Faction = Faction.Ally;
    }
}

public class EnemyEntity : PlayerEntity
{
    public IReadOnlyCollection<CardEntity> SelectedCards;

    public EnemyEntity()
    {
        Faction = Faction.Enemy;
        SelectedCards = new List<CardEntity>();
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
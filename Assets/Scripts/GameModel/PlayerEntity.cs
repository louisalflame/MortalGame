using UnityEngine;

public enum Faction
{
    None = 0,
    Player,
    Enemy
}

public class PlayerEntity
{
    public Faction Faction;
    public string Name;

    public CharacterEntity Character;
    public HandCardEntity HandCard;
    public DeckEntity Deck;
    public CardGraveyardEntity Graveyard;
}
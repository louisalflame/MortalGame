using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CardType
{
    None = 0,
    Attack,
    Defense,
    Speech,
    Sneak,
    Special
}

public class CardEntity
{
    public int CardIndentity;
    public string Title;
    public string Info;

    public CardType Type;
    public int Cost;
    public int Power;

}

public class HandCardEntity
{
    public int MaxCount;
    public IReadOnlyCollection<CardEntity> Cards;

    public IReadOnlyCollection<CardInfo> CardInfos =>
        Cards.Select(c => new CardInfo(c)).ToArray();

    public HandCardEntity()
    {
        MaxCount = 5;
        Cards = new List<CardEntity>();
    }

    public HandCardEntity AddCard(CardEntity card)
    {
        var cards = new List<CardEntity>(Cards.Append(card));

        return new HandCardEntity
        {
            MaxCount = MaxCount,
            Cards = cards
        };
    }

    public HandCardEntity RemoveCard(CardEntity card)
    {
        var cards = new List<CardEntity>(Cards.Where(c => c.CardIndentity != card.CardIndentity));

        return new HandCardEntity
        {
            MaxCount = MaxCount,
            Cards = cards
        };
    }
}

public class DeckEntity
{
    public IReadOnlyCollection<CardEntity> Cards;

    public IReadOnlyCollection<CardInfo> CardInfos =>
        Cards.Select(c => new CardInfo(c)).ToArray();
    
    public DeckEntity()
    {
        Cards = new List<CardEntity>();
    }

    public DeckEntity PopCard(out CardEntity card)
    {
        card = Cards.ElementAt(0);

        return new DeckEntity{
            Cards = Cards.Skip(1).ToArray()
        };
    }

    public DeckEntity Shuffle()
    {
        var cards = new List<CardEntity>(Cards);
        for (int i = 0; i < cards.Count; i++)
        {
            var temp = cards[i];
            var randomIndex = Random.Range(0, cards.Count);
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }

        return new DeckEntity
        {
            Cards = cards
        };
    }
}

public class CardGraveyardEntity
{
    public IReadOnlyCollection<CardEntity> Cards;

    public IReadOnlyCollection<CardInfo> CardInfos =>
        Cards.Select(c => new CardInfo(c)).ToArray();
        
    public CardGraveyardEntity()
    {
        Cards = new List<CardEntity>();
    }

    public CardGraveyardEntity AddCard(CardEntity card)
    {
        var cards = new List<CardEntity>(Cards);
        cards.Add(card);

        return new CardGraveyardEntity
        {
            Cards = cards
        };
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


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
    public DeckEntity EnqueueCards(IReadOnlyCollection<CardEntity> cards)
    {
        var newCards = new List<CardEntity>(Cards);
        newCards.AddRange(cards);

        return new DeckEntity
        {
            Cards = newCards
        };
    }
    public DeckEntity Shuffle()
    {
        var newCards = new List<CardEntity>(Cards);
        for (int i = 0; i < newCards.Count; i++)
        {
            var temp = newCards[i];
            var randomIndex = Random.Range(0, newCards.Count);
            newCards[i] = newCards[randomIndex];
            newCards[randomIndex] = temp;
        }

        return new DeckEntity
        {
            Cards = newCards
        };
    }
}
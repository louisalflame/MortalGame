using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraveyardEntity
{
    public IReadOnlyCollection<CardEntity> Cards;

    public IReadOnlyCollection<CardInfo> CardInfos =>
        Cards.Select(c => new CardInfo(c)).ToArray();
        
    public GraveyardEntity()
    {
        Cards = new List<CardEntity>();
    }

    public GraveyardEntity AddCard(CardEntity card)
    {
        var cards = new List<CardEntity>(Cards);
        cards.Add(card);

        return new GraveyardEntity
        {
            Cards = cards
        };
    }
    public GraveyardEntity AddCards(IEnumerable<CardEntity> cards)
    {
        var cardList = cards.ToList();
        var newCards = new List<CardEntity>(Cards);
        newCards.AddRange(cardList);

        return new GraveyardEntity
        {
            Cards = newCards
        };
    }

    public GraveyardEntity PopAllCards(out IReadOnlyCollection<CardEntity> allCards)
    {
        allCards = Cards;

        return new GraveyardEntity
        {
            Cards = new List<CardEntity>()
        };
    }
}
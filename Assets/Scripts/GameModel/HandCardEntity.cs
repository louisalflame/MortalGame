using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandCardEntity
{
    public int MaxCount;
    public IReadOnlyCollection<CardEntity> Cards;

    public IReadOnlyCollection<CardInfo> CardInfos =>
        Cards.Select(c => new CardInfo(c)).ToArray();

    public HandCardEntity()
    {
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


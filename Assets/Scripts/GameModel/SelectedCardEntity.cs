using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectedCardEntity
{
    public int MaxCount;
    public IReadOnlyCollection<CardEntity> Cards;

    public IReadOnlyCollection<CardInfo> CardInfos =>
        Cards.Select(c => new CardInfo(c)).ToArray();

    public SelectedCardEntity()
    {
        Cards = new List<CardEntity>();
    }
 
    public SelectedCardEntity EnqueueCard(CardEntity card)
    {
        var newCards = new List<CardEntity>(Cards);
        newCards.Add(card);

        return new SelectedCardEntity
        {
            MaxCount = MaxCount,
            Cards = newCards
        };
    }

    public SelectedCardEntity DequeueCard(out CardEntity card)
    {
        card = Cards.FirstOrDefault();
        var newCards = new List<CardEntity>(Cards.Skip(1));

        return new SelectedCardEntity
        {
            MaxCount = MaxCount,
            Cards = newCards
        };
    }
}


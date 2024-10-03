using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CardInfo
{
    public Guid Indentity { get; private set; }
    public string Title { get; private set; }
    public string Info { get; private set; }
    public int Cost { get; private set; }
    public int Power { get; private set; }

    public CardInfo(CardEntity card)
    {
        Indentity = card.Indentity;
        Title = card.Title;
        Info = card.Info;
        Cost = card.Cost;
        Power = card.Power;
    }
}

public class CardCollectionInfo
{
    public IReadOnlyDictionary<CardInfo, int> CardInfos { get; private set; }

    public int Count => CardInfos.Count;

    public CardCollectionInfo(IReadOnlyDictionary<CardInfo, int> cardInfos)
    {
        CardInfos = cardInfos;
    }
}

public static class CardCollectionInfoUtility
{
    public static CardCollectionInfo ToCardCollectionInfo(this IReadOnlyCollection<CardInfo> cardInfos)
    {
        return new CardCollectionInfo(
            cardInfos
                .Select((info, index) => (info, index))
                .ToDictionary(
                    pair => pair.Item1,
                    pair => pair.index));
    }
}
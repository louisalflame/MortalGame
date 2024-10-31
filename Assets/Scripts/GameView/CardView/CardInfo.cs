using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CardInfo
{
    public Guid Indentity { get; private set; }
    public string Title { get; private set; }
    public string Info { get; private set; }
    public int OriginCost { get; private set; }
    public int Cost { get; private set; }
    public int OriginPower { get; private set; }
    public int Power { get; private set; }

    public List<CardPropertyInfo> Properties { get; private set; }
    public List<CardPropertyInfo> AppendProperties { get; private set; }

    public CardInfo(CardEntity card, GameContext gameContext)
    {
        Indentity = card.Indentity;
        Title = card.Title;
        Info = card.Info;

        OriginCost = card.Cost;
        Cost = card.EvalCost(gameContext);
        OriginPower = card.Power;
        Power = card.EvalPower(gameContext);

        Properties = card.Properties.Select(p => new CardPropertyInfo(p)).ToList();
        AppendProperties = card.AppendProperties.Select(p => new CardPropertyInfo(p)).ToList();
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

    public static CardCollectionInfo ToCardCollectionInfo(this IReadOnlyCollection<CardEntity> cards, GameContext gameContext)
    { 
        return cards
            .Select(c => new CardInfo(c, gameContext))
            .ToArray()
            .ToCardCollectionInfo();
    }

    public static IReadOnlyCollection<CardInfo> ToCardInfos(this IReadOnlyCollection<CardEntity> cards, GameContext gameContext)
    {
        return cards.Select(c => new CardInfo(c, gameContext)).ToArray();
    }
}
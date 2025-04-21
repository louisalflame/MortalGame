using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CardInfo
{
    public Guid Identity { get; private set; }
    public string CardDataID { get; private set; }
    public int OriginCost { get; private set; }
    public int Cost { get; private set; }
    public int OriginPower { get; private set; }
    public int Power { get; private set; }

    public MainSelectableInfo MainSelectable;
    public List<SubSelectableInfo> SubSelectables;

    public List<CardBuffInfo> StatusInfos { get; private set; }

    public CardInfo(ICardEntity card, IGameplayStatusWatcher gameWatcher)
    {
        Identity = card.Identity;
        CardDataID = card.CardDataId;

        OriginCost = card.Cost;
        Cost = card.EvalCost(gameWatcher);
        OriginPower = card.Power;
        Power = card.EvalPower(gameWatcher);

        MainSelectable = new MainSelectableInfo(card.MainSelectable.SelectType);
        SubSelectables = card.SubSelectables.Select(s => new SubSelectableInfo(s.SelectType, s.TargetCount)).ToList();

        StatusInfos = card.BuffList.Select(s => new CardBuffInfo(s)).ToList();
    }
}

public class CardCollectionInfo
{
    public CardCollectionType Type { get; private set; }
    public IReadOnlyDictionary<CardInfo, int> CardInfos { get; private set; }

    public int Count => CardInfos.Count;

    public CardCollectionInfo( 
        CardCollectionType type,
        IReadOnlyDictionary<CardInfo, int> cardInfos)
    {
        Type = type;
        CardInfos = cardInfos;
    }
}

public static class CardCollectionInfoUtility
{
    public static CardCollectionInfo ToCardCollectionInfo(this IReadOnlyCollection<CardInfo> cardInfos, CardCollectionType type)
    {
        return new CardCollectionInfo(
            type,
            cardInfos
                .Select((info, index) => (info, index))
                .ToDictionary(
                    pair => pair.Item1,
                    pair => pair.index));
    }

    public static CardCollectionInfo ToCardCollectionInfo(
        this ICardColletionZone cardCollectionZone, IGameplayStatusWatcher gameWatcher)
    { 
        return cardCollectionZone.Cards
            .Select(c => new CardInfo(c, gameWatcher))
            .ToArray()
            .ToCardCollectionInfo(cardCollectionZone.Type);
    }

    public static IReadOnlyCollection<CardInfo> ToCardInfos(
        this IReadOnlyCollection<ICardEntity> cards, IGameplayStatusWatcher gameWatcher)
    {
        return cards.Select(c => new CardInfo(c, gameWatcher)).ToArray();
    }
}
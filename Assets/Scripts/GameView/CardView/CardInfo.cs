using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CardInfo
{
    public Guid Indentity { get; private set; }
    public string CardDataID { get; private set; }
    public int OriginCost { get; private set; }
    public int Cost { get; private set; }
    public int OriginPower { get; private set; }
    public int Power { get; private set; }

    public MainSelectableInfo MainSelectable;
    public List<SubSelectableInfo> SubSelectables;

    public List<CardStatusInfo> StatusInfos { get; private set; }

    public CardInfo(ICardEntity card, GameContext gameContext)
    {
        Indentity = card.Indentity;
        CardDataID = card.CardDataId;

        OriginCost = card.Cost;
        Cost = card.EvalCost(gameContext);
        OriginPower = card.Power;
        Power = card.EvalPower(gameContext);

        MainSelectable = new MainSelectableInfo(card.MainSelectable.TargetType);
        SubSelectables = card.SubSelectables.Select(s => new SubSelectableInfo(s.TargetType, s.TargetCount)).ToList();

        StatusInfos = card.StatusList.Select(s => new CardStatusInfo(s)).ToList();
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

    public static CardCollectionInfo ToCardCollectionInfo(this IReadOnlyCollection<ICardEntity> cards, GameContext gameContext)
    { 
        return cards
            .Select(c => new CardInfo(c, gameContext))
            .ToArray()
            .ToCardCollectionInfo();
    }

    public static IReadOnlyCollection<CardInfo> ToCardInfos(this IReadOnlyCollection<ICardEntity> cards, GameContext gameContext)
    {
        return cards.Select(c => new CardInfo(c, gameContext)).ToArray();
    }
}
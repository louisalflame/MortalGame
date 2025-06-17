using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CardInfo
{
    public Guid Identity { get; private set; }
    public string CardDataID { get; private set; }
    public CardType Type { get; private set; }
    public CardRarity Rarity { get; private set; }
    public IEnumerable<CardTheme> Themes { get; private set; }
    public int OriginCost { get; private set; }
    public int Cost { get; private set; }
    public int OriginPower { get; private set; }
    public int Power { get; private set; }

    public MainSelectableInfo MainSelectable;
    public List<SubSelectableInfo> SubSelectables;

    public List<CardBuffInfo> BuffInfos { get; private set; }
    public List<CardProperty> Properties { get; private set; }

    public CardInfo(ICardEntity card, IGameplayStatusWatcher gameWatcher)
    {
        Identity = card.Identity;
        CardDataID = card.CardDataId;
        Type = card.Type;
        Rarity = card.Rarity;
        Themes = card.Themes;

        OriginCost = card.OriginCost;
        Cost = GameFormula.CardCost(gameWatcher, card, new CardLookIntentAction(card));
        OriginPower = card.OriginPower;
        Power = GameFormula.CardPower(gameWatcher, card, new CardLookIntentAction(card));

        MainSelectable = new MainSelectableInfo(card.MainSelectable.SelectType);
        SubSelectables = card.SubSelectables.Select(s => new SubSelectableInfo(s.SelectType, s.TargetCount)).ToList();

        BuffInfos = card.BuffManager.Buffs.Select(s => new CardBuffInfo(s)).ToList();
        Properties = card.Properties.Select(p => p.Property)
            .Concat(card.BuffManager.Buffs
                .SelectMany(b => b.Properties.Select(p => p.Property)))
            .Distinct()
            .ToList();
    }

    public const string KEY_COST = "cost";
    public const string KEY_POWER = "power";

    public Dictionary<string, string> GetTemplateValues()
    {
        return new Dictionary<string, string>()
        {
            { KEY_COST, Cost.ToString() },
            { KEY_POWER, Power.ToString() },
        };
    }
}

public class CardCollectionInfo
{
    public CardCollectionType Type { get; private set; }
    public IReadOnlyDictionary<CardInfo, int> CardInfos { get; private set; }
    public static readonly CardCollectionInfo Empty = new CardCollectionInfo(
        CardCollectionType.None,
        new Dictionary<CardInfo, int>());

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
    public static CardInfo ToInfo(
        this ICardEntity card, IGameplayStatusWatcher gameWatcher)
    {
        return new CardInfo(card, gameWatcher);
    }

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
            .Select(c => c.ToInfo(gameWatcher))
            .ToArray()
            .ToCardCollectionInfo(cardCollectionZone.Type);
    }

    public static IReadOnlyCollection<CardInfo> ToCardInfos(
        this IReadOnlyCollection<ICardEntity> cards, IGameplayStatusWatcher gameWatcher)
    {
        return cards.Select(c => c.ToInfo(gameWatcher)).ToArray();
    }
}
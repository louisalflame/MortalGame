using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public record CardInfo(
    Guid Identity,
    string CardDataID,
    CardType Type,
    CardRarity Rarity,
    IEnumerable<CardTheme> Themes,
    int OriginCost,
    int Cost,
    int OriginPower,
    int Power,
    MainSelectionInfo MainSelectable,
    IReadOnlyList<CardBuffInfo> BuffInfos,
    IReadOnlyList<CardProperty> Properties,
    IReadOnlyList<string> Keywords)
{
    public static CardInfo Create(ICardEntity card, IGameplayStatusWatcher gameWatcher)
    {
        return new CardInfo(
            Identity: card.Identity,
            CardDataID: card.CardDataId,
            Type: card.Type,
            Rarity: card.Rarity,
            Themes: card.Themes,
            OriginCost: card.OriginCost,
            Cost: GameFormula.CardCost(gameWatcher, card, new CardLookIntentAction(card), new CardTrigger(card)),
            OriginPower: card.OriginPower,
            Power: GameFormula.CardPower(gameWatcher, card, new CardLookIntentAction(card), new CardTrigger(card)),
            MainSelectable: card.MainSelect.ToInfo(),
            BuffInfos: card.BuffManager.Buffs.Select(s => new CardBuffInfo(s)).ToList(),
            Properties: card.Properties.Select(p => p.Property)
                .Concat(card.BuffManager.Buffs
                    .SelectMany(b => b.Properties.Select(p => p.Property)))
                .Distinct()
                .ToList(),
            Keywords: card.Properties.SelectMany(p => p.Keywords)
                .Concat(card.BuffManager.Buffs.SelectMany(b => b.Keywords))
                .Distinct()
                .ToList()
        );
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

public record CardCollectionInfo(
    CardCollectionType Type,
    IReadOnlyDictionary<CardInfo, int> CardInfos)
{
    public static readonly CardCollectionInfo Empty = new CardCollectionInfo(
        CardCollectionType.None,
        new Dictionary<CardInfo, int>());

    public int Count => CardInfos.Count;
}

public static class CardCollectionInfoUtility
{
    public static CardInfo ToInfo(
        this ICardEntity card, IGameplayStatusWatcher gameWatcher)
    {
        return CardInfo.Create(card, gameWatcher);
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


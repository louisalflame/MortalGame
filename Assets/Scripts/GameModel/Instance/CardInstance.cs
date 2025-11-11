using System;
using System.Collections.Generic;
using System.Linq;

public record CardInstance(
    Guid InstanceGuid,
    string CardDataId,
    CardRarity Rarity,
    CardType Type,
    IReadOnlyList<CardTheme> Themes,
    int Cost,
    int Power,
    MainTargetSelectLogic MainSelect,
    IReadOnlyList<SubTargetSelectLogic> SubSelects,
    IReadOnlyList<ICardEffect> Effects,
    IReadOnlyDictionary<CardTriggeredTiming, ICardEffect[]> TriggeredEffects,
    IReadOnlyList<ICardPropertyData> PropertyDatas)
{
    public static CardInstance Create(CardData cardData)
    {
        return new CardInstance(
            InstanceGuid: Guid.NewGuid(),
            CardDataId: cardData.ID,
            Rarity: cardData.Rarity,
            Type: cardData.Type,
            Themes: cardData.Themes.ToList(),
            Cost: cardData.Cost,
            Power: cardData.Power,
            MainSelect: cardData.MainSelect,
            SubSelects: cardData.SubSelects.ToList(),
            Effects: cardData.Effects.ToList(),
            TriggeredEffects: cardData.TriggeredEffects.ToDictionary(
                pair => pair.Timing,
                pair => pair.Effects.ToArray()
            ),
            PropertyDatas: cardData.PropertyDatas.ToList()
        );
    }
}

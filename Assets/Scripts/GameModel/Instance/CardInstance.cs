using System;
using System.Collections.Generic;
using System.Linq;

public class CardInstance
{
    public Guid InstanceGuid;
    public string CardDataId;

    public CardRarity Rarity;
    public CardType Type;
    public CardTheme[] Themes;

    public int Cost;
    public int Power;

    public IMainTargetSelectable MainSelectable;
    public List<ISubTargetSelectable> SubSelectables;
    public IReadOnlyDictionary<TriggerTiming, ICardEffect[]> Effects;
    public List<ICardPropertyData> PropertyDatas;

    public static CardInstance Create(CardData cardData)
    {
        return new CardInstance()
        {
            InstanceGuid = Guid.NewGuid(),
            Rarity = cardData.Rarity,
            Type = cardData.Type,
            Themes = cardData.Themes.ToArray(),
            Cost = cardData.Cost,
            Power = cardData.Power,
            MainSelectable = cardData.MainSelectable,
            SubSelectables = cardData.SubSelectables.ToList(),
            Effects = cardData.CardEffects.ToDictionary(
                pair => pair.Timing,
                pair => pair.Effects.ToArray()
            ),
            PropertyDatas = cardData.PropertyDatas.ToList(),
            CardDataId = cardData.ID,
        };
    }
}

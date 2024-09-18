using System;
using System.Collections.Generic;
using System.Linq;

public class CardInstance
{
    public Guid InstanceGuid;
    public string TitleKey;
    public string InfoKey;

    public CardRarity Rarity;
    public CardType Type;
    public CardTheme[] Themes;

    public int Cost;
    public int Power;

    public List<ITargetSelectable> Selectables;
    public ICardEffect[] OnUseEffects;
    public ICardEffect[] OnHandEffects;
    public ICardEffect[] OnDeckEffects;
    public IReadOnlyDictionary<CardTiming, ICardEffect[]> Effects;
    public IReadOnlyDictionary<CardProperty, CardPropertyData[]> PropertyDatas;

    public string OriginCardDataId;

    public static CardInstance Create(CardData cardData)
    {
        return new CardInstance()
        {
            InstanceGuid = Guid.NewGuid(),
            TitleKey = cardData.TitleKey,
            InfoKey = cardData.InfoKey,
            Rarity = cardData.Rarity,
            Type = cardData.Type,
            Themes = cardData.Themes.ToArray(),
            Cost = cardData.Cost,
            Power = cardData.Power,
            Selectables = cardData.Selectables.ToList(),
            Effects = cardData.Effects.ToDictionary(
                pair => pair.Key,
                pair => pair.Value.ToArray()
            ),
            PropertyDatas = cardData.PropertyDatas.ToDictionary(
                pair => pair.Key,
                pair => pair.Value.ToArray()
            ),
            OriginCardDataId = cardData.ID,
        };
    }
}

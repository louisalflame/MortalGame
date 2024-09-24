using System;
using System.Collections.Generic;
using System.Linq;

public class CardInstance
{
    public Guid InstanceGuid;
    public string CardDataId;
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
    public List<ICardPropertyData> PropertyDatas;

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
            PropertyDatas = cardData.PropertyDatas.ToList(),
            CardDataId = cardData.ID,
        };
    }
}

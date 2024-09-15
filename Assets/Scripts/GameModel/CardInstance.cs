using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardInstance
{
    public string InstanceId;
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

    public string OriginCardDataId;

    public static CardInstance Create(CardData cardData)
    {
        return new CardInstance()
        {
            InstanceId = System.Guid.NewGuid().ToString(),
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
            OriginCardDataId = cardData.ID,
        };
    }
}

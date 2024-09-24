using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor.PackageManager;
using UnityEngine;


public class CardEntity
{
    public Guid Indentity;
    public Guid OriginCardInstanceGuid;
    public string CardDataId;
    public string Title;
    public string Info;

    public CardType Type;
    public CardRarity Rarity;
    public CardTheme[] Themes = new CardTheme[0];
    public int Cost;
    public int Power;
    
    public List<ITargetSelectable> Selectables;

    public Dictionary<CardTiming, List<ICardEffect>> Effects;
    public List<ICardPropertyEntity> Properties;

    public bool IsDummy => this == DummyCard;
    public static CardEntity DummyCard = new CardEntity()
    {
        Indentity = Guid.Empty,
    };

    public static CardEntity Create(CardInstance cardInstance)
    {
        return new CardEntity(){
            Indentity = Guid.NewGuid(),
            CardDataId = cardInstance.CardDataId,
            Title = cardInstance.TitleKey,
            Info = cardInstance.InfoKey,
            Type = cardInstance.Type,
            Rarity = cardInstance.Rarity,
            Themes = cardInstance.Themes.ToArray(),
            Cost = cardInstance.Cost,
            Power = cardInstance.Power,
            Selectables = cardInstance.Selectables.ToList(),
            Effects = cardInstance.Effects.ToDictionary(
                pair => pair.Key,
                pair => pair.Value.ToList()
            ),
            Properties = cardInstance.PropertyDatas.Select(p => p.CreateEntity()).ToList(),
            OriginCardInstanceGuid = cardInstance.InstanceGuid,
        };
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;


public class CardEntity
{
    public Guid Indentity;
    public string Title;
    public string Info;

    public CardType Type;
    public CardRarity Rarity;
    public CardTheme[] Themes = new CardTheme[0];
    public int Cost;
    public int Power;
    
    public IReadOnlyCollection<ITargetSelectable> Selectables;

    public IReadOnlyDictionary<CardTiming, List<ICardEffect>> Effects;
    public IReadOnlyDictionary<CardProperty, List<CardPropertyEntity>> Properties;

    public Guid OriginCardInstanceGuid;

    public bool IsDummy => this == DummyCard;
    public static CardEntity DummyCard = new CardEntity()
    {
        Indentity = Guid.Empty,
    };
}
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;


public class CardEntity
{
    public string Indentity;
    public string Title;
    public string Info;

    public CardType Type;
    public CardRarity Rarity;
    public CardTheme[] Themes = new CardTheme[0];
    public int Cost;
    public int Power;
    
    public IReadOnlyCollection<ITargetSelectable> Selectables;

    public IReadOnlyCollection<ICardEffect> OnUseEffects;

    public string OriginCardInstanceId;

    public bool IsDummy => this == DummyCard;
    public static CardEntity DummyCard = new CardEntity()
    {
        Indentity = string.Empty,
    };
}


using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CardData
{
    [BoxGroup("Identification")]
    public string ID;

    [Space]
    [TitleGroup("BasicData")]
    public CardRarity Rarity;
    [TitleGroup("BasicData")]
    public CardType Type;
    [TitleGroup("BasicData")]
    public CardTheme[] Themes;
    [TitleGroup("BasicData")]
    [Range(0, 10)]
    public int Cost;
    [TitleGroup("BasicData")]
    [Range(0, 20)]
    public int Power;
    
    [TitleGroup("Target")]
    [PropertySpace(SpaceBefore = 0, SpaceAfter = 10)]
    public List<ITargetSelectable> Selectables = new List<ITargetSelectable>();

    [BoxGroup("Effects")]
    public Dictionary<CardTiming, ICardEffect[]> Effects = new Dictionary<CardTiming, ICardEffect[]>();

    [TitleGroup("Localization")]
    public string TitleKey = string.Empty;
    public string InfoKey = string.Empty;
}
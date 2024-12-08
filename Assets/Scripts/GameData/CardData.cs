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
    
    [BoxGroup("Target")]
    [PropertySpace(SpaceBefore = 0, SpaceAfter = 10)]
    public IMainTargetSelectable MainSelectable;
    [BoxGroup("Target")]
    public List<ISubTargetSelectable> SubSelectables;

    [BoxGroup("Effects")]
    public Dictionary<CardTiming, ICardEffect[]> Effects = new Dictionary<CardTiming, ICardEffect[]>();

    [BoxGroup("Properties")]
    public List<ICardPropertyData> PropertyDatas = new List<ICardPropertyData>();
}

using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CardData
{
    [Serializable]
    public class CardEffect
    {
        [TableColumnWidth(150, false)]
        [ValueDropdown("_GetAllowedValues")]
        public GameTiming Timing;

        public ICardEffect[] Effects;

        private static IEnumerable _GetAllowedValues()
        {
            return new[] { 
                GameTiming.None,
                GameTiming.TurnStart,
                GameTiming.TurnEnd,
                GameTiming.DrawCard,
                GameTiming.PlayCard
            };
        }
    }

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
 
    [TableList]
    public List<CardEffect> CardEffects = new List<CardEffect>();

    [BoxGroup("Properties")]
    public List<ICardPropertyData> PropertyDatas = new List<ICardPropertyData>();

    public void OnValidate()
    {
        if (CardEffects == null) return;
        
        var keys = new HashSet<GameTiming>();
        CardEffects.RemoveAll(pair => 
        {
            if (keys.Contains(pair.Timing))
            {
                return true;
            }

            keys.Add(pair.Timing);
            return false;
        });
    }
}
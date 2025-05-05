using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CardData
{
    [Serializable]
    public class TriggeredCardEffect
    {
        [TableColumnWidth(150, false)]
        [ValueDropdown("_GetAllowedValues")]
        public TriggerTiming Timing;

        [ShowInInspector]
        // TODO: conditional cardeffect
        public ICardEffect[] Effects = new ICardEffect[0];

        private static IEnumerable _GetAllowedValues()
        {
            return new[] { 
                TriggerTiming.None,
                TriggerTiming.TurnStart,
                TriggerTiming.TurnEnd,
                TriggerTiming.DrawCard,
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
    [ShowInInspector]
    [BoxGroup("Target")]
    public List<ISubTargetSelectable> SubSelectables = new();
 
    [BoxGroup("Effects")]
    [ShowInInspector]
    public List<ICardEffect> Effects = new();    
    [BoxGroup("Effects")]
    [ShowInInspector]
    [TableList]
    public List<TriggeredCardEffect> TriggeredEffects = new();

    [ShowInInspector]
    [BoxGroup("Properties")]
    public List<ICardPropertyData> PropertyDatas = new();

    public void OnValidate()
    {
        if (TriggeredEffects == null) return;
        
        var keys = new HashSet<TriggerTiming>();
        TriggeredEffects.RemoveAll(pair => 
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
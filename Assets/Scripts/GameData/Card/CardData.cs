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
        public GameTiming Timing;

        [ShowInInspector]
        // TODO: conditional cardeffect
        public ICardEffect[] Effects = new ICardEffect[0];

        private static IEnumerable _GetAllowedValues()
        {
            return new[] {
                GameTiming.TurnStart,
                GameTiming.TurnEnd,
                GameTiming.DrawCard,
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
    [ShowInInspector]
    public CardTheme[] Themes = new CardTheme[0];
    [TitleGroup("BasicData")]
    [Range(0, 10)]
    public int Cost;
    [TitleGroup("BasicData")]
    [Range(0, 20)]
    public int Power;

    [ShowInInspector]
    [BoxGroup("Target")]
    public MainTargetSelectLogic MainSelect = new ();
    [ShowInInspector]
    [BoxGroup("Target")]
    public List<SubTargetSelectLogic> SubSelects = new();

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

        var keys = new HashSet<GameTiming>();
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

[Serializable]
public class MainTargetSelectLogic
{
    public IMainTargetSelectable MainSelectable = new NoneSelectable();
    public TargetLogicTag LogicTag = TargetLogicTag.None;
}
[Serializable]
public class SubTargetSelectLogic
{ 
    public ISubTargetSelectable SubSelectable = new NoneSelectable();
    public TargetLogicTag LogicTag = TargetLogicTag.None;
}
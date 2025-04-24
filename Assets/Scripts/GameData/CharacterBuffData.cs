using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CharacterBuffData
{
    [Serializable]
    public class BuffEffect
    {
        [TableColumnWidth(150, false)]
        [ValueDropdown("_GetAllowedValues")]
        public TriggerTiming Timing;

        [ShowInInspector]
        public ConditionalCharacterBuffEffect[] ConditionEffects = new ConditionalCharacterBuffEffect[0];

        private static IEnumerable _GetAllowedValues()
        {
            return new[] { 
                TriggerTiming.None,
                TriggerTiming.TurnStart,
                TriggerTiming.TurnEnd,
                TriggerTiming.ExecuteStart,
                TriggerTiming.ExecuteEnd,
            };
        }
    }
    
    [TitleGroup("BasicData")]
    public string ID;

    [TitleGroup("BasicData")]
    public int MaxLevel;
 
    [ShowInInspector]
    [BoxGroup("Effects")]
    public List<IReactionSessionData> Sessions = new();
    
    [Space(20)]
    [ShowInInspector]
    [TableList]
    [BoxGroup("Effects")]
    public List<BuffEffect> BuffEffects = new();  

    [ShowInInspector]
    [BoxGroup("Properties")]
    public List<ICharacterBuffPropertyData> PropertyDatas = new();

    [BoxGroup("LifeTime")]
    public ICharacterBuffLifeTimeData LifeTimeData;

    public void OnValidate()
    {
        if (BuffEffects == null) return;
        
        var keys = new HashSet<TriggerTiming>();
        BuffEffects.RemoveAll(pair => 
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
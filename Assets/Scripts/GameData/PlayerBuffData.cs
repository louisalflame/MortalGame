using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerBuffData
{
    [Serializable]
    public class BuffEffect
    {
        [TableColumnWidth(150, false)]
        [ValueDropdown("_GetAllowedValues")]
        public GameTiming Timing;

        [ShowInInspector]
        public ConditionalEffect[] ConditionEffects = new ConditionalEffect[0];

        private static IEnumerable _GetAllowedValues()
        {
            return new[] { 
                GameTiming.None,
                GameTiming.TurnStart,
                GameTiming.TurnEnd,
                GameTiming.ExecuteStart,
                GameTiming.ExecuteEnd,
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
    
    [ShowInInspector]
    [BoxGroup("Effects")]
    [TableList]
    public List<BuffEffect> BuffEffects = new();  

    [ShowInInspector]
    [BoxGroup("Properties")]
    public List<IPlayerBuffPropertyData> PropertyDatas = new();

    [BoxGroup("LifeTime")]
    public IPlayerBuffLifeTimeData LifeTimeData;

    public void OnValidate()
    {
        if (BuffEffects == null) return;
        
        var keys = new HashSet<GameTiming>();
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
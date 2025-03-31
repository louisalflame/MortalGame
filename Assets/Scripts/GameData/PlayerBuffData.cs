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

        public IPlayerBuffEffect[] Effects;

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
 
    [BoxGroup("Effects")]
    [TableList]
    public List<BuffEffect> BuffEffects = new List<BuffEffect>();

    [BoxGroup("Properties")]
    public List<PlayerBuffPropertyValue> PropertyDatas = new List<PlayerBuffPropertyValue>();

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
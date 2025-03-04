using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerBuffData
{
    [TitleGroup("BasicData")]
    public string ID;

    [TitleGroup("BasicData")]
    public int MaxLevel;
 
    [BoxGroup("Effects")]
    public Dictionary<BuffTiming, IPlayerBuffEffect[]> Effects = new Dictionary<BuffTiming, IPlayerBuffEffect[]>();

    [BoxGroup("Properties")]
    public List<PlayerBuffPropertyValue> PropertyDatas = new List<PlayerBuffPropertyValue>();

    [BoxGroup("LifeTime")]
    public IPlayerBuffLifeTimeData LifeTimeData;
}
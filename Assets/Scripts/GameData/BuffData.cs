using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BuffData
{
    [TitleGroup("BasicData")]
    public string ID;

    [TitleGroup("BasicData")]
    public int MaxLevel;
 
    [BoxGroup("Effects")]
    public Dictionary<BuffTiming, IBuffEffect[]> Effects = new Dictionary<BuffTiming, IBuffEffect[]>();

    [BoxGroup("Properties")]
    public List<BuffPropertyData> PropertyDatas = new List<BuffPropertyData>();
}

public class BuffPropertyData
{
    public BuffProperty Property;
    public PropertyDuration Duration;
    public IIntegerValue Value;
}

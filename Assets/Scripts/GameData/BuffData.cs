using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BuffData
{
    [TitleGroup("BasicData")]
    public string ID;

    [TitleGroup("BasicData")]
    public int MaxLevel;
 
    public Dictionary<BuffTiming, IBuffEffect[]> Effects = new Dictionary<BuffTiming, IBuffEffect[]>();
}

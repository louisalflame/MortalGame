using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CardStatusData
{
    [BoxGroup("Identification")]
    public string ID;

    [TitleGroup("Effects")]
    public Dictionary<CardTiming, ICardEffect[]> Effects = new Dictionary<CardTiming, ICardEffect[]>();

    [TitleGroup("Properties")]
    public List<ICardPropertyData> PropertyDatas;

    [TitleGroup("LifeTime")]
    public ICardStatusLifeTimeData LifeTimeData;
}

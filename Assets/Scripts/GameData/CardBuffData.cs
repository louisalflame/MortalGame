using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CardBuffData
{
    [BoxGroup("Identification")]
    public string ID;

    [ShowInInspector]
    [TitleGroup("Effects")]
    public Dictionary<TriggerTiming, ICardEffect[]> Effects = new ();

    [ShowInInspector]
    [TitleGroup("Properties")]
    public List<ICardPropertyData> PropertyDatas = new ();

    [TitleGroup("LifeTime")]
    public ICardBuffLifeTimeData LifeTimeData;
}


[Serializable]
public class AddCardBuffData
{
    [ValueDropdown("@DropdownHelper.CardBuffNames")]
    public string CardBuffId;
}
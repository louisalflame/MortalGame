using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CardStatusData
{
    [BoxGroup("Identification")]
    public string ID;

    [ShowInInspector]
    [TitleGroup("Effects")]
    public Dictionary<GameTiming, ICardEffect[]> Effects = new ();

    [ShowInInspector]
    [TitleGroup("Properties")]
    public List<ICardPropertyData> PropertyDatas = new ();

    [TitleGroup("LifeTime")]
    public ICardStatusLifeTimeData LifeTimeData;
}


[Serializable]
public class AddCardStatusData
{
    [ValueDropdown("@DropdownHelper.CardStatusNames")]
    public string CardStatusId;
    public IIntegerValue Power;
    public IIntegerValue Times;
}
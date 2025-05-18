using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CharacterBuffData
{
    [TitleGroup("BasicData")]
    public string ID;

    [TitleGroup("BasicData")]
    public int MaxLevel;
 
    [ShowInInspector]
    [BoxGroup("Effects")]
    public List<IReactionSessionData> Sessions = new();
    
    [Space(20)]
    [ShowInInspector]
    [BoxGroup("Effects")]
    public Dictionary<TriggerTiming, ConditionalCharacterBuffEffect[]> BuffEffects = new();  

    [ShowInInspector]
    [BoxGroup("Properties")]
    public List<ICharacterBuffPropertyData> PropertyDatas = new();

    [BoxGroup("LifeTime")]
    public ICharacterBuffLifeTimeData LifeTimeData;
}
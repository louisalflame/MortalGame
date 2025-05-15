using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class ConditionalPlayerBuffEffect
{
    [ShowInInspector]
    public IPlayerBuffCondition[] Conditions = new IPlayerBuffCondition[0];

    [Space(20)]
    public IPlayerBuffEffect Effect;
}

public class EffectiveDamagePlayerBuffEffect : IPlayerBuffEffect
{
    public ITargetCharacterCollectionValue Targets;
    public IIntegerValue Value;
}

public class AddCardBuffPlayerBuffEffect : IPlayerBuffEffect
{
    ITargetCardCollectionValue Targets;
    
    [ShowInInspector]
    public List<AddCardBuffData> AddCardBuffDatas = new ();
}
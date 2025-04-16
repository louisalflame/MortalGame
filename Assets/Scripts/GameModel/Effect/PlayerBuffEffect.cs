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

public class EffectiveDamageBuffEffect : IPlayerBuffEffect
{
    public ITargetCharacterCollectionValue Targets;
    public IIntegerValue Value;
}
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class ConditionalEffect
{
    [ShowInInspector]
    public ICondition[] Conditions = new ICondition[0];

    [Space(20)]
    public IPlayerBuffEffect Effect;
}

public class EffectiveDamageBuffEffect : IPlayerBuffEffect
{
    public ITargetCharacterCollectionValue Targets;
    public IIntegerValue Value;
}
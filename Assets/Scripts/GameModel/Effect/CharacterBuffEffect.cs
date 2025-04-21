using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class ConditionalCharacterBuffEffect
{
    [ShowInInspector]
    public ICharacterBuffCondition[] Conditions = new ICharacterBuffCondition[0];

    [Space(20)]
    public ICharacterBuffEffect Effect;
}

public class EffectiveDamageCharacterBuffEffect : ICharacterBuffEffect
{
    public ITargetCharacterCollectionValue Targets;
    public IIntegerValue Value;
}
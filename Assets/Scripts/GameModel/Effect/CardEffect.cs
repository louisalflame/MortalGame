using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public interface ICardEffect
{
    
}

// ==============================
// Target-Character Effect
// ==============================
[Serializable]
public class DamageEffect : ICardEffect
{
    public ITargetCharacterCollectionValue Targets;
    public IIntegerValue Value;
}
[Serializable]
public class PenetrateDamageEffect : ICardEffect
{
    public ITargetCharacterCollectionValue Targets;
    public IIntegerValue Value;
}
[Serializable]
public class AdditionalAttackEffect : ICardEffect 
{
    public ITargetCharacterCollectionValue Targets;
    public IIntegerValue Value;
}
[Serializable]
public class EffectiveAttackEffect : ICardEffect
{
    public ITargetCharacterCollectionValue Targets;
    public IIntegerValue Value;
}

[Serializable]
public class ShieldEffect : ICardEffect
{
    public ITargetCharacterCollectionValue Targets;
    public IIntegerValue Value;
}
[Serializable]
public class HealEffect : ICardEffect
{
    public ITargetCharacterCollectionValue Targets;
    public IIntegerValue Value;
}

// ==============================
// Target-Player Effect
// ==============================
[Serializable]
public class GainEnergyEffect : ICardEffect
{
    public ITargetPlayerCollectionValue Targets;
    public IIntegerValue Value;
}
[Serializable]
public class LoseEnegyEffect : ICardEffect
{
    public ITargetPlayerCollectionValue Targets;
    public IIntegerValue Value;
}

[Serializable]
public class AddBuffEffect : ICardEffect
{
    public ITargetPlayerCollectionValue Targets;
    [ValueDropdown("@DropdownHelper.BuffNames")]
    public string BuffId;
    public IIntegerValue Level;
}
[Serializable]
public class RemoveBuffEffect : ICardEffect
{
    public ITargetPlayerCollectionValue Targets;
    [ValueDropdown("@DropdownHelper.BuffNames")]
    public string BuffId;
}

// ==============================
// Target-Card Effect
// ==============================
[Serializable]
public class DrawCardEffect : ICardEffect
{
    public ITargetPlayerCollectionValue Targets;
    public IIntegerValue Value;
}
[Serializable]
public class DiscardCardEffect : ICardEffect
{
    public ITargetCardCollectionValue TargetCards;
}
[Serializable]
public class ConsumeCardEffect : ICardEffect
{
    public ITargetCardCollectionValue TargetCards;
}
[Serializable]
public class DisposeCardEffect : ICardEffect
{
    public ITargetCardCollectionValue TargetCards;
}
[Serializable]
public class CreateCardEffect : ICardEffect
{
    public ITargetPlayerValue Target;
    [ShowInInspector]
    public List<CardDataScriptable> CardDatas = new ();    
    [ShowInInspector]
    public List<AddCardBuffData> AddCardBuffDatas = new ();
    public CardCollectionType CreateDestination;
}
[Serializable]
public class CloneCardEffect : ICardEffect
{
    public ITargetPlayerValue Target;
    public ITargetCardCollectionValue ClonedCards;
    [ShowInInspector]
    public List<AddCardBuffData> AddCardBuffDatas = new ();
    public CardCollectionType CloneDestination;
}
[Serializable]
public class AppendCardBuffEffect : ICardEffect
{
    public ITargetCardCollectionValue TargetCards;
    [ShowInInspector]
    public List<AddCardBuffData> AddCardBuffDatas = new ();
}
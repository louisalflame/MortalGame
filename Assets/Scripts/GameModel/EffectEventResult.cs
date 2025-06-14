using System.Collections.Generic;
using Optional;
using UnityEngine;

public interface IEffectResult
{
}

public class TakeDamageResult : IEffectResult
{
    public DamageType Type;
    public int DamagePoint;
    public int DeltaHp;
    public int DeltaDp;
    public int OverHp;
}
public class GetHealResult : IEffectResult
{
    public int HealPoint;
    public int DeltaHp;
    public int OverHp;
}
public class GetShieldResult : IEffectResult
{
    public int ShieldPoint;
    public int DeltaDp;
    public int OverDp;
}

public class GetEnergyResult : IEffectResult
{
    public EnergyGainType Type;
    public int EnergyPoint;
    public int DeltaEp;
    public int OverEp;
}
public class LoseEnergyResult : IEffectResult
{
    public EnergyLoseType Type;
    public int EnergyPoint;
    public int DeltaEp;
    public int OverEp;
}

public class AddPlayerBuffResult : IEffectResult
{
    public bool IsNewBuff;
    public IPlayerBuffEntity Buff;
    public int DeltaLevel;
}
public class AddCharacterBuffResult : IEffectResult
{
    public bool IsNewBuff;
    public ICharacterBuffEntity Buff;
    public int DeltaLevel;
}
public class AddCardBuffResult : IEffectResult
{
    public bool IsNewBuff;
    public ICardBuffEntity Buff;
    public int DeltaLevel;
}

public class RemovePlayerBuffResult : IEffectResult
{
    public IReadOnlyCollection<IPlayerBuffEntity> Buffs;
}
public class RemoveCharacterBuffResult : IEffectResult
{
    public IReadOnlyCollection<ICharacterBuffEntity> Buff;
}
public class RemoveCardBuffResult : IEffectResult
{
    public IReadOnlyCollection<ICardBuffEntity> Buff;
}


public class CreateCardResult : IEffectResult
{
    public ICardEntity Card;
    public ICardColletionZone Zone;
    public AddCardBuffResult[] AddBuffs;
}
public class CloneCardResult : IEffectResult
{
    public ICardEntity OriginCard;
    public ICardEntity Card;
    public ICardColletionZone Zone;
    public AddCardBuffResult[] AddBuffs;
}
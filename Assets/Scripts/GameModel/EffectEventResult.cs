using Optional;
using UnityEngine;

public class TakeDamageResult
{
    public DamageType Type;
    public int DamagePoint;
    public int DeltaHp;
    public int DeltaDp;
    public int OverHp;
}
public class GetHealResult
{
    public int HealPoint;
    public int DeltaHp;
    public int OverHp;
}
public class GetShieldResult
{
    public int ShieldPoint;
    public int DeltaDp;
    public int OverDp;
}

public class GetEnergyResult
{
    public EnergyGainType Type;
    public int EnergyPoint;
    public int DeltaEp;
    public int OverEp;
}
public class LoseEnergyResult
{
    public EnergyLoseType Type;
    public int EnergyPoint;
    public int DeltaEp;
    public int OverEp;
}

public class AddPlayerBuffResult
{
    public bool IsNewBuff;
    public IPlayerBuffEntity Buff;
    public int DeltaLevel;
}
public class AddCharacterBuffResult
{
    public bool IsNewBuff;
    public ICharacterBuffEntity Buff;
    public int DeltaLevel;
}
public class AddCardBuffResult
{
    public bool IsNewBuff;
    public ICardBuffEntity Buff;
    public int DeltaLevel;
}

public class RemovePlayerBuffResult
{
    public Option<IPlayerBuffEntity> Buff;
}
public class RemoveCharacterBuffResult
{
    public Option<ICharacterBuffEntity> Buff;
}
public class RemoveCardBuffResult
{
    public Option<ICardBuffEntity> Buff;
}


public class CreateCardResult
{
    public ICardEntity Card;
    public ICardColletionZone Zone;
    public AddCardBuffResult[] AddBuffs;
}
public class CloneCardResult
{
    public ICardEntity OriginCard;
    public ICardEntity Card;
    public ICardColletionZone Zone;
    public AddCardBuffResult[] AddBuffs;
}
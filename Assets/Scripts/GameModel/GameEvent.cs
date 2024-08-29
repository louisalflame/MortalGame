using System.Collections.Generic;
using UnityEngine;

public interface IGameEvent
{

}

public class RoundStartEvent : IGameEvent
{
    public int Round;
    public AllyEntity Player;
    public EnemyEntity Enemy;
}
public class RecycleGraveyardEvent : IGameEvent
{
    public Faction Faction;
    public IReadOnlyCollection<CardInfo> DeckCardInfos;
    public IReadOnlyCollection<CardInfo> GraveyardCardInfos;
}
public class RecycleHandCardEvent : IGameEvent
{
    public Faction Faction;    
    public IReadOnlyCollection<CardInfo> RecycledCardInfos;
    public IReadOnlyCollection<CardInfo> HandCardInfos;
    public IReadOnlyCollection<CardInfo> GraveyardCardInfos;
}
public class DrawCardEvent : IGameEvent
{
    public Faction Faction;
    public CardInfo NewCardInfo;
    public IReadOnlyCollection<CardInfo> DeckCardInfos;
    public IReadOnlyCollection<CardInfo> HandCardInfos;
}
public class EnemySelectCardEvent : IGameEvent
{
    public CardInfo SelectedCardInfo;
    public IReadOnlyCollection<CardInfo> SelectedCardInfos;
}
public class EnemyUnselectedCardEvent : IGameEvent
{ 
    public CardInfo SelectedCardInfo;
    public IReadOnlyCollection<CardInfo> SelectedCardInfos;
}
public class UsedCardEvent : IGameEvent
{
    public Faction Faction;
    public CardInfo UsedCardInfo;
    public IReadOnlyCollection<CardInfo> HandCardInfos;
    public IReadOnlyCollection<CardInfo> GraveyardCardInfos;
}

public abstract class EnergyEvent : IGameEvent
{
    public Faction Faction;
    public int Energy;
    public int DeltaEnergy;
    public int MaxEnergy;

    public EnergyEvent(PlayerEntity player, int deltaEnergy)
    {
        Faction = player.Faction;
        Energy = player.Character.EnergyManager.Energy;
        DeltaEnergy = deltaEnergy;
        MaxEnergy = player.Character.EnergyManager.MaxEnergy;
    }
}
public class RecoverEnergyEvent : EnergyEvent
{
    public RecoverEnergyEvent(PlayerEntity player, int deltaEnergy) : base(player, deltaEnergy) { }
}
public class ConsumeEnergyEvent : EnergyEvent
{
    public ConsumeEnergyEvent(PlayerEntity player, int deltaEnergy) : base(player, deltaEnergy) { }
}
public class GainEnergyEvent : EnergyEvent
{
    public GainEnergyEvent(PlayerEntity player, int deltaEnergy) : base(player, deltaEnergy) { }
}
public class LoseEnergyEvent : EnergyEvent
{
    public LoseEnergyEvent(PlayerEntity player, int deltaEnergy) : base(player, deltaEnergy) { }
}

public abstract class HealthEvent : IGameEvent
{
    public Faction Faction;
    public int Hp;
    public int Dp;
    public int MaxHp;

    public HealthEvent(PlayerEntity player)
    {
        Faction = player.Faction;
        Hp = player.Character.HealthManager.Hp;
        Dp = player.Character.HealthManager.Dp;
        MaxHp = player.Character.HealthManager.MaxHp;
    }
}
public class TakeDamageEvent : HealthEvent
{
    public int DeltaHp;
    public int DeltaShield;
    public int DamagePoint;

    public TakeDamageEvent(PlayerEntity player, int damagePoint, int deltaHp, int deltaShield) : base(player)
    {
        DeltaHp = deltaHp;
        DeltaShield = deltaShield;
        DamagePoint = damagePoint;
    }
}
public class TakePenetrateDamageEvent : HealthEvent
{
    public int DeltaHp;
    public int DamagePoint;

    public TakePenetrateDamageEvent(PlayerEntity player, int damagePoint, int deltaHp) : base(player)
    {
        DeltaHp = deltaHp;
        DamagePoint = damagePoint;
    }
}
public class TakeAdditionalDamageEvent : HealthEvent
{
    public int DeltaHp;
    public int DeltaShield;
    public int DamagePoint;

    public TakeAdditionalDamageEvent(PlayerEntity player, int damagePoint, int deltaHp, int deltaShield) : base(player)
    {
        DeltaHp = deltaHp;
        DeltaShield = deltaShield;
        DamagePoint = damagePoint;
    }
}
public class TakeEffectiveDamageEvent : HealthEvent
{
    public int DeltaHp;
    public int DamagePoint;

    public TakeEffectiveDamageEvent(PlayerEntity player, int damagePoint, int deltaHp) : base(player)
    {
        DeltaHp = deltaHp;
        DamagePoint = damagePoint;
    }
}

public class GetHealEvent : HealthEvent
{
    public int DeltaHp;
    public int HealPoint;

    public GetHealEvent(PlayerEntity player, int healPoint, int deltaHp) : base(player)
    {
        DeltaHp = deltaHp;
        HealPoint = healPoint;
    }
}
public class GetShieldEvent : HealthEvent
{
    public int DeltaShield;
    public int ShieldPoint;

    public GetShieldEvent(PlayerEntity player, int shieldPoint, int deltaShield) : base(player)
    {
        DeltaShield = deltaShield;
        ShieldPoint = shieldPoint;
    }
}
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
        Energy = player.Character.CurrentEnergy;
        DeltaEnergy = deltaEnergy;
        MaxEnergy = player.Character.MaxEnergy;
    }
}
public class RecoverEnergyEvent : EnergyEvent
{
    public RecoverEnergyEvent(PlayerEntity player, GainEnergyResult gainEnergyResult) : base(player, gainEnergyResult.DeltaEp) { }
}
public class ConsumeEnergyEvent : EnergyEvent
{
    public ConsumeEnergyEvent(PlayerEntity player, LoseEnergyResult loseEnergyResult) : base(player, loseEnergyResult.DeltaEp) { }
}
public class GainEnergyEvent : EnergyEvent
{
    public GainEnergyEvent(PlayerEntity player, GainEnergyResult gainEnergyResult) : base(player, gainEnergyResult.DeltaEp) { }
}
public class LoseEnergyEvent : EnergyEvent
{
    public LoseEnergyEvent(PlayerEntity player, LoseEnergyResult loseEnergyResult) : base(player, loseEnergyResult.DeltaEp) { }
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
        Hp = player.Character.CurrentHealth;
        Dp = player.Character.CurrentArmor;
        MaxHp = player.Character.MaxHealth;
    }
}
public class TakeDamageEvent : HealthEvent
{
    public int DeltaHp;
    public int DeltaShield;
    public int DamagePoint;

    public TakeDamageEvent(PlayerEntity player, TakeDamageResult takeDamageResult) : base(player)
    {
        DeltaHp = takeDamageResult.DeltaHp;
        DeltaShield = takeDamageResult.DeltaDp;
        DamagePoint = takeDamageResult.DamagePoint;
    }
}
public class TakePenetrateDamageEvent : HealthEvent
{
    public int DeltaHp;
    public int DamagePoint;

    public TakePenetrateDamageEvent(PlayerEntity player, TakeDamageResult takeDamageResult) : base(player)
    { 
        DeltaHp = takeDamageResult.DeltaHp;
        DamagePoint = takeDamageResult.DamagePoint;
    }
}
public class TakeAdditionalDamageEvent : HealthEvent
{
    public int DeltaHp;
    public int DeltaShield;
    public int DamagePoint;

    public TakeAdditionalDamageEvent(PlayerEntity player, TakeDamageResult takeDamageResult) : base(player)
    {
        DeltaHp = takeDamageResult.DeltaHp;
        DeltaShield = takeDamageResult.DeltaDp;
        DamagePoint = takeDamageResult.DamagePoint;
    }
}
public class TakeEffectiveDamageEvent : HealthEvent
{
    public int DeltaHp;
    public int DamagePoint;

    public TakeEffectiveDamageEvent(PlayerEntity player, TakeDamageResult takeDamageResult) : base(player)
    {
        DeltaHp = takeDamageResult.DeltaHp;
        DamagePoint = takeDamageResult.DamagePoint;
    }
}

public class GetHealEvent : HealthEvent
{
    public int DeltaHp;
    public int HealPoint;

    public GetHealEvent(PlayerEntity player, GetHealResult getHealResult) : base(player)
    {
        DeltaHp = getHealResult.DeltaHp;
        HealPoint = getHealResult.HealPoint;
    }
}
public class GetShieldEvent : HealthEvent
{
    public int DeltaShield;
    public int ShieldPoint;

    public GetShieldEvent(PlayerEntity player, GetShieldResult getShieldResult) : base(player)
    {
        DeltaShield = getShieldResult.DeltaDp;
        ShieldPoint = getShieldResult.ShieldPoint;
    }
}

public class AddBuffEvent : IGameEvent
{
    public Faction Faction;
    public BuffEntity Buff;

    public AddBuffEvent(PlayerEntity player, BuffEntity buff)
    {
        Faction = player.Faction;
        Buff = buff;
    }
}
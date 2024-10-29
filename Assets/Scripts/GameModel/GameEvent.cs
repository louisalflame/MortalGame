using System.Collections.Generic;
using UnityEngine;

public interface IGameEvent
{

}

public class AllySummonEvent : IGameEvent
{
    public AllyEntity Player;
}
public class EnemySummonEvent : IGameEvent
{
    public EnemyEntity Enemy;
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
    public CardCollectionInfo DeckInfo;
    public CardCollectionInfo GraveyardInfo;
}
public class RecycleHandCardEvent : IGameEvent
{
    public Faction Faction;    
    public IReadOnlyCollection<CardInfo> RecycledCardInfos;
    public IReadOnlyCollection<CardInfo> ExcludedCardInfos;
    public CardCollectionInfo HandCardInfo;
    public CardCollectionInfo GraveyardInfo;
}
public class DrawCardEvent : IGameEvent
{
    public Faction Faction;
    public CardInfo NewCardInfo;
    public CardCollectionInfo DeckInfo;
    public CardCollectionInfo HandCardInfo;
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
public class PlayerExecuteStartEvent : IGameEvent
{
    public Faction Faction;
    public CardCollectionInfo HandCardInfo;
}
public class PlayerExecuteEndEvent : IGameEvent
{
    public Faction Faction;
    public CardCollectionInfo HandCardInfo;
}
public class UsedCardEvent : IGameEvent
{
    public Faction Faction;
    public CardInfo UsedCardInfo;
    public CardCollectionInfo HandCardInfo;
    public CardCollectionInfo GraveyardInfo;
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
public class DamageEvent : HealthEvent
{
    public DamageType Type;
    public int DeltaHp;
    public int DeltaShield;
    public int DamagePoint;

    public DamageEvent(PlayerEntity player, TakeDamageResult takeDamageResult) : base(player)
    {
        Type = takeDamageResult.Type;
        DeltaHp = takeDamageResult.DeltaHp;
        DeltaShield = takeDamageResult.DeltaDp;
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
    public BuffInfo Buff;

    public AddBuffEvent(PlayerEntity player, BuffInfo buff)
    {
        Faction = player.Faction;
        Buff = buff;
    }
}
public class UpdateBuffEvent : IGameEvent
{
    public Faction Faction;
    public BuffInfo Buff;

    public UpdateBuffEvent(PlayerEntity player, BuffInfo buff)
    {
        Faction = player.Faction;
        Buff = buff;
    }
}
public class RemoveBuffEvent : IGameEvent
{
    public Faction Faction;
    public BuffInfo Buff;

    public RemoveBuffEvent(PlayerEntity player, BuffInfo buff)
    {
        Faction = player.Faction;
        Buff = buff;
    }
}
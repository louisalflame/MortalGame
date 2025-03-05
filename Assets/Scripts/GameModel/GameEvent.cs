using System;
using System.Collections.Generic;
using UnityEngine;

public interface IGameEvent
{

}

public class NoneEvent : IGameEvent
{
    public static readonly NoneEvent Instance = new NoneEvent();
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
    public CardCollectionInfo ExclusionZoneInfo;
    public CardCollectionInfo DisposeZoneInfo;
}
public class DrawCardEvent : IGameEvent
{
    public Faction Faction;
    public CardInfo NewCardInfo;
    public CardCollectionInfo DeckInfo;
    public CardCollectionInfo HandCardInfo;
}

public abstract class MoveCardEvent : IGameEvent
{
    public Faction Faction;
    public CardInfo CardInfo;
    public CardCollectionInfo StartZoneInfo;
    public CardCollectionInfo DestinationZoneInfo;

    public MoveCardEvent(ICardEntity card, IGameplayStatusWatcher gameWatcher, ICardColletionZone start, ICardColletionZone destination)
    {
        Faction = card.Owner.Faction;
        CardInfo = new CardInfo(card, gameWatcher);
        StartZoneInfo = start.ToCardCollectionInfo(gameWatcher);
        DestinationZoneInfo = destination.ToCardCollectionInfo(gameWatcher);
    }
}
public abstract class AddCardEvent : IGameEvent
{
    public Faction Faction;
    public CardInfo CardInfo;
    public CardCollectionInfo DestinationZoneInfo;

    public AddCardEvent(ICardEntity card,IGameplayStatusWatcher gameWatcher, ICardColletionZone destination)
    {
        Faction = card.Owner.Faction;
        CardInfo = new CardInfo(card, gameWatcher);
        DestinationZoneInfo = destination.ToCardCollectionInfo(gameWatcher);
    }
}
public class DiscardCardEvent : MoveCardEvent
{
    public DiscardCardEvent(ICardEntity card, IGameplayStatusWatcher gameWatcher, ICardColletionZone start, ICardColletionZone destination) :
        base(card, gameWatcher, start, destination) { }
}
public class ConsumeCardEvent : MoveCardEvent
{
    public ConsumeCardEvent(ICardEntity card, IGameplayStatusWatcher gameWatcher, ICardColletionZone start, ICardColletionZone destination) :
        base(card, gameWatcher, start, destination) { }
}
public class DisposeCardEvent : MoveCardEvent
{
    public DisposeCardEvent(ICardEntity card, IGameplayStatusWatcher gameWatcher, ICardColletionZone start, ICardColletionZone destination) :
        base(card, gameWatcher, start, destination) { }
}
public class CreateCardEvent : AddCardEvent
{
    public CreateCardEvent(ICardEntity card, IGameplayStatusWatcher gameWatcher, ICardColletionZone destination) :
        base(card, gameWatcher, destination) { }
}
public class CloneCardEvent : AddCardEvent
{    
    public CloneCardEvent(ICardEntity card, IGameplayStatusWatcher gameWatcher, ICardColletionZone destination) :
        base(card, gameWatcher, destination) { }
}
public class AppendCardStatusEvent : IGameEvent
{
    public Faction Faction;
    public CardInfo CardInfo;

    public AppendCardStatusEvent(ICardEntity card, IGameplayStatusWatcher gameWatcher)
    {
        Faction = card.Owner.Faction;
        CardInfo = new CardInfo(card, gameWatcher);
    }
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

    public EnergyEvent(IPlayerEntity player, int deltaEnergy)
    {
        Faction = player.Faction;
        Energy = player.CurrentEnergy;
        DeltaEnergy = deltaEnergy;
        MaxEnergy = player.MaxEnergy;
    }
}
public class RecoverEnergyEvent : EnergyEvent
{
    public RecoverEnergyEvent(IPlayerEntity player, GainEnergyResult gainEnergyResult) : base(player, gainEnergyResult.DeltaEp) { }
}
public class ConsumeEnergyEvent : EnergyEvent
{
    public ConsumeEnergyEvent(IPlayerEntity player, LoseEnergyResult loseEnergyResult) : base(player, loseEnergyResult.DeltaEp) { }
}
public class GainEnergyEvent : EnergyEvent
{
    public GainEnergyEvent(IPlayerEntity player, GainEnergyResult gainEnergyResult) : base(player, gainEnergyResult.DeltaEp) { }
}
public class LoseEnergyEvent : EnergyEvent
{
    public LoseEnergyEvent(IPlayerEntity player, LoseEnergyResult loseEnergyResult) : base(player, loseEnergyResult.DeltaEp) { }
}

public abstract class HealthEvent : IGameEvent
{
    public Faction Faction;
    public Guid CharacterIdentity;
    public int Hp;
    public int Dp;
    public int MaxHp;

    public HealthEvent(ICharacterEntity character)
    {
        Faction = character.Owner.Faction;
        Hp = character.CurrentHealth;
        Dp = character.CurrentArmor;
        MaxHp = character.MaxHealth;
    }
}
public abstract class DamageEvent : HealthEvent
{
    public DamageType Type;
    public int DeltaHp;
    public int DeltaShield;
    public int DamagePoint;

    public DamageEvent(ICharacterEntity character, TakeDamageResult takeDamageResult) : base(character)
    {
        Type = takeDamageResult.Type;
        DeltaHp = takeDamageResult.DeltaHp;
        DeltaShield = takeDamageResult.DeltaDp;
        DamagePoint = takeDamageResult.DamagePoint;
    }
}
public class NormalDamageEvent : DamageEvent
{
    public NormalDamageEvent(ICharacterEntity character, TakeDamageResult takeDamageResult) : base(character, takeDamageResult) { }
}
public class PenetrateDamageEvent : DamageEvent
{
    public PenetrateDamageEvent(ICharacterEntity character, TakeDamageResult takeDamageResult) : base(character, takeDamageResult) { }
}
public class AdditionalAttackEvent : DamageEvent
{
    public AdditionalAttackEvent(ICharacterEntity character, TakeDamageResult takeDamageResult) : base(character, takeDamageResult) { }
}
public class EffectiveAttackEvent : DamageEvent
{
    public EffectiveAttackEvent(ICharacterEntity character, TakeDamageResult takeDamageResult) : base(character, takeDamageResult) { }
}

public class GetHealEvent : HealthEvent
{
    public int DeltaHp;
    public int HealPoint;

    public GetHealEvent(ICharacterEntity character, GetHealResult getHealResult) : base(character)
    {
        DeltaHp = getHealResult.DeltaHp;
        HealPoint = getHealResult.HealPoint;
    }
}
public class GetShieldEvent : HealthEvent
{
    public int DeltaShield;
    public int ShieldPoint;

    public GetShieldEvent(ICharacterEntity character, GetShieldResult getShieldResult) : base(character)
    {
        DeltaShield = getShieldResult.DeltaDp;
        ShieldPoint = getShieldResult.ShieldPoint;
    }
}

public class AddBuffEvent : IGameEvent
{
    public Faction Faction;
    public PlayerBuffInfo Buff;

    public AddBuffEvent(IPlayerEntity player, PlayerBuffInfo buff)
    {
        Faction = player.Faction;
        Buff = buff;
    }
}
public class UpdateBuffEvent : IGameEvent
{
    public Faction Faction;
    public PlayerBuffInfo Buff;

    public UpdateBuffEvent(IPlayerEntity player, PlayerBuffInfo buff)
    {
        Faction = player.Faction;
        Buff = buff;
    }
}
public class RemoveBuffEvent : IGameEvent
{
    public Faction Faction;
    public PlayerBuffInfo Buff;

    public RemoveBuffEvent(IPlayerEntity player, PlayerBuffInfo buff)
    {
        Faction = player.Faction;
        Buff = buff;
    }
}
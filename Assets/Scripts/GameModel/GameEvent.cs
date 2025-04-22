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
        Faction = card.Faction(gameWatcher);
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
        Faction = card.Faction(gameWatcher);
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
public class AppendCardBuffEvent : IGameEvent
{
    public Faction Faction;
    public CardInfo CardInfo;

    public AppendCardBuffEvent(ICardEntity card, IGameplayStatusWatcher gameWatcher)
    {
        Faction = card.Faction(gameWatcher);
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
public class GainEnergyEvent : EnergyEvent
{
    public EnergyGainType GainType;

    public GainEnergyEvent(IPlayerEntity player, GetEnergyResult result) : 
        base(player, result.DeltaEp) { }
}
public class LoseEnergyEvent : EnergyEvent
{
    public EnergyLoseType LoseType;

    public LoseEnergyEvent(IPlayerEntity player, LoseEnergyResult result) : 
        base(player, result.DeltaEp) { }
}

public abstract class HealthEvent : IGameEvent
{
    public Faction Faction;
    public Guid CharacterIdentity;
    public int Hp;
    public int Dp;
    public int MaxHp;

    public HealthEvent(Faction faction, ICharacterEntity character)
    {
        Faction = faction;
        Hp = character.CurrentHealth;
        Dp = character.CurrentArmor;
        MaxHp = character.MaxHealth;
    }
}
public class DamageEvent : HealthEvent
{
    public DamageType Type;
    public DamageStyle Style;
    public int DeltaHp;
    public int DeltaShield;
    public int DamagePoint;

    public DamageEvent(
        Faction faction, 
        ICharacterEntity character, 
        TakeDamageResult takeDamageResult, 
        DamageStyle damageStyle) : base(faction, character)
    {
        Type = takeDamageResult.Type;
        Style = damageStyle;
        DeltaHp = takeDamageResult.DeltaHp;
        DeltaShield = takeDamageResult.DeltaDp;
        DamagePoint = takeDamageResult.DamagePoint;
    }
}

public class GetHealEvent : HealthEvent
{
    public int DeltaHp;
    public int HealPoint;

    public GetHealEvent(Faction faction, ICharacterEntity character, GetHealResult getHealResult) : 
        base(faction, character)
    {
        DeltaHp = getHealResult.DeltaHp;
        HealPoint = getHealResult.HealPoint;
    }
}
public class GetShieldEvent : HealthEvent
{
    public int DeltaShield;
    public int ShieldPoint;

    public GetShieldEvent(Faction faction, ICharacterEntity character, GetShieldResult getShieldResult) : 
        base(faction, character)
    {
        DeltaShield = getShieldResult.DeltaDp;
        ShieldPoint = getShieldResult.ShieldPoint;
    }
}

public class AddPlayerBuffEvent : IGameEvent
{
    public Faction Faction;
    public PlayerBuffInfo Buff;

    public AddPlayerBuffEvent(IPlayerEntity player, PlayerBuffInfo buff)
    {
        Faction = player.Faction;
        Buff = buff;
    }
}
public class UpdatePlayerBuffEvent : IGameEvent
{
    public Faction Faction;
    public PlayerBuffInfo Buff;

    public UpdatePlayerBuffEvent(IPlayerEntity player, PlayerBuffInfo buff)
    {
        Faction = player.Faction;
        Buff = buff;
    }
}
public class RemovePlayerBuffEvent : IGameEvent
{
    public Faction Faction;
    public PlayerBuffInfo Buff;

    public RemovePlayerBuffEvent(IPlayerEntity player, PlayerBuffInfo buff)
    {
        Faction = player.Faction;
        Buff = buff;
    }
}
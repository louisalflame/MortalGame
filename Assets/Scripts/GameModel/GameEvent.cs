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

public abstract class CardEvent : IGameEvent
{
    public Faction Faction;
    public CardInfo CardInfo;
    public CardCollectionInfo HandCardInfo;
    public CardCollectionInfo GraveyardInfo;
    public CardCollectionInfo ExclusionZoneInfo;
    public CardCollectionInfo DisposeZoneInfo;

    public CardEvent(ICardEntity card, GameContextManager contextMgr)
    {
        Faction = card.Owner.Faction;
        CardInfo = new CardInfo(card, contextMgr.Context);
        HandCardInfo = card.Owner.CardManager.HandCard.Cards.ToCardCollectionInfo(contextMgr.Context);
        GraveyardInfo = card.Owner.CardManager.Graveyard.Cards.ToCardCollectionInfo(contextMgr.Context);
        ExclusionZoneInfo = card.Owner.CardManager.ExclusionZone.Cards.ToCardCollectionInfo(contextMgr.Context);
        DisposeZoneInfo = card.Owner.CardManager.DisposeZone.Cards.ToCardCollectionInfo(contextMgr.Context);
    }
}
public class DiscardCardEvent : CardEvent
{
    public DiscardCardEvent(ICardEntity card, GameContextManager contextMgr) :
        base(card, contextMgr) { }
}
public class ConsumeCardEvent : CardEvent
{
    public ConsumeCardEvent(ICardEntity card, GameContextManager contextMgr) :
        base(card, contextMgr) { }
}
public class DisposeCardEvent : CardEvent
{
    public DisposeCardEvent(ICardEntity card, GameContextManager contextMgr) :
        base(card, contextMgr) { }
}
public class CreateCardEvent : CardEvent
{
    public CardCollectionType CardCollectionType;

    public CreateCardEvent(ICardEntity card, CardCollectionType cardCollectionType, GameContextManager contextMgr) :
        base(card, contextMgr) 
    {
        CardCollectionType = cardCollectionType;
    }
}
public class CloneCardEvent : CardEvent
{
    public CardCollectionType CardCollectionType;
    
    public CloneCardEvent(ICardEntity card, CardCollectionType cardCollectionType, GameContextManager contextMgr) :
        base(card, contextMgr)
    {
        CardCollectionType = cardCollectionType;
    }
}
public class AppendCardStatusEvent : IGameEvent
{
    public Faction Faction;
    public CardInfo CardInfo;

    public AppendCardStatusEvent(ICardEntity card, GameContextManager contextMgr)
    {
        Faction = card.Owner.Faction;
        CardInfo = new CardInfo(card, contextMgr.Context);
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
public class DamageEvent : HealthEvent
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
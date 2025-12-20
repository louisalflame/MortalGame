using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using Sirenix.Serialization.Utilities;
using Sirenix.Utilities;
using UnityEngine;

public interface IGameEvent
{
}

public interface IAnimationNumberEvent : IGameEvent
{
}

public record NoneEvent : IGameEvent
{
    public static readonly NoneEvent Instance = new NoneEvent();
}

public record AllySummonEvent(AllyEntity Player) : IGameEvent;
public record EnemySummonEvent(EnemyEntity Enemy) : IGameEvent;
public record RoundStartEvent(int Round, AllyEntity Player, EnemyEntity Enemy) : IGameEvent;
public record RecycleGraveyardToDeckEvent(Faction Faction, CardManagerInfo CardManagerInfo) : IGameEvent;
public record DiscardHandCardEvent(
    Faction Faction,
    IReadOnlyCollection<CardInfo> DiscardedCardInfos,
    IReadOnlyCollection<CardInfo> ExcludedCardInfos,
    CardManagerInfo CardManagerInfo) : IGameEvent;
public record RecycleGraveyardToHandCardEvent(
    Faction Faction, CardInfo RecycledCardInfo, CardManagerInfo CardManagerInfo) : IGameEvent;
public record DrawCardEvent(Faction Faction, CardInfo NewCardInfo, CardManagerInfo CardManagerInfo) : IGameEvent;

public record MoveCardEvent(
    Faction Faction,
    CardInfo CardInfo,
    CardCollectionInfo StartZoneInfo,
    CardCollectionInfo DestinationZoneInfo) : IGameEvent
{
    public MoveCardEvent(ICardEntity card, IGameplayModel gameWatcher, ICardColletionZone start, ICardColletionZone destination)
        : this(card.Faction(gameWatcher),
            card.ToInfo(gameWatcher),
            start.ToCardCollectionInfo(gameWatcher),
            destination.ToCardCollectionInfo(gameWatcher)) { }
}
public record AddCardEvent(
    Faction Faction,
    Option<CardInfo> OriginCardInfo,
    CardInfo CardInfo,
    CardCollectionInfo DestinationZoneInfo) : IGameEvent
{
    public AddCardEvent(Option<ICardEntity> originCard, ICardEntity card, IGameplayModel gameWatcher, ICardColletionZone destination)
        : this(card.Faction(gameWatcher),
            originCard.Map(c => c.ToInfo(gameWatcher)),
            card.ToInfo(gameWatcher),
            destination.ToCardCollectionInfo(gameWatcher)) { }
}
public record UpdateHandCardsEvent(Faction Faction, CardInfo CardInfo) : IGameEvent
{
    public UpdateHandCardsEvent(ICardEntity card, IGameplayModel gameWatcher) 
        : this(card.Faction(gameWatcher), card.ToInfo(gameWatcher)) { }
}
public record AddCardBuffEvent(Faction Faction, CardInfo CardInfo) : IGameEvent
{
    public AddCardBuffEvent(ICardEntity card, IGameplayModel gameWatcher)
        : this(card.Faction(gameWatcher), card.ToInfo(gameWatcher)) { }
}
public record RemoveCardBuffEvent(Faction Faction, CardInfo CardInfo) : IGameEvent
{
    public RemoveCardBuffEvent(ICardEntity card, IGameplayModel gameWatcher)
        : this(card.Faction(gameWatcher), card.ToInfo(gameWatcher)) { }
}

public record EnemySelectCardEvent(CardInfo SelectedCardInfo, IReadOnlyCollection<CardInfo> SelectedCardInfos) : IGameEvent;
public record EnemyUnselectedCardEvent(IReadOnlyCollection<CardInfo> UnselectedCardInfos) : IGameEvent;
public record PlayerExecuteStartEvent(Faction Faction, CardManagerInfo CardManagerInfo) : IGameEvent;
public record PlayerExecuteEndEvent(Faction Faction, CardManagerInfo CardManagerInfo) : IGameEvent;
public record UsedCardEvent(Faction Faction, CardInfo UsedCardInfo, CardManagerInfo CardManagerInfo) : IGameEvent;

public record GainEnergyEvent(Faction Faction, EnergyInfo Info, GainEnergyResult GainEnergyResult) : IGameEvent, IAnimationNumberEvent;
public record LoseEnergyEvent(Faction Faction, EnergyInfo Info, LoseEnergyResult LoseEnergyResult) : IGameEvent, IAnimationNumberEvent;

public record IncreaseDispositionEvent(DispositionInfo Info, int DeltaDisposition) : IGameEvent, IAnimationNumberEvent;
public record DecreaseDispositionEvent(DispositionInfo Info, int DeltaDisposition) : IGameEvent, IAnimationNumberEvent;

public abstract record HealthEvent(Faction Faction, Guid CharacterIdentity, int Hp, int Dp, int MaxHp) : IGameEvent, IAnimationNumberEvent;
public record DamageEvent(
    DamageType Type,
    DamageStyle Style,
    int DeltaHp,
    int DeltaShield,
    int DamagePoint,
    Faction Faction, 
    Guid CharacterIdentity, 
    int Hp, 
    int Dp, 
    int MaxHp) : HealthEvent(Faction, CharacterIdentity, Hp, Dp, MaxHp)
{
    public DamageEvent(
        Faction faction,
        ICharacterEntity character,
        TakeDamageResult takeDamageResult,
        DamageStyle damageStyle) : this(
            takeDamageResult.Type,
            damageStyle,
            takeDamageResult.DeltaHp,
            takeDamageResult.DeltaDp,
            takeDamageResult.DamagePoint,
            faction,
            character.Identity,
            character.CurrentHealth,
            character.CurrentArmor,
            character.MaxHealth) { }
}

public record GetHealEvent(
    int DeltaHp,
    int HealPoint,
    Faction Faction, 
    Guid CharacterIdentity, 
    int Hp, 
    int Dp, 
    int MaxHp) : HealthEvent(Faction, CharacterIdentity, Hp, Dp, MaxHp)
{
    public GetHealEvent(Faction faction, ICharacterEntity character, GetHealResult getHealResult) 
        : this(getHealResult.DeltaHp,
            getHealResult.HealPoint,
            faction,
            character.Identity,
            character.CurrentHealth,
            character.CurrentArmor,
            character.MaxHealth) { }
}
public record GetShieldEvent(
    int DeltaShield,
    int ShieldPoint,
    Faction Faction, 
    Guid CharacterIdentity, 
    int Hp, 
    int Dp, 
    int MaxHp) : HealthEvent(Faction, CharacterIdentity, Hp, Dp, MaxHp)
{
    public GetShieldEvent(Faction faction, ICharacterEntity character, GetShieldResult getShieldResult) 
        : this(getShieldResult.DeltaDp,
            getShieldResult.ShieldPoint,
            faction,
            character.Identity,
            character.CurrentHealth,
            character.CurrentArmor,
            character.MaxHealth) { }
}

public record AddPlayerBuffEvent(Faction Faction, PlayerBuffInfo Buff) : IGameEvent
{
    public AddPlayerBuffEvent(IPlayerEntity player, PlayerBuffInfo buff)
        : this(player.Faction, buff) { }
}
public record RemovePlayerBuffEvent(Faction Faction, PlayerBuffInfo Buff) : IGameEvent
{
    public RemovePlayerBuffEvent(IPlayerEntity player, PlayerBuffInfo buff)
        : this(player.Faction, buff) { }
}

public record GeneralUpdateEvent(
    IReadOnlyList<PlayerBuffInfo> PlayerBuffInfos,
    IReadOnlyList<CharacterBuffInfo> CharacterBuffInfos,
    IReadOnlyList<CardInfo> CardInfos) : IGameEvent
{
    public GeneralUpdateEvent(
        IEnumerable<PlayerBuffInfo> playerBuffInfos,
        IEnumerable<CharacterBuffInfo> characterBuffInfos,
        IEnumerable<CardInfo> cardInfos)
        : this(playerBuffInfos.ToList(),
            characterBuffInfos.ToList(),
            cardInfos.ToList()) {}
               
    public GeneralUpdateEvent(IReadOnlyCollection<PlayerBuffInfo> playerBuffInfos)
        : this(playerBuffInfos.ToList(),
            Array.Empty<CharacterBuffInfo>(),
            Array.Empty<CardInfo>()) {}
    public GeneralUpdateEvent(IReadOnlyCollection<CharacterBuffInfo> characterBuffInfos)
        : this(Array.Empty<PlayerBuffInfo>(),
            characterBuffInfos.ToList(),
            Array.Empty<CardInfo>()) {}
    public GeneralUpdateEvent(IReadOnlyCollection<CardInfo> cardInfos)
        : this(Array.Empty<PlayerBuffInfo>(),
            Array.Empty<CharacterBuffInfo>(),
            cardInfos.ToList()) {}
    public GeneralUpdateEvent(PlayerBuffInfo playerBuffInfo)
        : this(new List<PlayerBuffInfo> { playerBuffInfo },
              Array.Empty<CharacterBuffInfo>(),
              Array.Empty<CardInfo>()) {}
    public GeneralUpdateEvent(CharacterBuffInfo characterBuffInfo)
        : this(Array.Empty<PlayerBuffInfo>(),
            new List<CharacterBuffInfo> { characterBuffInfo },
            Array.Empty<CardInfo>()) {}
    public GeneralUpdateEvent(CardInfo cardInfo)
        : this(Array.Empty<PlayerBuffInfo>(),
            Array.Empty<CharacterBuffInfo>(),
            new List<CardInfo> { cardInfo }) {}
}

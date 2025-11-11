using System.Collections.Generic;
using Optional;
using UnityEngine;

public interface IEffectResult
{
}

public record TakeDamageResult(
    DamageType Type,
    int DamagePoint,
    int DeltaHp,
    int DeltaDp,
    int OverHp) : IEffectResult;
public record GetHealResult(
    int HealPoint,
    int DeltaHp,
    int OverHp) : IEffectResult;
public record GetShieldResult(
    int ShieldPoint,
    int DeltaDp,
    int OverDp) : IEffectResult;

public record GainEnergyResult(
    EnergyGainType Type,
    int EnergyPoint,
    int DeltaEp,
    int OverEp) : IEffectResult;
public record LoseEnergyResult(
    EnergyLoseType Type,
    int EnergyPoint,
    int DeltaEp,
    int OverEp) : IEffectResult;

public record IncreaseDispositionResult(
    int DispositionPoint,
    int DeltaDisposition,
    int OverDisposition) : IEffectResult;
public record DecreaseDispositionResult(
    int DispositionPoint,
    int DeltaDisposition,
    int OverDisposition) : IEffectResult;

public record AddPlayerBuffResult(
    bool IsNewBuff,
    IPlayerBuffEntity Buff,
    int DeltaLevel) : IEffectResult;
public record AddCharacterBuffResult(
    bool IsNewBuff,
    ICharacterBuffEntity Buff,
    int DeltaLevel) : IEffectResult;
public record AddCardBuffResult(
    bool IsNewBuff,
    ICardBuffEntity Buff,
    int DeltaLevel) : IEffectResult;

public record RemovePlayerBuffResult(
    IReadOnlyCollection<IPlayerBuffEntity> Buffs) : IEffectResult;
public record RemoveCharacterBuffResult(
    IReadOnlyCollection<ICharacterBuffEntity> Buff) : IEffectResult;
public record RemoveCardBuffResult(
    IReadOnlyCollection<ICardBuffEntity> Buff) : IEffectResult;

public record CreateCardResult(
    ICardEntity Card,
    ICardColletionZone Zone,
    AddCardBuffResult[] AddBuffs) : IEffectResult;
public record CloneCardResult(
    ICardEntity OriginCard,
    ICardEntity Card,
    ICardColletionZone Zone,
    AddCardBuffResult[] AddBuffs) : IEffectResult;
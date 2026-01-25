using System.Collections;
using System.Collections.Generic;
using Optional;
using UnityEngine;

public interface IEffectCommand
{ }

public record DamageEffectCommand(
    ICharacterEntity Target,
    int DamagePoint,
    DamageType DamageType) : IEffectCommand;

public record HealEffectCommand(
    ICharacterEntity Target,
    int HealPoint) : IEffectCommand;

public record ShieldEffectCommand(
    ICharacterEntity Target,
    int ShieldPoint) : IEffectCommand;

public record GainEnergyEffectCommand(
    IPlayerEntity Target,
    int EnergyPoint) : IEffectCommand;
public record LoseEnergyEffectCommand(
    IPlayerEntity Target,
    int EnergyPoint) : IEffectCommand;

public record IncreaseDispositionEffectCommand(
    AllyEntity Target,
    int DispositionPoint) : IEffectCommand;
public record DecreaseDispositionEffectCommand(
    AllyEntity Target,
    int DispositionPoint) : IEffectCommand;

public record AddPlayerBuffEffectCommand(
    IPlayerEntity Target,
    string BuffId,
    int BuffLevel) : IEffectCommand;
public record RemovePlayerBuffEffectCommand(
    IPlayerEntity Target,
    string BuffId) : IEffectCommand;

public record DrawCardEffectCommand(
    IPlayerEntity Target,
    int DrawCount) : IEffectCommand;
public record MoveCardEffectCommand(
    IPlayerEntity Target,
    ICardEntity Card,
    CardCollectionType Start,
    CardCollectionType Destination,
    MoveCardType MoveType) : IEffectCommand;

public record CreateCardEffectCommand(
    IPlayerEntity Target,
    ICardEntity NewCard,
    CardCollectionType Destination) : IEffectCommand;
public record CloneCardEffectCommand(
    IPlayerEntity Target,
    ICardEntity OriginCard,
    ICardEntity ClonedCard,
    CardCollectionType Destination) : IEffectCommand;
public record AddCardBuffEffectCommand(
    ICardEntity Target,
    string BuffId,
    int BuffLevel) : IEffectCommand;
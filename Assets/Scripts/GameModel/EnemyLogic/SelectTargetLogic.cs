using System;
using System.Linq;
using Optional.Collections;
using UnityEngine;

public record SelectTargetResult(
    bool IsValid,
    TargetType TargetType,
    Guid TargetId
);

public static class SelectTargetLogic
{
    public static SelectTargetResult SelectTarget(
        IGameplayStatusWatcher gameplayWatcher,
        ICardEntity cardEntity)
    {
        var mainSelect = cardEntity.MainSelect;
        if (mainSelect == null)
        {
            Debug.LogError($"MainSelect is null. cardId={cardEntity.CardDataId}");
            return new SelectTargetResult(false, TargetType.None, Guid.Empty);
        }

        var selectable = mainSelect.MainSelectable;
        if (selectable == null)
        {
            Debug.LogError($"MainSelectable is null. cardId={cardEntity.CardDataId}");
            return new SelectTargetResult(false, TargetType.None, Guid.Empty);
        }

        return selectable switch
        {
            NoneSelectable => new SelectTargetResult(true, TargetType.None, Guid.Empty),            
            CharacterSelectable => SelectCharacterWithLogic(gameplayWatcher, mainSelect.LogicTag),
            CharacterAllySelectable => SelectAllyCharacter(gameplayWatcher),
            CharacterEnemySelectable => SelectEnemyCharacter(gameplayWatcher),
            CardSelectable => SelectCardWithLogic(gameplayWatcher, mainSelect.LogicTag),
            CardAllySelectable => SelectAllyCard(gameplayWatcher),
            CardEnemySelectable => SelectEnemyCard(gameplayWatcher),
            _ => new SelectTargetResult(false, TargetType.None, Guid.Empty)
        };
    }

    private static SelectTargetResult SelectCharacterWithLogic(IGameplayStatusWatcher gameplayWatcher, TargetLogicTag logicTag)
    {
        return logicTag switch
        {
            TargetLogicTag.ToEnemy => SelectEnemyCharacter(gameplayWatcher),
            TargetLogicTag.ToAlly => SelectAllyCharacter(gameplayWatcher),
            TargetLogicTag.ToRandom => SelectRandomCharacter(gameplayWatcher),
            _ => new SelectTargetResult(false, TargetType.None, Guid.Empty)
        };
    }

    private static SelectTargetResult SelectCardWithLogic(IGameplayStatusWatcher gameplayWatcher, TargetLogicTag logicTag)
    {
        return logicTag switch
        {
            TargetLogicTag.ToEnemy => SelectEnemyCard(gameplayWatcher),
            TargetLogicTag.ToAlly => SelectAllyCard(gameplayWatcher),
            TargetLogicTag.ToRandom => SelectRandomCard(gameplayWatcher),
            _ => new SelectTargetResult(false, TargetType.None, Guid.Empty)
        };
    }

    private static SelectTargetResult SelectEnemyCharacter(IGameplayStatusWatcher gameplayWatcher)
    {
        return gameplayWatcher.GameStatus.OppositePlayer
            .FlatMap(oppositePlayer => LinqEnumerableExtensions.FirstOrNone(oppositePlayer.Characters))
            .Map(oppositeCharacter => new SelectTargetResult(true, TargetType.EnemyCharacter, oppositeCharacter.Identity))
            .ValueOr(new SelectTargetResult(false, TargetType.None, Guid.Empty));
    }

    private static SelectTargetResult SelectAllyCharacter(IGameplayStatusWatcher gameplayWatcher)
    {
        return gameplayWatcher.GameStatus.CurrentPlayer
            .FlatMap(currentPlayer => LinqEnumerableExtensions.FirstOrNone(currentPlayer.Characters))
            .Map(currentCharacter => new SelectTargetResult(true, TargetType.AllyCharacter, currentCharacter.Identity))
            .ValueOr(new SelectTargetResult(false, TargetType.None, Guid.Empty));
    }

    private static SelectTargetResult SelectRandomCharacter(IGameplayStatusWatcher gameplayWatcher)
    {
        return LinqEnumerableExtensions.FirstOrNone(
            gameplayWatcher.GameStatus.Ally.Characters
                .Concat(gameplayWatcher.GameStatus.Enemy.Characters))
            .FlatMap(randomCharacter => randomCharacter.Owner(gameplayWatcher)
                .Map(randomPlayer => (randomPlayer, randomCharacter)))
            .Map(tuple => new SelectTargetResult(true,
                tuple.randomPlayer.Faction == Faction.Ally ? TargetType.AllyCharacter : TargetType.EnemyCharacter,
                tuple.randomCharacter.Identity))
            .ValueOr(new SelectTargetResult(false, TargetType.None, Guid.Empty));
    }

    private static SelectTargetResult SelectEnemyCard(IGameplayStatusWatcher gameplayWatcher)
    {
        return gameplayWatcher.GameStatus.OppositePlayer
            .FlatMap(oppositePlayer => LinqEnumerableExtensions.FirstOrNone(oppositePlayer.CardManager.HandCard.Cards))
            .Map(oppositeCard => new SelectTargetResult(true, TargetType.EnemyCard, oppositeCard.Identity))
            .ValueOr(new SelectTargetResult(false, TargetType.None, Guid.Empty));
    }

    private static SelectTargetResult SelectAllyCard(IGameplayStatusWatcher gameplayWatcher)
    {
        return gameplayWatcher.GameStatus.CurrentPlayer
            .FlatMap(currentPlayer => LinqEnumerableExtensions.FirstOrNone(currentPlayer.CardManager.HandCard.Cards))
            .Map(currentCard => new SelectTargetResult(true, TargetType.AllyCard, currentCard.Identity))
            .ValueOr(new SelectTargetResult(false, TargetType.None, Guid.Empty));
    }

    private static SelectTargetResult SelectRandomCard(IGameplayStatusWatcher gameplayWatcher)
    {
        return LinqEnumerableExtensions.FirstOrNone(
            gameplayWatcher.GameStatus.Ally.CardManager.HandCard.Cards
                .Concat(gameplayWatcher.GameStatus.Enemy.CardManager.HandCard.Cards))
            .FlatMap(randomCard => randomCard.Owner(gameplayWatcher)
                .Map(randomPlayer => (randomPlayer, randomCard)))
            .Map(tuple => new SelectTargetResult(true,
                tuple.randomPlayer.Faction == Faction.Ally ? TargetType.AllyCard : TargetType.EnemyCard,
                tuple.randomCard.Identity))
            .ValueOr(new SelectTargetResult(false, TargetType.None, Guid.Empty));
    }
}

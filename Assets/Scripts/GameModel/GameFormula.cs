using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameFormula
{
    public static int NormalDamagePoint(
        IGameplayStatusWatcher gameWatcher,
        int rawDamagePoint,
        IActionUnit actionUnit)
    {
        var actionAddition = GetCardPlayAttributeAddition(
            actionUnit, gameWatcher, EffectAttributeAdditionType.NormalDamageAddition, PlayerBuffProperty.NormalDamageAddition);
        
        var actionRatio = GetCardPlayAttributeRatio(
            actionUnit, gameWatcher, EffectAttributeRatioType.NormalDamageRatio, PlayerBuffProperty.NormalDamageRatio);

        return rawDamagePoint + actionAddition;
    }

    public static int CardPower(
        IGameplayStatusWatcher gameWatcher,
        ICardEntity card,
        IActionUnit actionUnit)
    {
        var actionAddition = GetCardLookAttributeAddition(actionUnit, gameWatcher, PlayerBuffProperty.AllCardPower);

        var cardAddition = card.GetCardProperty(gameWatcher, CardProperty.PowerAddition);

        return Math.Max(0, card.OriginPower + actionAddition + cardAddition);
    }

    public static int CardCost(
        IGameplayStatusWatcher gameWatcher,
        ICardEntity card,
        IActionUnit actionUnit)
    {
        var actionAddition = GetCardLookAttributeAddition(actionUnit, gameWatcher, PlayerBuffProperty.AllCardCost);

        var cardAddition = card.GetCardProperty(gameWatcher, CardProperty.CostAddition);

        return Math.Max(0, card.OriginPower + actionAddition + cardAddition);
    }

    private static int GetCardLookAttributeAddition(
        IActionUnit actionUnit, IGameplayStatusWatcher gameWatcher, PlayerBuffProperty playerBuffProperty)
    {
        if (actionUnit is CardLookIntentAction cardLookIntent)
        {
            var playerAttribute = cardLookIntent.Card.Owner(gameWatcher.GameStatus)
                .Map(player => player.GetPlayerBuffAdditionProperty(gameWatcher, playerBuffProperty))
                .ValueOr(0);
            return playerAttribute;
        }
        return 0;
    }

    private static int GetCardPlayAttributeAddition(
        IActionUnit actionUnit,
        IGameplayStatusWatcher gameWatcher,
        EffectAttributeAdditionType attribute,
        PlayerBuffProperty playerBuffProperty)
    {
        if (actionUnit is IActionSourceUnit actionSourceUnit &&
            actionSourceUnit.Source is CardPlaySource cardPlaySource)
        {
            var cardAttribute = cardPlaySource.Attribute.IntValues
                .GetValueOrDefault(attribute, 0);
            var playerAttribute = gameWatcher.GameStatus.CurrentPlayer
                .Map(player => player.GetPlayerBuffAdditionProperty(gameWatcher, playerBuffProperty))
                .ValueOr(0);
            return cardAttribute + playerAttribute;
        }
        return 0;
    }
    private static float GetCardPlayAttributeRatio(
        IActionUnit actionUnit,
        IGameplayStatusWatcher gameWatcher,
        EffectAttributeRatioType attribute,
        PlayerBuffProperty playerBuffProperty)
    {
        if (actionUnit is IActionSourceUnit actionSourceUnit &&
            actionSourceUnit.Source is CardPlaySource cardPlaySource)
        {
            var cardAttribute = cardPlaySource.Attribute.FloatValues
                .GetValueOrDefault(attribute, 0);
            var playerAttribute = gameWatcher.GameStatus.CurrentPlayer
                .Map(player => player.GetPlayerBuffRatioProperty(gameWatcher, playerBuffProperty))
                .ValueOr(0);
            return cardAttribute + playerAttribute;
        }
        return 0f;
    }
}

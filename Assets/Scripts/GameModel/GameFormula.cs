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
        var actionAddition = GetAttributeAddition(
            actionUnit, gameWatcher, EffectAttributeAdditionType.NormalDamageAddition, PlayerBuffProperty.NormalDamageAddition);
        
        var actionRatio = GetAttributeRatio(
            actionUnit, gameWatcher, EffectAttributeRatioType.NormalDamageRatio, PlayerBuffProperty.NormalDamageRatio);

        return rawDamagePoint + actionAddition;
    }

    public static int PenetrateDamagePoint(
        IGameplayStatusWatcher gameWatcher,
        int rawDamagePoint,
        IActionUnit actionUnit)
    {
        var actionAddition = GetAttributeAddition(
            actionUnit, gameWatcher, EffectAttributeAdditionType.PenetrateDamageAddition, PlayerBuffProperty.PenetrateDamageAddition);
        
        var actionRatio = GetAttributeRatio(
            actionUnit, gameWatcher, EffectAttributeRatioType.PenetrateDamageRatio, PlayerBuffProperty.PenetrateDamageRatio);

        return rawDamagePoint + actionAddition;
    }

    public static int AdditionalDamagePoint(
        IGameplayStatusWatcher gameWatcher,
        int rawDamagePoint,
        IActionUnit actionUnit)
    {
        var actionAddition = GetAttributeAddition(
            actionUnit, gameWatcher, EffectAttributeAdditionType.AdditionalDamageAddition, PlayerBuffProperty.AdditionalDamageAddition);
        
        var actionRatio = GetAttributeRatio(
            actionUnit, gameWatcher, EffectAttributeRatioType.AdditionalDamageRatio, PlayerBuffProperty.AdditionalDamageRatio);

        return rawDamagePoint + actionAddition;
    }
    
    public static int EffectiveDamagePoint(
        IGameplayStatusWatcher gameWatcher,
        int rawDamagePoint,
        IActionUnit actionUnit)
    {
        var actionAddition = GetAttributeAddition(
            actionUnit, gameWatcher, EffectAttributeAdditionType.EffectiveDamageAddition, PlayerBuffProperty.EffectiveDamageAddition);
        
        var actionRatio = GetAttributeRatio(
            actionUnit, gameWatcher, EffectAttributeRatioType.EffectiveDamageRatio, PlayerBuffProperty.EffectiveDamageRatio);

        return rawDamagePoint + actionAddition;
    }

    public static int HealPoint(
        IGameplayStatusWatcher gameWatcher,
        int rawHealPoint,
        IActionUnit actionUnit)
    {
        var actionAddition = GetAttributeAddition(
            actionUnit, gameWatcher, EffectAttributeAdditionType.HealAddition, PlayerBuffProperty.HealAddition);

        var actionRatio = GetAttributeRatio(
            actionUnit, gameWatcher, EffectAttributeRatioType.HealRatio, PlayerBuffProperty.HealRatio);

        return rawHealPoint + actionAddition;
    }

    public static int CardPower(
        IGameplayStatusWatcher gameWatcher,
        ICardEntity card,
        IActionUnit actionUnit)
    {
        var actionAddition = GetAttributeAddition(
            actionUnit, gameWatcher, EffectAttributeAdditionType.PowerAddition, PlayerBuffProperty.AllCardPower);

        var cardAddition = card.GetCardProperty(gameWatcher, CardProperty.PowerAddition);

        return Math.Max(0, card.OriginPower + actionAddition + cardAddition);
    }

    public static int CardCost(
        IGameplayStatusWatcher gameWatcher,
        ICardEntity card,
        IActionUnit actionUnit)
    {
        var actionAddition = GetAttributeAddition(
            actionUnit, gameWatcher, EffectAttributeAdditionType.CostAddition, PlayerBuffProperty.AllCardCost);

        var cardAddition = card.GetCardProperty(gameWatcher, CardProperty.CostAddition);

        return Math.Max(0, card.OriginCost + actionAddition + cardAddition);
    }

    private static int GetAttributeAddition(
        IActionUnit actionUnit,
        IGameplayStatusWatcher gameWatcher,
        EffectAttributeAdditionType attribute,
        PlayerBuffProperty playerBuffProperty)
    {
        if (actionUnit is CardLookIntentAction cardLookIntent)
        {
            var playerAttribute = cardLookIntent.Card.Owner(gameWatcher.GameStatus)
                .Map(player => player.GetPlayerBuffAdditionProperty(gameWatcher, playerBuffProperty))
                .ValueOr(0);
            return playerAttribute;
        }
        if (actionUnit.Source is CardPlaySource cardPlaySource)
        {
            var cardPlayAttribute = cardPlaySource.Attribute.IntValues
                .GetValueOrDefault(attribute, 0);
            var playerAttribute = gameWatcher.GameStatus.CurrentPlayer
                .Map(player => player.GetPlayerBuffAdditionProperty(gameWatcher, playerBuffProperty))
                .ValueOr(0);

            return cardPlayAttribute + playerAttribute;
        }
        return 0;
    }
    private static float GetAttributeRatio(
        IActionUnit actionUnit,
        IGameplayStatusWatcher gameWatcher,
        EffectAttributeRatioType attribute,
        PlayerBuffProperty playerBuffProperty)
    {
        if (actionUnit.Source is CardPlaySource cardPlaySource)
        {
            var cardPlayAttribute = cardPlaySource.Attribute.FloatValues
                .GetValueOrDefault(attribute, 0);
            var playerAttribute = gameWatcher.GameStatus.CurrentPlayer
                .Map(player => player.GetPlayerBuffRatioProperty(gameWatcher, playerBuffProperty))
                .ValueOr(0);

            return cardPlayAttribute + playerAttribute;
        }
        return 0f;
    }
}

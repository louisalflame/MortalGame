using UnityEngine;

public static class GameFormula
{
    public static int NormalDamagePoint(
        IGameplayStatusWatcher gameplayWatcher,
        int rawDamagePoint,
        IActionUnit actionUnit)
    {
        switch (actionUnit)
        {
            case IActionSourceUnit actionSourceUnit:
                switch (actionSourceUnit.Source)
                {
                    case CardPlaySource cardPlay:
                        var cpNorDamAdd = cardPlay.Attribute.NormalDamageAddition;

                        var pbuffNorDamAdd = gameplayWatcher.GameStatus.CurrentPlayer
                            .Map(player => player.GetPlayerBuffProperty(gameplayWatcher, EffectAttributeType.NormalDamageAddition))
                            .ValueOr(0);

                        return rawDamagePoint + cpNorDamAdd + pbuffNorDamAdd;
                }
                break;
        }

        return rawDamagePoint;
    }

    public static int CardPower(
        IGameplayStatusWatcher gameplayWatcher,
        ICardEntity card,
        IActionUnit actionUnit)
    {
        var basePower = card.EvalPower(gameplayWatcher);

        switch (actionUnit)
        {
            case IActionSourceUnit actionSourceUnit:
                switch (actionSourceUnit.Source)
                {
                    case CardPlaySource cardPlay:
                        var cpPowerAdd = cardPlay.Attribute.PowerAddition;
                        var cpPowerRatio = cardPlay.Attribute.PowerRatio;

                        var pbuffPowerAdd = gameplayWatcher.GameStatus.CurrentPlayer
                            .Map(player => player.GetPlayerBuffProperty(gameplayWatcher, EffectAttributeType.PowerAddition))
                            .ValueOr(0);

                        var pbuffPowerRatio = gameplayWatcher.GameStatus.CurrentPlayer
                            .Map(player => player.GetPlayerBuffProperty(gameplayWatcher, EffectAttributeType.PowerRatio))
                            .ValueOr(0);

                        return (int)((card.EvalPower(gameplayWatcher) + cpPowerAdd + pbuffPowerAdd) * (1 + cpPowerRatio + pbuffPowerRatio));
                }
                break;
        }

        return basePower;
    }

    public static int CardCost(
        IGameplayStatusWatcher gameplayWatcher,
        ICardEntity card,
        IActionUnit actionUnit)
    {
        var baseCost = card.EvalCost(gameplayWatcher);

        return baseCost;
    }
}

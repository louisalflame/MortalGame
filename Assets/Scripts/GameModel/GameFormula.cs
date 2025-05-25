using UnityEngine;

public static class GameFormula
{
    public static int NormalDamagePoint(
        IGameplayStatusWatcher gameplayWatcher,
        int rawDamagePoint,
        IActionSource actionSource,
        IActionTarget actionTarget)
    {
        // PowerAdjust is applied in RawDamagePoint calculation,

        switch (actionSource)
        {
            case CardPlaySource cardPlay:
                var cpNorDamAdd = cardPlay.Attribute.NormalDamageAddition;

                var pbuffNorDamAdd = gameplayWatcher.GameStatus.CurrentPlayer
                    .Map(player => player.GetPlayerBuffProperty(gameplayWatcher, EffectAttributeType.NormalDamageAddition))
                    .ValueOr(0);

                return rawDamagePoint + cpNorDamAdd + pbuffNorDamAdd;
            default:
                return rawDamagePoint;
        }
    }
}

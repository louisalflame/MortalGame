using System;
using System.Collections.Generic;
using System.Linq;
using Optional.Collections;
using Sirenix.OdinInspector;

public interface IIntegerValue
{
    int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource, IActionUnit actionUnit);
}

[Serializable]
public class ConstInteger : IIntegerValue
{
    public int Value;

    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource, IActionUnit actionUnit)
    {
        return Value;
    }
}

[Serializable]
public class CardIntegerProperty : IIntegerValue
{   
    public enum CardIntegerValueType
    {
        Power,
        Cost,
    }

    [HorizontalGroup("1")]
    public ITargetCardValue Card;
    public CardIntegerValueType Property;

    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource, IActionUnit actionUnit)
    {
        return Card
            .Eval(gameWatcher, triggerSource, actionUnit)
            .Map(
                card => Property switch
                {
                    // TODO: Apply EffectAttribute.Power adjust
                    CardIntegerValueType.Power => GameFormula.CardPower(gameWatcher, card, new CardLookIntentAction(card)),
                    // TODO: Apply EffectAttribute.Cost adjust
                    CardIntegerValueType.Cost => GameFormula.CardCost(gameWatcher, card, new CardLookIntentAction(card)),
                    _ => 0
                })
            .ValueOr(0);
    }
}

[Serializable]
public class PlayerIntegerProperty : IIntegerValue
{
    public enum PlayerIntegerValueType
    {
        MaxEnergy,
        CurrentEnergy,
    }

    [HorizontalGroup("1")]
    public ITargetPlayerValue Player;
    public PlayerIntegerValueType Property;

    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource, IActionUnit actionUnit)
    {
        var playerOpt = Player.Eval(gameWatcher, triggerSource, actionUnit);
        return Player
            .Eval(gameWatcher, triggerSource, actionUnit)
            .Map(
                player => Property switch
                {
                    PlayerIntegerValueType.MaxEnergy => player.MaxEnergy,
                    PlayerIntegerValueType.CurrentEnergy => player.CurrentEnergy,
                    _ => 0
                })
            .ValueOr(0);
    }
}

[Serializable]
public class CardBuffIntegerProperty : IIntegerValue
{
    public enum CardBuffIntegerValueType
    {
        Level
    }

    [HorizontalGroup("1")]
    public ITargetCardBuffValue CardBuff;
    public CardBuffIntegerValueType Property;

    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource, IActionUnit actionUnit)
    {
        var cardBuffOpt = CardBuff.Eval(gameWatcher, triggerSource, actionUnit);
        return cardBuffOpt
            .Map(
                cardBuff => Property switch
                {
                    CardBuffIntegerValueType.Level => cardBuff.Level,
                    _ => 0
                })
            .ValueOr(0);
    }
}

[Serializable]
public class PlayerBuffIntegerProperty : IIntegerValue
{
    public enum PlayerBuffIntegerValueType
    {
        Level,
    }

    [HorizontalGroup("1")]
    public ITargetPlayerBuffValue PlayerBuff;
    public PlayerBuffIntegerValueType Property;

    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource, IActionUnit actionUnit)
    {
        return PlayerBuff
            .Eval(gameWatcher, triggerSource, actionUnit)
            .Map(
                playerBuff => Property switch
                {
                    PlayerBuffIntegerValueType.Level => playerBuff.Level,
                    _ => 0
                })
            .ValueOr(0);
    }
}

[Serializable]
public class PlayerBuffSessionInteger : IIntegerValue
{
    [HorizontalGroup("1")]
    public ITargetPlayerBuffValue PlayerBuff;
    public string SessionIntegerId;

    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource, IActionUnit actionUnit)
    {
        return PlayerBuff
            .Eval(gameWatcher, triggerSource, actionUnit)
            .FlatMap(playerBuff => playerBuff.GetSessionInteger(SessionIntegerId))
            .ValueOr(0);
    }
}

[Serializable]
public class ConditionalValue : IIntegerValue
{
    [Serializable]
    public class ConditionPair
    {
        [ShowInInspector]
        [HorizontalGroup("1")]
        public List<IPlayerBuffCondition> Conditions = new ();

        [HorizontalGroup("2")]
        public IIntegerValue Value;
    }

    [ShowInInspector]
    [HorizontalGroup("1")]
    public List<ConditionPair> Pairs = new ();

    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource, IActionUnit actionUnit)
    {
        foreach (var pair in Pairs)
        {
            if (pair.Conditions.All(condition => condition.Eval(gameWatcher, triggerSource, actionUnit)))
            {
                return pair.Value.Eval(gameWatcher, triggerSource, actionUnit);
            }
        }
        return 0;
    }
}
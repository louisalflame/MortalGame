using System;
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
                    CardIntegerValueType.Power => card.EvalPower(gameWatcher),
                    CardIntegerValueType.Cost => card.EvalCost(gameWatcher),
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
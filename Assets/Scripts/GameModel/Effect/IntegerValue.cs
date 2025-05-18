using System;
using System.Linq;
using Optional.Collections;
using Sirenix.OdinInspector;

public interface IIntegerValue
{
    int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource);
}

[Serializable]
public class ConstInteger : IIntegerValue
{
    public int Value;

    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource)
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

    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource)
    {
        return Card
            .Eval(gameWatcher, triggerSource)
            .Map(
                card => Property switch
                {
                    CardIntegerValueType.Power => card.Power,
                    CardIntegerValueType.Cost => card.Cost,
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

    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource)
    {
        var playerOpt = Player.Eval(gameWatcher, triggerSource);
        return Player
            .Eval(gameWatcher, triggerSource)
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

    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource)
    {
        var cardBuffOpt = CardBuff.Eval(gameWatcher, triggerSource);
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

    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource)
    {
        return PlayerBuff
            .Eval(gameWatcher, triggerSource)
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

    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource)
    {
        return PlayerBuff
            .Eval(gameWatcher, triggerSource)
            .FlatMap(playerBuff => playerBuff.GetSessionInteger(SessionIntegerId))
            .ValueOr(0);
    }
}
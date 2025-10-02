using Optional;
using UnityEngine;

public interface IActionUnit
{
    GameTiming Timing { get; }
    IActionSource Source { get; }
}

public interface IActionTargetUnit : IActionUnit
{
    IActionTarget Target { get; }
}

public class UpdateTimingAction : IActionUnit
{
    public GameTiming Timing { get; }
    public IActionSource Source { get; }

    public UpdateTimingAction(GameTiming timing, IActionSource source)
    {
        Timing = timing;
        Source = source;
    }
}

// EffectAction
public interface IEffectAction : IActionUnit
{
    EffectType EffectType { get; }
}

public interface IEffectTargetAction : IEffectAction, IActionTargetUnit
{
}

public interface IEffectResultAction : IEffectAction, IActionTargetUnit
{
}

// LookAction
public class CardLookIntentAction : IActionUnit
{
    public GameTiming Timing => GameTiming.None;
    public IActionSource Source => SystemSource.Instance;
    public ICardEntity Card { get; private set; }

    public CardLookIntentAction(ICardEntity card)
    {
        Card = card;
    }
}
public class CardBuffPropertyLookAction : IActionUnit
{
    public GameTiming Timing => GameTiming.None;
    public IActionSource Source => SystemSource.Instance;
    public ICardBuffPropertyEntity Property { get; private set; }

    public CardBuffPropertyLookAction(ICardBuffPropertyEntity property)
    {
        Property = property;
    }
}
public class PlayerBuffPropertyLookAction : IActionUnit
{
    public GameTiming Timing => GameTiming.None;
    public IActionSource Source => SystemSource.Instance;
    public IPlayerBuffPropertyEntity Property { get; private set; }

    public PlayerBuffPropertyLookAction(IPlayerBuffPropertyEntity property)
    {
        Property = property;
    }
}

//===

public class CardPlayIntentAction : IActionUnit
{
    public GameTiming Timing => GameTiming.PlayCardStart;
    public IActionSource Source => CardPlaySource;
    public CardPlaySource CardPlaySource { get; private set; }

    public CardPlayIntentAction(CardPlaySource cardPlaySource)
    {
        CardPlaySource = cardPlaySource;
    }
}
public class CardPlayResultAction : IActionUnit
{
    public GameTiming Timing => GameTiming.PlayCardEnd;
    public IActionSource Source => CardPlaySource;
    public CardPlaySource CardPlaySource { get; private set; }

    public CardPlayResultAction(CardPlaySource cardPlaySource)
    {
        CardPlaySource = cardPlaySource;
    }
}

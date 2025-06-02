using Optional;
using UnityEngine;

public interface IActionUnit
{
}

public interface IActionSourceUnit : IActionUnit
{
    IActionSource Source { get; }
}
public interface IActionTargetUnit : IActionUnit
{
    IActionTarget Target { get; }
}

public class SystemAction : IActionUnit
{
    public static readonly SystemAction Instance = new SystemAction();
}
public class UpdateTimingAction : IActionUnit
{
    public UpdateTiming Timing { get; }

    public UpdateTimingAction(UpdateTiming timing)
    {
        Timing = timing;
    }
}
public class TriggerTimingAction : IActionUnit, IActionSourceUnit
{
    public TriggerTiming Timing { get; }
    public IActionSource Source { get; }

    public TriggerTimingAction(TriggerTiming timing, IActionSource source)
    {
        Timing = timing;
        Source = source;
    }
}

public interface IIntentAction : IActionSourceUnit
{
    UpdateAction ActionType { get; }
}

public interface IIntentTargetAction : IIntentAction, IActionTargetUnit
{
}

public interface IResultTargetAction : IActionSourceUnit, IActionTargetUnit
{
    UpdateAction ActionType { get; }
}
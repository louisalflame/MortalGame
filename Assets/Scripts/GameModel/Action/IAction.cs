using Optional;
using UnityEngine;

public interface IActionUnit
{
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
public class TriggerTimingAction : IActionUnit
{
    public TriggerTiming Timing { get; }
    public IActionSource Source { get; }

    public TriggerTimingAction(TriggerTiming timing, IActionSource source)
    {
        Timing = timing;
        Source = source;
    }
}

public interface IIntentAction : IActionUnit
{
    UpdateAction ActionType { get; }
    IActionSource Source { get; }
}

public interface IIntentTargetAction : IIntentAction
{
    IActionTarget Target { get; }
}

public interface IResultTargetAction : IActionUnit
{
    UpdateAction ActionType { get; }
    IActionSource Source { get; }
    IActionTarget Target { get; }
}
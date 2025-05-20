using Optional;
using UnityEngine;

public interface IActionUnit
{
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

    public TriggerTimingAction(TriggerTiming timing)
    {
        Timing = timing;
    }
}

public interface IActionIntent : IActionUnit
{
    UpdateAction ActionType { get; }
    IActionSource Source { get; }
}

public interface ITargetActionIntent : IActionUnit
{
    UpdateAction ActionType { get; }
    IActionSource Source { get; }
    IActionTarget Target { get; }
}

public interface ITargetActionResult : IActionUnit
{
    UpdateAction ActionType { get; }
    IActionSource Source { get; }
    IActionTarget Target { get; }
}
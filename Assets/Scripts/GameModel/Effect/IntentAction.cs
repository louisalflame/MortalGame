using Optional;
using UnityEngine;

public abstract class BaseIntentAction : IIntentAction
{
    public IActionSource Source { get; private set; }
    public IActionTarget Target { get; private set; }

    protected BaseIntentAction(IActionSource source, IActionTarget target)
    {
        Source = source;
        Target = target;
    }
}

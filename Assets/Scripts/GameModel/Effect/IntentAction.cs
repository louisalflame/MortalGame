using Optional;
using UnityEngine;

public abstract class BaseIntentAction : IIntentAction
{
    public abstract UpdateAction ActionType { get; }
    public IActionSource Source { get; private set; }
    public IActionTarget Target { get; private set; }

    protected BaseIntentAction(IActionSource source, IActionTarget target)
    {
        Source = source;
        Target = target;
    }
}

public class PlayCardIntentAction : BaseIntentAction
{
    public override UpdateAction ActionType => UpdateAction.PlayCard;
    public ICardEntity Card { get; private set; }

    public PlayCardIntentAction(IActionSource source, IActionTarget target, ICardEntity card) : base(source, target)
    {
        Card = card;
    }
}
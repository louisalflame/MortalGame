using Optional;
using UnityEngine;

public abstract class BaseIntentTargetAction : IIntentTargetAction
{
    public abstract UpdateAction ActionType { get; }
    public IActionSource Source { get; private set; }
    public IActionTarget Target { get; private set; }

    protected BaseIntentTargetAction(IActionSource source, IActionTarget target)
    {
        Source = source;
        Target = target;
    }
}

public class DamageIntentTargetAction : BaseIntentTargetAction
{
    public override UpdateAction ActionType => UpdateAction.Damage;
    public DamageType Type { get; private set; }

    public DamageIntentTargetAction(IActionSource source, IActionTarget target, DamageType type) : base(source, target)
    {
        Type = type;
    }
}

public class HealIntentTargetAction : BaseIntentTargetAction
{
    public override UpdateAction ActionType => UpdateAction.Heal;

    public HealIntentTargetAction(IActionSource source, IActionTarget target) : base(source, target)
    { }
}

public class ShieldIntentTargetAction : BaseIntentTargetAction
{
    public override UpdateAction ActionType => UpdateAction.Shield;

    public ShieldIntentTargetAction(IActionSource source, IActionTarget target) : base(source, target)
    { }
}

public class GainEnergyIntentTargetAction : BaseIntentTargetAction
{
    public override UpdateAction ActionType => UpdateAction.GainEnergy;

    public GainEnergyIntentTargetAction(IActionSource source, IActionTarget target) : base(source, target)
    { }
}
public class LoseEnergyIntentTargetAction : BaseIntentTargetAction
{
    public override UpdateAction ActionType => UpdateAction.LoseEnergy;

    public LoseEnergyIntentTargetAction(IActionSource source, IActionTarget target) : base(source, target)
    { }
}

public class CardPlayIntentTargetAction : BaseIntentTargetAction
{
    public override UpdateAction ActionType => UpdateAction.PlayCard;
    public CardPlaySource CardPlay => Source as CardPlaySource;

    public CardPlayIntentTargetAction(CardPlaySource source, IActionTarget target) : base(source, target)
    { }
}

public class RecycleDeckIntentTargetAction : BaseIntentTargetAction
{
    public override UpdateAction ActionType => UpdateAction.RecycleDeck;
    public RecycleDeckIntentTargetAction(IActionTarget target) : base(new SystemSource(), target)
    { }
}

public class DrawCardIntentTargetAction : BaseIntentTargetAction
{
    public override UpdateAction ActionType => UpdateAction.DrawCard;

    public DrawCardIntentTargetAction(IActionSource source, IActionTarget target) : base(source, target)
    { }
}

public class DiscardCardIntentTargetAction : BaseIntentTargetAction
{
    public override UpdateAction ActionType => UpdateAction.DiscardCard;

    public DiscardCardIntentTargetAction(IActionSource source, IActionTarget target) : base(source, target)
    { }
}
public class ConsumeCardIntentTargetAction : BaseIntentTargetAction
{
    public override UpdateAction ActionType => UpdateAction.ConsumeCard;

    public ConsumeCardIntentTargetAction(IActionSource source, IActionTarget target) : base(source, target)
    { }
}
public class DisposeCardIntentTargetAction : BaseIntentTargetAction
{
    public override UpdateAction ActionType => UpdateAction.DisposeCard;

    public DisposeCardIntentTargetAction(IActionSource source, IActionTarget target) : base(source, target)
    { }
}
public class CreateCardIntentTargetAction : BaseIntentTargetAction
{
    public override UpdateAction ActionType => UpdateAction.CreateCard;

    public CreateCardIntentTargetAction(IActionSource source, IActionTarget target) : base(source, target)
    { }
}
public class CloneCardIntentTargetAction : BaseIntentTargetAction
{
    public override UpdateAction ActionType => UpdateAction.CloneCard;

    public CloneCardIntentTargetAction(IActionSource source, IActionTarget target) : base(source, target)
    { }
}

public class AddPlayerBuffIntentTargetAction : BaseIntentTargetAction
{
    public override UpdateAction ActionType => UpdateAction.AddPlayerBuff;

    public AddPlayerBuffIntentTargetAction(IActionSource source, IActionTarget target) : base(source, target)
    { }
}
public class RemovePlayerBuffIntentTargetAction : BaseIntentTargetAction
{
    public override UpdateAction ActionType => UpdateAction.RemovePlayerBuff;

    public RemovePlayerBuffIntentTargetAction(IActionSource source, IActionTarget target) : base(source, target)
    { }
}

public class AddCardBuffIntentTargetAction : BaseIntentTargetAction
{
    public override UpdateAction ActionType => UpdateAction.AddCardBuff;

    public AddCardBuffIntentTargetAction(IActionSource source, IActionTarget target) : base(source, target)
    { }
}
public class RemoveCardBuffIntentTargetAction : BaseIntentTargetAction
{
    public override UpdateAction ActionType => UpdateAction.RemoveCardBuff;

    public RemoveCardBuffIntentTargetAction(IActionSource source, IActionTarget target) : base(source, target)
    { }
}
using Optional;
using UnityEngine;

public abstract class BaseIntentTargetAction : IEffectTargetAction
{
    public GameTiming Timing => GameTiming.EffectTargetIntent;
    public abstract EffectType EffectType { get; }
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
    public override EffectType EffectType => EffectType.Damage;
    public DamageType Type { get; private set; }

    public DamageIntentTargetAction(IActionSource source, IActionTarget target, DamageType type) : base(source, target)
    {
        Type = type;
    }
}

public class HealIntentTargetAction : BaseIntentTargetAction
{
    public override EffectType EffectType => EffectType.Heal;

    public HealIntentTargetAction(IActionSource source, IActionTarget target) : base(source, target)
    { }
}

public class ShieldIntentTargetAction : BaseIntentTargetAction
{
    public override EffectType EffectType => EffectType.Shield;

    public ShieldIntentTargetAction(IActionSource source, IActionTarget target) : base(source, target)
    { }
}

public class GainEnergyIntentTargetAction : BaseIntentTargetAction
{
    public override EffectType EffectType => EffectType.GainEnergy;

    public GainEnergyIntentTargetAction(IActionSource source, IActionTarget target) : base(source, target)
    { }
}
public class LoseEnergyIntentTargetAction : BaseIntentTargetAction
{
    public override EffectType EffectType => EffectType.LoseEnergy;

    public LoseEnergyIntentTargetAction(IActionSource source, IActionTarget target) : base(source, target)
    { }
}

public class RecycleDeckIntentTargetAction : BaseIntentTargetAction
{
    public override EffectType EffectType => EffectType.RecycleDeck;
    public RecycleDeckIntentTargetAction(IActionTarget target) : base(SystemSource.Instance, target)
    { }
}

public class DrawCardIntentTargetAction : BaseIntentTargetAction
{
    public override EffectType EffectType => EffectType.DrawCard;

    public DrawCardIntentTargetAction(IActionSource source, IActionTarget target) : base(source, target)
    { }
}

public class DiscardCardIntentTargetAction : BaseIntentTargetAction
{
    public override EffectType EffectType => EffectType.DiscardCard;

    public DiscardCardIntentTargetAction(IActionSource source, IActionTarget target) : base(source, target)
    { }
}
public class ConsumeCardIntentTargetAction : BaseIntentTargetAction
{
    public override EffectType EffectType => EffectType.ConsumeCard;

    public ConsumeCardIntentTargetAction(IActionSource source, IActionTarget target) : base(source, target)
    { }
}
public class DisposeCardIntentTargetAction : BaseIntentTargetAction
{
    public override EffectType EffectType => EffectType.DisposeCard;

    public DisposeCardIntentTargetAction(IActionSource source, IActionTarget target) : base(source, target)
    { }
}
public class CreateCardIntentTargetAction : BaseIntentTargetAction
{
    public override EffectType EffectType => EffectType.CreateCard;

    public CreateCardIntentTargetAction(IActionSource source, IActionTarget target) : base(source, target)
    { }
}
public class CloneCardIntentTargetAction : BaseIntentTargetAction
{
    public override EffectType EffectType => EffectType.CloneCard;

    public CloneCardIntentTargetAction(IActionSource source, IActionTarget target) : base(source, target)
    { }
}

public class AddPlayerBuffIntentTargetAction : BaseIntentTargetAction
{
    public override EffectType EffectType => EffectType.AddPlayerBuff;

    public AddPlayerBuffIntentTargetAction(IActionSource source, IActionTarget target) : base(source, target)
    { }
}
public class RemovePlayerBuffIntentTargetAction : BaseIntentTargetAction
{
    public override EffectType EffectType => EffectType.RemovePlayerBuff;

    public RemovePlayerBuffIntentTargetAction(IActionSource source, IActionTarget target) : base(source, target)
    { }
}

public class AddCardBuffIntentTargetAction : BaseIntentTargetAction
{
    public override EffectType EffectType => EffectType.AddCardBuff;

    public AddCardBuffIntentTargetAction(IActionSource source, IActionTarget target) : base(source, target)
    { }
}
public class RemoveCardBuffIntentTargetAction : BaseIntentTargetAction
{
    public override EffectType EffectType => EffectType.RemoveCardBuff;

    public RemoveCardBuffIntentTargetAction(IActionSource source, IActionTarget target) : base(source, target)
    { }
}
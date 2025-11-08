using UnityEngine;

public abstract class BaseEffectIntentAction : IEffectAction
{
    public GameTiming Timing => GameTiming.EffectIntent;
    public abstract EffectType EffectType { get; }
    public IActionSource Source { get; private set; }

    protected BaseEffectIntentAction(IActionSource source)
    {
        Source = source;
    }
}

public class DamageIntentAction : BaseEffectIntentAction
{
    public override EffectType EffectType => EffectType.Damage;
    public DamageType Type { get; private set; }

    public DamageIntentAction(IActionSource source, DamageType type) : base(source)
    {
        Type = type;
    }
}

public class HealIntentAction : BaseEffectIntentAction
{
    public override EffectType EffectType => EffectType.Heal;

    public HealIntentAction(IActionSource source) : base(source)
    { }
}

public class ShieldIntentAction : BaseEffectIntentAction
{
    public override EffectType EffectType => EffectType.Shield;

    public ShieldIntentAction(IActionSource source) : base(source)
    { }
}

public class GainEnergyIntentAction : BaseEffectIntentAction
{
    public override EffectType EffectType => EffectType.GainEnergy;

    public GainEnergyIntentAction(IActionSource source) : base(source)
    { }
}
public class LoseEnergyIntentAction : BaseEffectIntentAction
{
    public override EffectType EffectType => EffectType.LoseEnergy;

    public LoseEnergyIntentAction(IActionSource source) : base(source)
    { }
}

public class IncreaseDispositionIntentAction : BaseEffectIntentAction
{
    public override EffectType EffectType => EffectType.AdjustDisposition;

    public IncreaseDispositionIntentAction(IActionSource source) : base(source)
    { }
}
public class DecreaseDispositionIntentAction : BaseEffectIntentAction
{
    public override EffectType EffectType => EffectType.AdjustDisposition;

    public DecreaseDispositionIntentAction(IActionSource source) : base(source)
    { }
}

public class RecycleDeckIntentAction : BaseEffectIntentAction
{
    public override EffectType EffectType => EffectType.RecycleDeck;
    public RecycleDeckIntentAction() : base(SystemSource.Instance)
    { }
}

public class DrawCardIntentAction : BaseEffectIntentAction
{
    public override EffectType EffectType => EffectType.DrawCard;

    public DrawCardIntentAction(IActionSource source) : base(source)
    { }
}

public class DiscardCardIntentAction : BaseEffectIntentAction
{
    public override EffectType EffectType => EffectType.DiscardCard;

    public DiscardCardIntentAction(IActionSource source) : base(source)
    { }
}
public class ConsumeCardIntentAction : BaseEffectIntentAction
{
    public override EffectType EffectType => EffectType.ConsumeCard;

    public ConsumeCardIntentAction(IActionSource source) : base(source)
    { }
}
public class DisposeCardIntentAction : BaseEffectIntentAction
{
    public override EffectType EffectType => EffectType.DisposeCard;

    public DisposeCardIntentAction(IActionSource source) : base(source)
    { }
}
public class CreateCardIntentAction : BaseEffectIntentAction
{
    public override EffectType EffectType => EffectType.CreateCard;

    public CreateCardIntentAction(IActionSource source) : base(source)
    { }
}
public class CloneCardIntentAction : BaseEffectIntentAction
{
    public override EffectType EffectType => EffectType.CloneCard;

    public CloneCardIntentAction(IActionSource source) : base(source)
    { }
}

public class AddPlayerBuffIntentAction : BaseEffectIntentAction
{
    public override EffectType EffectType => EffectType.AddPlayerBuff;

    public AddPlayerBuffIntentAction(IActionSource source) : base(source)
    { }
}
public class RemovePlayerBuffIntentAction : BaseEffectIntentAction
{
    public override EffectType EffectType => EffectType.RemovePlayerBuff;

    public RemovePlayerBuffIntentAction(IActionSource source) : base(source)
    { }
}

public class AddCardBuffIntentAction : BaseEffectIntentAction
{
    public override EffectType EffectType => EffectType.AddCardBuff;

    public AddCardBuffIntentAction(IActionSource source) : base(source)
    { }
}
public class RemoveCardBuffIntentAction : BaseEffectIntentAction
{
    public override EffectType EffectType => EffectType.RemoveCardBuff;

    public RemoveCardBuffIntentAction(IActionSource source) : base(source)
    { }
}

public class CardPlayEffectAttributeIntentAction : BaseEffectIntentAction
{
    public override EffectType EffectType => EffectType.CardPlayEffectAttribute;

    public CardPlayEffectAttributeIntentAction(IActionSource source) : base(source)
    { }
}
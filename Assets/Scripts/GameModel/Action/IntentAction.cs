using UnityEngine;

public abstract class BaseIntentAction : IIntentAction
{
    public abstract UpdateAction ActionType { get; }
    public IActionSource Source { get; private set; }

    protected BaseIntentAction(IActionSource source)
    {
        Source = source;
    }
}

public class CardLookIntentAction : BaseIntentAction
{
    public override UpdateAction ActionType => UpdateAction.None;
    public ICardEntity Card { get; private set; }

    public CardLookIntentAction(ICardEntity card) : base(SystemSource.Instance)
    { 
        Card = card;
    }
}
public class CardPlayIntentAction : BaseIntentAction
{
    public override UpdateAction ActionType => UpdateAction.CardPlay;
    public CardPlaySource CardPlay => Source as CardPlaySource;

    public CardPlayIntentAction(CardPlaySource source) : base(source)
    { }
}

public class DamageIntentAction : BaseIntentAction
{
    public override UpdateAction ActionType => UpdateAction.Damage;
    public DamageType Type { get; private set; }

    public DamageIntentAction(IActionSource source, DamageType type) : base(source)
    {
        Type = type;
    }
}

public class HealIntentAction : BaseIntentAction
{
    public override UpdateAction ActionType => UpdateAction.Heal;

    public HealIntentAction(IActionSource source) : base(source)
    { }
}

public class ShieldIntentAction : BaseIntentAction
{
    public override UpdateAction ActionType => UpdateAction.Shield;

    public ShieldIntentAction(IActionSource source) : base(source)
    { }
}

public class GainEnergyIntentAction : BaseIntentAction
{
    public override UpdateAction ActionType => UpdateAction.GainEnergy;

    public GainEnergyIntentAction(IActionSource source) : base(source)
    { }
}
public class LoseEnergyIntentAction : BaseIntentAction
{
    public override UpdateAction ActionType => UpdateAction.LoseEnergy;

    public LoseEnergyIntentAction(IActionSource source) : base(source)
    { }
}

public class RecycleDeckIntentAction : BaseIntentAction
{
    public override UpdateAction ActionType => UpdateAction.RecycleDeck;
    public RecycleDeckIntentAction() : base(SystemSource.Instance)
    { }
}

public class DrawCardIntentAction : BaseIntentAction
{
    public override UpdateAction ActionType => UpdateAction.DrawCard;

    public DrawCardIntentAction(IActionSource source) : base(source)
    { }
}

public class DiscardCardIntentAction : BaseIntentAction
{
    public override UpdateAction ActionType => UpdateAction.DiscardCard;

    public DiscardCardIntentAction(IActionSource source) : base(source)
    { }
}
public class ConsumeCardIntentAction : BaseIntentAction
{
    public override UpdateAction ActionType => UpdateAction.ConsumeCard;

    public ConsumeCardIntentAction(IActionSource source) : base(source)
    { }
}
public class DisposeCardIntentAction : BaseIntentAction
{
    public override UpdateAction ActionType => UpdateAction.DisposeCard;

    public DisposeCardIntentAction(IActionSource source) : base(source)
    { }
}
public class CreateCardIntentAction : BaseIntentAction
{
    public override UpdateAction ActionType => UpdateAction.CreateCard;

    public CreateCardIntentAction(IActionSource source) : base(source)
    { }
}
public class CloneCardIntentAction : BaseIntentAction
{
    public override UpdateAction ActionType => UpdateAction.CreateCard;

    public CloneCardIntentAction(IActionSource source) : base(source)
    { }
}

public class AddPlayerBuffIntentAction : BaseIntentAction
{
    public override UpdateAction ActionType => UpdateAction.AddPlayerBuff;

    public AddPlayerBuffIntentAction(IActionSource source) : base(source)
    { }
}
public class RemovePlayerBuffIntentAction : BaseIntentAction
{
    public override UpdateAction ActionType => UpdateAction.RemovePlayerBuff;

    public RemovePlayerBuffIntentAction(IActionSource source) : base(source)
    { }
}

public class AddCardBuffIntentAction : BaseIntentAction
{
    public override UpdateAction ActionType => UpdateAction.AddCardBuff;

    public AddCardBuffIntentAction(IActionSource source) : base(source)
    { }
}
public class RemoveCardBuffIntentAction : BaseIntentAction
{
    public override UpdateAction ActionType => UpdateAction.RemoveCardBuff;

    public RemoveCardBuffIntentAction(IActionSource source) : base(source)
    { }
}

public class CardPlayEffectAttributeIntentAction : BaseIntentAction
{
    public override UpdateAction ActionType => UpdateAction.CardPlayEffectAttribute;

    public CardPlayEffectAttributeIntentAction(IActionSource source) : base(source)
    { }
}
using Optional;
using UnityEngine;

public abstract class BaseResultAction : IResultAction
{
    public IActionSource Source { get; private set; }
    public IActionTarget Target { get; private set; }

    protected BaseResultAction(IActionSource source, IActionTarget target)
    {
        Source = source;
        Target = target;
    }
}

public class SummonAction : BaseResultAction
{
    public SummonAction(IActionSource source, IActionTarget target) : base(source, target)
    {
    }
}

public class DeathAction : BaseResultAction
{
    public DeathAction(IActionSource source, IActionTarget target) : base(source, target)
    {
    }
}

public class DamageAction : BaseResultAction
{
    public TakeDamageResult DamageResult { get; private set; }
    public DamageStyle Style { get; private set; }
    public DamageType Type => DamageResult.Type;

    public DamageAction(
        IActionSource source, 
        IActionTarget target, 
        TakeDamageResult damageResult,
        DamageStyle style) : base(source, target)
    {
        DamageResult = damageResult;
        Style = style;
    }
}

public class GetHealAction : BaseResultAction
{
    public GetHealResult HealResult { get; private set; }

    public GetHealAction(
        IActionSource source, 
        IActionTarget target,
        GetHealResult healResult) : base(source, target)
    {
        HealResult = healResult;
    }
}
public class GetShieldAction : BaseResultAction
{
    public GetShieldResult ShieldResult { get; private set; }

    public GetShieldAction(
        IActionSource source,
        IActionTarget target,
        GetShieldResult shieldResult) : base(source, target)
    {
        ShieldResult = shieldResult;
    }
}

public class GainEnergyAction : BaseResultAction
{    
    public GetEnergyResult EnergyResult { get; private set; }

    public GainEnergyAction(
        IActionSource source, 
        IActionTarget target,
        GetEnergyResult energyResult) : base(source, target)
    {
        EnergyResult = energyResult;
    }
}
public class LoseEnergyAction : BaseResultAction
{
    public LoseEnergyResult EnergyResult { get; private set; }

    public LoseEnergyAction(
        IActionSource source, 
        IActionTarget target,
        LoseEnergyResult energyResult) : base(source, target)
    {
        EnergyResult = energyResult;
    }
}

public class RecycleDeckAction : BaseResultAction
{
    public RecycleDeckAction(IActionTarget target) : base(new SystemSource(), target)
    {
    }
}

public class DrawCardAction : BaseResultAction
{
    public ICardEntity Card { get; private set; }

    public DrawCardAction(IActionSource source, IActionTarget target, ICardEntity card) :
        base(source, target)
    {
        Card = card;
    }
}

public class DiscardCardAction : BaseResultAction
{
    public ICardEntity Card { get; private set; }

    public DiscardCardAction(IActionSource source, IActionTarget target, ICardEntity card) :
        base(source, target)
    {
        Card = card;
    }
}
public class ConsumeCardAction : BaseResultAction
{
    public ICardEntity Card { get; private set; }

    public ConsumeCardAction(IActionSource source, IActionTarget target, ICardEntity card) :
        base(source, target)
    {
        Card = card;
    }
}
public class DisposeCardAction : BaseResultAction
{
    public ICardEntity Card { get; private set; }

    public DisposeCardAction(IActionSource source, IActionTarget target, ICardEntity card) :
        base(source, target)
    {
        Card = card;
    }
}

public class AddPlayerBuffAction : BaseResultAction
{
    public IPlayerBuffEntity Buff { get; private set; }

    public AddPlayerBuffAction(IActionSource source, IActionTarget target, IPlayerBuffEntity buff) :
        base(source, target)
    {
        Buff = buff;
    }
}
public class StackPlayerBuffAction : BaseResultAction
{
    public IPlayerBuffEntity Buff { get; private set; }

    public StackPlayerBuffAction(IActionSource source, IActionTarget target, IPlayerBuffEntity buff) :
        base(source, target)
    {
        Buff = buff;
    }
}
public class RemovePlayerBuffAction : BaseResultAction
{
    public IPlayerBuffEntity Buff { get; private set; }

    public RemovePlayerBuffAction(IActionSource source, IActionTarget target, IPlayerBuffEntity buff) :
        base(source, target)
    {
        Buff = buff;
    }
}
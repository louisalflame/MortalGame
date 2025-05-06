using Optional;
using UnityEngine;

public abstract class BaseResultAction : IResultAction
{
    public abstract UpdateAction ActionType { get; }
    public IActionSource Source { get; private set; }
    public IActionTarget Target { get; private set; }

    protected BaseResultAction(IActionSource source, IActionTarget target)
    {
        Source = source;
        Target = target;
    }
}

public class SummonResultAction : BaseResultAction
{
    public override UpdateAction ActionType => UpdateAction.Summon;
    public SummonResultAction(IActionSource source, IActionTarget target) : base(source, target)
    {
    }
}

public class DeathResultAction : BaseResultAction
{
    public override UpdateAction ActionType => UpdateAction.Death;
    public DeathResultAction(IActionSource source, IActionTarget target) : base(source, target)
    {
    }
}

public class DamageResultAction : BaseResultAction
{
    public override UpdateAction ActionType => UpdateAction.Damage;
    public TakeDamageResult DamageResult { get; private set; }
    public DamageStyle Style { get; private set; }
    public DamageType Type => DamageResult.Type;

    public DamageResultAction(
        IActionSource source, 
        IActionTarget target, 
        TakeDamageResult damageResult,
        DamageStyle style) : base(source, target)
    {
        DamageResult = damageResult;
        Style = style;
    }
}

public class HealResultAction : BaseResultAction
{
    public GetHealResult HealResult { get; private set; }
    public override UpdateAction ActionType => UpdateAction.Heal;

    public HealResultAction(
        IActionSource source, 
        IActionTarget target,
        GetHealResult healResult) : base(source, target)
    {
        HealResult = healResult;
    }
}
public class ShieldResultAction : BaseResultAction
{
    public override UpdateAction ActionType => UpdateAction.Shield;
    public GetShieldResult ShieldResult { get; private set; }

    public ShieldResultAction(
        IActionSource source,
        IActionTarget target,
        GetShieldResult shieldResult) : base(source, target)
    {
        ShieldResult = shieldResult;
    }
}

public class GainEnergyResultAction : BaseResultAction
{    
    public override UpdateAction ActionType => UpdateAction.GainEnergy;
    public GetEnergyResult EnergyResult { get; private set; }

    public GainEnergyResultAction(
        IActionSource source, 
        IActionTarget target,
        GetEnergyResult energyResult) : base(source, target)
    {
        EnergyResult = energyResult;
    }
}
public class LoseEnergyResultAction : BaseResultAction
{
    public override UpdateAction ActionType => UpdateAction.LoseEnergy;
    public LoseEnergyResult EnergyResult { get; private set; }

    public LoseEnergyResultAction(
        IActionSource source, 
        IActionTarget target,
        LoseEnergyResult energyResult) : base(source, target)
    {
        EnergyResult = energyResult;
    }
}

public class PlayCardResultAction : BaseResultAction
{
    public override UpdateAction ActionType => UpdateAction.PlayCard;
    public ICardEntity Card { get; private set; }

    public PlayCardResultAction(IActionSource source, IActionTarget target, ICardEntity card) :
        base(source, target)
    {
        Card = card;
    }
}

public class RecycleDeckResultAction : BaseResultAction
{
    public override UpdateAction ActionType => UpdateAction.RecycleDeck;
    public RecycleDeckResultAction(IActionTarget target) : base(new SystemSource(), target)
    {
    }
}

public class DrawCardResultAction : BaseResultAction
{
    public override UpdateAction ActionType => UpdateAction.DrawCard;
    public ICardEntity Card { get; private set; }

    public DrawCardResultAction(IActionSource source, IActionTarget target, ICardEntity card) :
        base(source, target)
    {
        Card = card;
    }
}

public class DiscardCardResultAction : BaseResultAction
{
    public override UpdateAction ActionType => UpdateAction.DiscardCard;
    public ICardEntity Card { get; private set; }

    public DiscardCardResultAction(IActionSource source, IActionTarget target, ICardEntity card) :
        base(source, target)
    {
        Card = card;
    }
}
public class ConsumeCardResultAction : BaseResultAction
{
    public override UpdateAction ActionType => UpdateAction.ConsumeCard;
    public ICardEntity Card { get; private set; }

    public ConsumeCardResultAction(IActionSource source, IActionTarget target, ICardEntity card) :
        base(source, target)
    {
        Card = card;
    }
}
public class DisposeCardResultAction : BaseResultAction
{
    public override UpdateAction ActionType => UpdateAction.DisposeCard;
    public ICardEntity Card { get; private set; }

    public DisposeCardResultAction(IActionSource source, IActionTarget target, ICardEntity card) :
        base(source, target)
    {
        Card = card;
    }
}

public class AddPlayerBuffResultAction : BaseResultAction
{
    public override UpdateAction ActionType => UpdateAction.AddPlayerBuff;
    public IPlayerBuffEntity Buff { get; private set; }

    public AddPlayerBuffResultAction(IActionSource source, IActionTarget target, IPlayerBuffEntity buff) :
        base(source, target)
    {
        Buff = buff;
    }
}
public class StackPlayerBuffResultAction : BaseResultAction
{
    public override UpdateAction ActionType => UpdateAction.StackPlayerBuff;
    public IPlayerBuffEntity Buff { get; private set; }

    public StackPlayerBuffResultAction(IActionSource source, IActionTarget target, IPlayerBuffEntity buff) :
        base(source, target)
    {
        Buff = buff;
    }
}
public class RemovePlayerBuffResultAction : BaseResultAction
{
    public override UpdateAction ActionType => UpdateAction.RemovePlayerBuff;
    public IPlayerBuffEntity Buff { get; private set; }

    public RemovePlayerBuffResultAction(IActionSource source, IActionTarget target, IPlayerBuffEntity buff) :
        base(source, target)
    {
        Buff = buff;
    }
}
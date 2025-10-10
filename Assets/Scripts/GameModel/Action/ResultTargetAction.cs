using Optional;
using UnityEngine;

public abstract class BaseResultAction : IEffectResultAction
{
    public GameTiming Timing => GameTiming.EffectTargetResult;
    public abstract EffectType EffectType { get; }
    public IActionSource Source { get; private set; }
    public IActionTarget Target { get; private set; }

    protected BaseResultAction(IActionSource source, IActionTarget target)
    {
        Source = source;
        Target = target;
    }
}

public class DamageResultAction : BaseResultAction
{
    public override EffectType EffectType => EffectType.Damage;
    public TakeDamageResult DamageResult { get; private set; }
    public DamageStyle Style { get; private set; }

    public DamageResultAction(
        IActionSource source,
        IActionTarget target,
        TakeDamageResult damageResult) : base(source, target)
    {
        DamageResult = damageResult;
    }
}

public class HealResultAction : BaseResultAction
{
    public GetHealResult HealResult { get; private set; }
    public override EffectType EffectType => EffectType.Heal;

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
    public override EffectType EffectType => EffectType.Shield;
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
    public override EffectType EffectType => EffectType.GainEnergy;
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
    public override EffectType EffectType => EffectType.LoseEnergy;
    public LoseEnergyResult EnergyResult { get; private set; }

    public LoseEnergyResultAction(
        IActionSource source, 
        IActionTarget target,
        LoseEnergyResult energyResult) : base(source, target)
    {
        EnergyResult = energyResult;
    }
}

public class RecycleDeckResultAction : BaseResultAction
{
    public override EffectType EffectType => EffectType.RecycleDeck;
    public RecycleDeckResultAction(IActionTarget target) : base(SystemSource.Instance, target)
    {
    }
}

public class DrawCardResultAction : BaseResultAction
{
    public override EffectType EffectType => EffectType.DrawCard;
    public ICardEntity Card { get; private set; }

    public DrawCardResultAction(IActionSource source, IActionTarget target, ICardEntity card) :
        base(source, target)
    {
        Card = card;
    }
}

public class DiscardCardResultAction : BaseResultAction
{
    public override EffectType EffectType => EffectType.DiscardCard;
    public ICardEntity Card { get; private set; }

    public DiscardCardResultAction(IActionSource source, IActionTarget target, ICardEntity card) :
        base(source, target)
    {
        Card = card;
    }
}
public class ConsumeCardResultAction : BaseResultAction
{
    public override EffectType EffectType => EffectType.ConsumeCard;
    public ICardEntity Card { get; private set; }

    public ConsumeCardResultAction(IActionSource source, IActionTarget target, ICardEntity card) :
        base(source, target)
    {
        Card = card;
    }
}
public class DisposeCardResultAction : BaseResultAction
{
    public override EffectType EffectType => EffectType.DisposeCard;
    public ICardEntity Card { get; private set; }

    public DisposeCardResultAction(IActionSource source, IActionTarget target, ICardEntity card) :
        base(source, target)
    {
        Card = card;
    }
}
public class CreateCardResultAction : BaseResultAction
{
    public override EffectType EffectType => EffectType.CreateCard;
    public CreateCardResult CreateResult { get; private set; }

    public CreateCardResultAction(IActionSource source, IActionTarget target, CreateCardResult createResult) :
        base(source, target)
    {
        CreateResult = createResult;
    }
}
public class CloneCardResultAction : BaseResultAction
{
    public override EffectType EffectType => EffectType.CloneCard;
    public CloneCardResult CloneResult { get; private set; }

    public CloneCardResultAction(IActionSource source, IActionTarget target, CloneCardResult cloneResult) :
        base(source, target)
    {
        CloneResult = cloneResult;
    }
}

public class AddPlayerBuffResultAction : BaseResultAction
{
    public override EffectType EffectType => EffectType.AddPlayerBuff;
    public AddPlayerBuffResult AddResult { get; private set; }

    public AddPlayerBuffResultAction(IActionSource source, IActionTarget target, AddPlayerBuffResult addResult) :
        base(source, target)
    {
        AddResult = addResult;
    }
}
public class RemovePlayerBuffResultAction : BaseResultAction
{
    public override EffectType EffectType => EffectType.RemovePlayerBuff;
    public RemovePlayerBuffResult RemoveResult { get; private set; }

    public RemovePlayerBuffResultAction(IActionSource source, IActionTarget target, RemovePlayerBuffResult removeResult) :
        base(source, target)
    {
        RemoveResult = removeResult;
    }
}

public class AddCardBuffResultAction : BaseResultAction
{
    public override EffectType EffectType => EffectType.AddCardBuff;
    public AddCardBuffResult AddResult { get; private set; }

    public AddCardBuffResultAction(IActionSource source, IActionTarget target, AddCardBuffResult addResult) :
        base(source, target)
    {
        AddResult = addResult;
    }
}
public class RemoveCardBuffResultAction : BaseResultAction
{
    public override EffectType EffectType => EffectType.RemoveCardBuff;
    public RemoveCardBuffResult RemoveResult { get; private set; }

    public RemoveCardBuffResultAction(IActionSource source, IActionTarget target, RemoveCardBuffResult removeResult) :
        base(source, target)
    {
        RemoveResult = removeResult;
    }
}
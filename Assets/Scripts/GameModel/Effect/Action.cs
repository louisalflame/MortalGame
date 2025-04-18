using Optional;
using UnityEngine;

public interface IAction
{
    
}

public abstract class BaseAction : IAction
{
    public IActionSource Source { get; private set; }
    public IActionTarget Target { get; private set; }

    protected BaseAction(IActionSource source, IActionTarget target)
    {
        Source = source;
        Target = target;
    }
}

public class UpdateTimingAction : BaseAction
{
    public UpdateTiming Timing { get; private set; }

    public UpdateTimingAction(UpdateTiming timing) : 
        base(new SystemSource(), new SystemTarget())
    {
        Timing = timing;
    }
}

public class SummonAction : BaseAction
{
    public SummonAction() : base(null, null)
    {
    }
}

public class DeathAction : BaseAction
{
    public DeathAction() : base(null, null)
    {
    }
}

public class NormalDamageAction : BaseAction
{
    public int Damage { get; private set; }

    public NormalDamageAction(IActionSource source, IActionTarget target, int damage) :
        base(source, target)
    {
        Damage = damage;
    }
}
public class PenetrateDamageAction : BaseAction
{
    public int Damage { get; private set; }

    public PenetrateDamageAction(IActionSource source, IActionTarget target, int damage) :
        base(source, target)
    {
        Damage = damage;
    }
}
public class AdditionalDamageAction : BaseAction
{
    public int Damage { get; private set; }

    public AdditionalDamageAction(IActionSource source, IActionTarget target, int damage) :
        base(source, target)
    {
        Damage = damage;
    }
}
public class EffectiveDamageAction : BaseAction
{
    public int Damage { get; private set; }

    public EffectiveDamageAction(IActionSource source, IActionTarget target, int damage) :
        base(source, target)
    {
        Damage = damage;
    }
}

public class GetHealAction : BaseAction
{
    public int Heal { get; private set; }

    public GetHealAction(IActionSource source, IActionTarget target, int heal) :
        base(source, target)
    {
        Heal = heal;
    }
}
public class GetShieldAction : BaseAction
{
    public int Shield { get; private set; }

    public GetShieldAction(IActionSource source, IActionTarget target, int shield) :
        base(source, target)
    {
        Shield = shield;
    }
}

public class GainEnergyAction : BaseAction
{
    public int EnergyChange { get; private set; }

    public GainEnergyAction(IActionSource source, IActionTarget target, int energyChange) :
        base(source, target)
    {
        EnergyChange = energyChange;
    }
}
public class LoseEnergyAction : BaseAction
{
    public int EnergyChange { get; private set; }

    public LoseEnergyAction(IActionSource source, IActionTarget target, int energyChange) :
        base(source, target)
    {
        EnergyChange = energyChange;
    }
}

public class RecycleDeckAction : BaseAction
{
    public RecycleDeckAction(IActionTarget target) : base(new SystemSource(), target)
    {
    }
}

public class DrawCardAction : BaseAction
{
    public ICardEntity Card { get; private set; }

    public DrawCardAction(IActionSource source, IActionTarget target, ICardEntity card) :
        base(source, target)
    {
        Card = card;
    }
}

public class DiscardCardAction : BaseAction
{
    public ICardEntity Card { get; private set; }

    public DiscardCardAction(IActionSource source, IActionTarget target, ICardEntity card) :
        base(source, target)
    {
        Card = card;
    }
}
public class ConsumeCardAction : BaseAction
{
    public ICardEntity Card { get; private set; }

    public ConsumeCardAction(IActionSource source, IActionTarget target, ICardEntity card) :
        base(source, target)
    {
        Card = card;
    }
}
public class DisposeCardAction : BaseAction
{
    public ICardEntity Card { get; private set; }

    public DisposeCardAction(IActionSource source, IActionTarget target, ICardEntity card) :
        base(source, target)
    {
        Card = card;
    }
}

public class AddPlayerBuffAction : BaseAction
{
    public IPlayerBuffEntity Buff { get; private set; }

    public AddPlayerBuffAction(IActionSource source, IActionTarget target, IPlayerBuffEntity buff) :
        base(source, target)
    {
        Buff = buff;
    }
}
public class StackPlayerBuffAction : BaseAction
{
    public IPlayerBuffEntity Buff { get; private set; }

    public StackPlayerBuffAction(IActionSource source, IActionTarget target, IPlayerBuffEntity buff) :
        base(source, target)
    {
        Buff = buff;
    }
}
public class RemovePlayerBuffAction : BaseAction
{
    public IPlayerBuffEntity Buff { get; private set; }

    public RemovePlayerBuffAction(IActionSource source, IActionTarget target, IPlayerBuffEntity buff) :
        base(source, target)
    {
        Buff = buff;
    }
}
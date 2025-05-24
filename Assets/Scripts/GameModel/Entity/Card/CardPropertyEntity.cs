using Rayark.Mast;
using UnityEngine;

public interface ICardPropertyEntity
{
    CardProperty Property { get; }
    
    int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource);
}

public class PreservedPropertyEntity : ICardPropertyEntity
{
    public CardProperty Property => CardProperty.Preserved;

    public PreservedPropertyEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource) => 0;
}

public class InitialPriorityPropertyEntity : ICardPropertyEntity
{
    public CardProperty Property => CardProperty.InitialPriority;

    public InitialPriorityPropertyEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource) => 0;
}   

public class ConsumablePropertyEntity : ICardPropertyEntity
{
    public CardProperty Property => CardProperty.Consumable;

    public ConsumablePropertyEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource) => 0;
}

public class DisposePropertyEntity : ICardPropertyEntity
{
    public CardProperty Property => CardProperty.Dispose;
    public DisposePropertyEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource) => 0;
}

public class AutoDisposePropertyEntity : ICardPropertyEntity
{
    public CardProperty Property => CardProperty.AutoDispose;

    public AutoDisposePropertyEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource) => 0;
}

public class SealedPropertyEntity : ICardPropertyEntity
{
    public CardProperty Property => CardProperty.Sealed;

    public SealedPropertyEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource) => 0;
}
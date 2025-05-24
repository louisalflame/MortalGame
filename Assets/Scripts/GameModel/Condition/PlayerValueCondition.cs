using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IPlayerValueCondition
{
    bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit, IPlayerEntity player);
}

[Serializable]
public class PlayerEqualCondition : IPlayerValueCondition
{
    [HorizontalGroup("1")]
    public ITargetPlayerValue ComparePlayer;

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit, IPlayerEntity player)
    {
        return ComparePlayer
            .Eval(gameWatcher, source, actionUnit)
            .Match(
                comparePlayer => player == comparePlayer,
                () => false);
    }
}

[Serializable]
public class PlayerEnergyCondition : IPlayerValueCondition
{
    [ShowInInspector]
    [HorizontalGroup("1")]
    public List<IPlayerEnergyCondition> Conditions = new ();

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit, IPlayerEntity player)
    {
        return Conditions.All(c => c.Eval(gameWatcher, source, actionUnit, player.EnergyManager));
    }
}

public interface IPlayerEnergyCondition
{
    bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit, IEnergyManager energyManager);
}

[Serializable]
public class EnergyIntegerCondition : IPlayerEnergyCondition
{
    public enum EnergyValueType
    {
        Current,
        Max
    }

    public EnergyValueType ValueType;

    [ShowInInspector]
    [HorizontalGroup("1")]
    public List<IIntegerValueCondition> Conditions = new ();

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit, IEnergyManager energyManager)
    {
        return ValueType switch
        {
            EnergyValueType.Current => Conditions.All(c => c.Eval(gameWatcher, source, actionUnit, energyManager.Energy)),
            EnergyValueType.Max     => Conditions.All(c => c.Eval(gameWatcher, source, actionUnit, energyManager.MaxEnergy)),
            _                       => false
        };
    }
}
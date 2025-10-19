using System;
using Optional;
using Sirenix.OdinInspector;
using UnityEngine;

public interface ICharacterValueCondition
{
    bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit, ICharacterEntity character);
}

[Serializable]
public class CharacterFactionCondition : ICharacterValueCondition
{
    public enum FactionCondition
    {
        Same,
        Opposite
    }

    [HorizontalGroup("1")]
    public ITargetPlayerValue ComparePlayer;

    public FactionCondition Faction;

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit, ICharacterEntity character)
    {
        return ComparePlayer.Eval(gameWatcher, source, actionUnit)
            .Combine(character.Owner(gameWatcher))
            .Match(
                tuple => Faction switch
                {
                    FactionCondition.Same     => tuple.Item1.Faction == tuple.Item2.Faction,
                    FactionCondition.Opposite => tuple.Item2.Faction != tuple.Item1.Faction,
                    _                         => false
                },
                () => false);
    }
}
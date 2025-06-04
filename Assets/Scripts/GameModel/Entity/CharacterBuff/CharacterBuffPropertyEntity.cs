using Rayark.Mast;
using UnityEngine;

public interface ICharacterBuffPropertyEntity
{
    CharacterBuffProperty Property { get; }
    
    int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource);
}

public class MaxHealthPropertyCharacterBuffEntity : ICharacterBuffPropertyEntity
{
    public CharacterBuffProperty Property => CharacterBuffProperty.MaxHealth;

    public MaxHealthPropertyCharacterBuffEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource) => 0;
}

public class MaxEnergyPropertyCharacterBuffEntity : ICharacterBuffPropertyEntity
{
    public CharacterBuffProperty Property => CharacterBuffProperty.MaxEnergy;

    public MaxEnergyPropertyCharacterBuffEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource) => 0;
}
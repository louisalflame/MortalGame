using Optional;
using UnityEngine;

public interface ICharacterValueCondition
{
    bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, ICharacterEntity character);
}

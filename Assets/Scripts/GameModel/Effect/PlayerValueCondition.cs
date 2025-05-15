using Optional;
using UnityEngine;

public interface IPlayerValueCondition
{
    bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, Option<IPlayerEntity> player);
}
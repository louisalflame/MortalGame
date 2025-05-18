using Optional;
using UnityEngine;

public interface IPlayerBuffValueCondition
{
    bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IPlayerBuffEntity playerBuff);
}
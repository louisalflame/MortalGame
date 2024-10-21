using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacterView : BaseCharacterView
{
    public void Init(IGameplayStatusWatcher statusWatcher) 
    {
        _statusWatcher = statusWatcher;
        _timmer = 0;
        _healthEventBuffer = new Queue<HealthEvent>();
    }

    public void SummonEnemy(EnemySummonEvent enemySummonEvent)
    {
        Debug.Log($"Summon Enemy: {enemySummonEvent.Enemy.Name}");

        _Run().Forget();
    }
}

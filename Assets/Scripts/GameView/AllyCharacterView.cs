using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

public class AllyCharacterView : BaseCharacterView
{
    public void Init(IGameplayStatusWatcher statusWatcher) 
    {
        _statusWatcher = statusWatcher;
        _timmer = 0;
        _healthEventBuffer = new Queue<HealthEvent>();
    }

    public void SummonAlly(AllySummonEvent allySummonEvent)
    {
        Debug.Log($"Summon Ally: {allySummonEvent.Player.Name}");

        _Run().Forget();
    }    
}

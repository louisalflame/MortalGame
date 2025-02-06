using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacterView : BaseCharacterView, ISelectableView
{
    [SerializeField]
    private RectTransform _rectTransform;

    public RectTransform RectTransform => _rectTransform;
    public TargetType TargetType => TargetType.Enemy;
    public Guid TargetIdentity => _playerIdentity;

    private Guid _playerIdentity;

    public void Init(IGameplayStatusWatcher statusWatcher) 
    {
        _statusWatcher = statusWatcher;
        _timmer = 0;
        _healthEventBuffer = new Queue<HealthEvent>();
    }

    public void SummonEnemy(EnemySummonEvent enemySummonEvent)
    {
        Debug.Log($"Summon Enemy: {enemySummonEvent.Enemy.NameKey}");
        _playerIdentity = enemySummonEvent.Enemy.Identity;

        _Run().Forget();
    }

    public void OnSelect()
    {
    }
    public void OnDeselect()
    {
    }
}

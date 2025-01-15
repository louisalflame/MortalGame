using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

public class AllyCharacterView : BaseCharacterView, ISelectableView
{
    [SerializeField]
    private RectTransform _rectTransform;

    public RectTransform RectTransform => _rectTransform;
    public TargetType TargetType => TargetType.Ally;

    public void Init(IGameplayStatusWatcher statusWatcher) 
    {
        _statusWatcher = statusWatcher;
        _timmer = 0;
        _healthEventBuffer = new Queue<HealthEvent>();
    }

    public void SummonAlly(AllySummonEvent allySummonEvent)
    {
        Debug.Log($"Summon Ally: {allySummonEvent.Player.NameKey}");

        _Run().Forget();
    }
    
    public void OnSelect()
    {
    }

    public void OnDeselect()
    {
    }
}

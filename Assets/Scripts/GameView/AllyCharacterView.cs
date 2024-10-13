using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

public class AllyCharacterView : MonoBehaviour
{
    [SerializeField]
    private DamageEventViewFactory _damageEventViewFactory;
    [SerializeField]
    private HealEventViewFactory _healEventViewFactory;
    [SerializeField]
    private ShieldEventViewFactory _shieldEventViewFactory;
    [SerializeField]
    private Transform _eventViewParent;

    [SerializeField]
    private float _minTimeInterval = 0.5f;

    private float _timmer = 0;
    private Queue<HealthEvent> _healthEventBuffer;
    private IGameplayStatusWatcher _statusWatcher;

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
    public void UpdateHealth(HealthEvent healthEvent)
    {
        _healthEventBuffer.Enqueue(healthEvent);
    }

    private async UniTaskVoid _Run()
    {
        while (true)
        {
            _timmer += Time.deltaTime;
            if (_timmer >= _minTimeInterval)
            {
                _timmer -= _minTimeInterval;
                if (_healthEventBuffer.TryDequeue(out var healthEvent))
                {
                    _PlayHealthEventAnimation(healthEvent).Forget();
                }
            }

            await UniTask.NextFrame();            
        }
    }

    private async UniTaskVoid _PlayHealthEventAnimation(HealthEvent healthEvent)
    {
        switch (healthEvent)
        {
            case DamageEvent takeDamageEvent:
                var damageEventView = _damageEventViewFactory.CreatePrefab();
                damageEventView.SetEventInfo(takeDamageEvent, _eventViewParent);

                await damageEventView.PlayAnimation();
                _damageEventViewFactory.RecyclePrefab(damageEventView);
                break;

            case GetHealEvent getHealEvent:
                var healEventView = _healEventViewFactory.CreatePrefab();
                healEventView.SetEventInfo(getHealEvent, _eventViewParent);
                
                await healEventView.PlayAnimation();
                _healEventViewFactory.RecyclePrefab(healEventView);
                break;

            case GetShieldEvent getShieldEvent:
                var shieldEventView = _shieldEventViewFactory.CreatePrefab();
                shieldEventView.SetEventInfo(getShieldEvent, _eventViewParent);
                
                await shieldEventView.PlayAnimation();
                _shieldEventViewFactory.RecyclePrefab(shieldEventView);
                break;
        }        
    }
}

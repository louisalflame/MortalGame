using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

public abstract class BaseCharacterView : MonoBehaviour
{
    [SerializeField]
    protected DamageEventViewFactory _damageEventViewFactory;
    [SerializeField]
    protected HealEventViewFactory _healEventViewFactory;
    [SerializeField]
    protected ShieldEventViewFactory _shieldEventViewFactory;
    [SerializeField]
    protected Transform _eventViewParent;

    [SerializeField]
    protected float _minTimeInterval;

    protected float _timmer = 0;
    protected Queue<HealthEvent> _healthEventBuffer;
    protected IGameplayStatusWatcher _statusWatcher;

    public void UpdateHealth(HealthEvent healthEvent)
    {
        _healthEventBuffer.Enqueue(healthEvent);
    }

    protected async UniTaskVoid _Run()
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

    protected async UniTaskVoid _PlayHealthEventAnimation(HealthEvent healthEvent)
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

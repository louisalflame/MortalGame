using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
    private float _minTimeInterval = 0.5f;

    private float _nextIntervalTime = 0;

    public void UpdateHealth(HealthEvent healthEvent)
    {
        switch (healthEvent)
        {
            case DamageEvent takeDamageEvent:
                var damageEventView = _damageEventViewFactory.CreatePrefab();
                damageEventView.SetEventInfo(takeDamageEvent, transform);

                UniTask.Delay((int)(_nextIntervalTime * 1000))
                    .ContinueWith(() => damageEventView.PlayAnimation())
                    .ContinueWith(() => _damageEventViewFactory.RecyclePrefab(damageEventView))
                    .Forget();
                break;

            case GetHealEvent getHealEvent:
                var healEventView = _healEventViewFactory.CreatePrefab();
                healEventView.SetEventInfo(getHealEvent, transform);
                
                UniTask.Delay((int)(_nextIntervalTime * 1000))
                    .ContinueWith(() => healEventView.PlayAnimation())
                    .ContinueWith(() => _healEventViewFactory.RecyclePrefab(healEventView))
                    .Forget();
                break;

            case GetShieldEvent getShieldEvent:
                var shieldEventView = _shieldEventViewFactory.CreatePrefab();
                shieldEventView.SetEventInfo(getShieldEvent, transform);
                
                UniTask.Delay((int)(_nextIntervalTime * 1000))
                    .ContinueWith(() => shieldEventView.PlayAnimation())
                    .ContinueWith(() => _shieldEventViewFactory.RecyclePrefab(shieldEventView))
                    .Forget();
                break;
        }
        
        _nextIntervalTime += _minTimeInterval;
    }

    public void Update()
    {
        _nextIntervalTime = Mathf.Max(0, _nextIntervalTime - Time.deltaTime);
    }
}

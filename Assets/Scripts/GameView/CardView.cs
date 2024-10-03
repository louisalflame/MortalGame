using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _title;
    [SerializeField]
    private TextMeshProUGUI _cost;
    [SerializeField]
    private TextMeshProUGUI _power;

    [BoxGroup("UI")]
    [SerializeField]
    private Button _clickButton;
    [BoxGroup("UI")]
    [SerializeField]
    private Button _pointButton;

    [BoxGroup("Focus")]
    [SerializeField]
    private Transform _content;
    [SerializeField]
    [BoxGroup("Focus")]
    private float _focusScale;
    [SerializeField]
    [BoxGroup("Focus")]
    private Vector3 _focusOffset;
    [BoxGroup("Focus")]
    private float _focusDuration = 0.5f;

    public event Action<CardView> OnFocusStart;
    public event Action<CardView> OnFocusStop;

    private CompositeDisposable _disposables = new CompositeDisposable();
    private CompositeDisposable _useCardDisposables = new CompositeDisposable();
    private Vector3 _localPosition;
    private Quaternion _localRotation; 
    private Dictionary<Guid, List<Vector3>> _offsets = new Dictionary<Guid, List<Vector3>>();
    private Tween _currentMoveTween;

    public void SetCardInfo(CardInfo cardInfo)
    {
        _title.text = cardInfo.Title;
        _cost.text = cardInfo.Cost.ToString();
        _power.text = cardInfo.Power.ToString();

        _pointButton.interactable = true;
        _pointButton.OnPointerEnterAsObservable()
            .Subscribe(_ => _ShowFocusContent(cardInfo)) 
            .AddTo(_disposables);
        _pointButton.OnPointerExitAsObservable()
            .Subscribe(_ => _HideFocusContent(cardInfo)) 
            .AddTo(_disposables);
    }

    public void EnableUseCardAction(CardInfo cardInfo, IGameplayActionReciever reciever)
    {
        _clickButton.interactable = true;
        _clickButton.OnClickAsObservable()
            .Subscribe(_ => 
                reciever.RecieveEvent(
                    new UseCardAction{ CardIndentity = cardInfo.Indentity }))
            .AddTo(_useCardDisposables); 
    }
    public void DisableUseCardAction()
    {
        _clickButton.interactable = false;
        _useCardDisposables.Clear();
    }

    public void Reset()
    {
        _clickButton.interactable = false;
        _pointButton.interactable = false;
        _disposables.Clear();
        _useCardDisposables.Clear();
        _offsets.Clear();
        _UpdateLocalPosition();
        _content.localRotation = Quaternion.identity;
        _content.localScale = Vector3.one;
    } 

    public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        _localPosition = position;
        _localRotation = rotation;
        _UpdateLocalPosition();
        transform.localRotation = _localRotation;
    }

    public void AddLocationOffset(Guid guid, Vector3 offset)
    {
        if(!_offsets.ContainsKey(guid))
        {
            _offsets.Add(guid, new List<Vector3>());
        }
        _offsets[guid].Add(offset);
        _UpdateLocalPosition(_focusDuration);
    }
    public void RemoveLocationOffset(Guid guid)
    {
        if(_offsets.ContainsKey(guid))
        {
            _offsets.Remove(guid);
        }
        _UpdateLocalPosition(_focusDuration);
    }

    private void _ShowFocusContent(CardInfo cardInfo)
    {
        _content.localPosition = _focusOffset;
        _content.localRotation = Quaternion.Inverse(_localRotation);
        _content.localScale = Vector3.one * _focusScale;
        OnFocusStart?.Invoke(this);
    }

    private void _HideFocusContent(CardInfo cardInfo)
    {
        _content.localPosition = Vector3.zero;
        _content.localRotation = Quaternion.identity;
        _content.localScale = Vector3.one;
        OnFocusStop?.Invoke(this);
    }

    private void _UpdateLocalPosition(float duration = 0f)
    {
        var offset = Vector3.zero;
        foreach(var offsets in _offsets.Values)
        {
            foreach(var o in offsets)
            {
                offset += o;
            }
        }

        if(_currentMoveTween != null && _currentMoveTween.IsActive())
        {
            _currentMoveTween.Kill();
        }

        if(duration == 0f)
        {
            transform.localPosition = _localPosition + offset;
        }
        else
        {
            _currentMoveTween = transform.DOLocalMove(_localPosition + offset, 0.5f).SetEase(Ease.OutExpo);
        }
    }
}

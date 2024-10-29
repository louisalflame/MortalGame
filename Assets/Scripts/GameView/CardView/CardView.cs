using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour, IRecyclable
{
    [SerializeField]
    private TextMeshProUGUI _title;
    [SerializeField]
    private TextMeshProUGUI _cost;
    [SerializeField]
    private TextMeshProUGUI _power;

    [BoxGroup("UI")]
    [SerializeField]
    private Button _button;

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

    private CompositeDisposable _disposables = new CompositeDisposable();
    private Vector3 _localPosition;
    private Quaternion _localRotation; 
    private Dictionary<Guid, List<Vector3>> _offsets = new Dictionary<Guid, List<Vector3>>();
    private Tween _currentMoveTween;

    public void SetCardInfo(CardInfo cardInfo)
    {
        _title.text = cardInfo.Title;
        _cost.text = cardInfo.Cost.ToString();
        _power.text = cardInfo.Power.ToString();
    }

    public void EnableHandCardAction(CardInfo cardInfo, IHandCardViewHandler handler)
    {
        _button.interactable = true;
        _button.OnClickAsObservable()
            .Subscribe(_ => handler.UseCard(cardInfo.Indentity))
            .AddTo(_disposables); 
        _button.OnPointerEnterAsObservable()
            .Subscribe(_ => handler.FocusStart(cardInfo.Indentity)) 
            .AddTo(_disposables);
        _button.OnPointerExitAsObservable()
            .Subscribe(_ => handler.FocusStop(cardInfo.Indentity)) 
            .AddTo(_disposables);
    }
    public void EnableSimpleCardAction(CardInfo cardInfo, ISimpleCardViewHandler handler)
    {
        _button.interactable = true;
        _button.OnClickAsObservable()
            .Subscribe(_ => handler.ReadCard(cardInfo.Indentity))
            .AddTo(_disposables);
    }
    public void DisableCardAction()
    {
        _button.interactable = false;
        _disposables.Dispose();

        // Disposed object can't be reused by same instance.
        _disposables = new CompositeDisposable();
    }

    public void ShowHandCardFocusContent()
    {
        _content.localPosition = _focusOffset;
        _content.localRotation = Quaternion.Inverse(_localRotation);
        _content.localScale = Vector3.one * _focusScale;
        transform.SetAsLastSibling();
    }
    public void HideHandCardFocusContent(int originSiblingIndex)
    {
        _ResetFocusContent();
        transform.SetSiblingIndex(originSiblingIndex);
    }

    public void Reset()
    {
        _offsets.Clear();
        _localPosition = Vector3.zero;
        _localRotation = Quaternion.identity;
        transform.localRotation = _localRotation;
        _UpdateLocalPosition();
        _ResetFocusContent();
        DisableCardAction();
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

    private void _ResetFocusContent()
    {
        _content.localPosition = Vector3.zero;
        _content.localRotation = Quaternion.identity;
        _content.localScale = Vector3.one;
    }
}

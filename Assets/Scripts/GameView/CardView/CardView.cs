using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour, IRecyclable, ISelectableView
{
    [BoxGroup("Content")]
    [SerializeField]
    private RectTransform _rectTransform;
    [BoxGroup("Content")]
    [SerializeField]
    private TextMeshProUGUI _title;
    [BoxGroup("Content")]
    [SerializeField]
    private TextMeshProUGUI _info;
    [BoxGroup("Content")]
    [SerializeField]
    private TextMeshProUGUI _cost;
    [BoxGroup("Content")]
    [SerializeField]
    private TextMeshProUGUI _power;

    [BoxGroup("UI")]
    [SerializeField]
    private Button _button;
    [BoxGroup("UI")]
    [SerializeField]
    private CanvasGroup _canvasGroup;

    public RectTransform RectTransform => _rectTransform;
    public TargetType TargetType => TargetType.Card;
    public Guid TargetIdentity => _cardIdentity;

    private CompositeDisposable _disposables = new CompositeDisposable();
    private Vector3 _localPosition;
    private Quaternion _localRotation; 
    private Dictionary<Guid, List<Vector3>> _offsets = new Dictionary<Guid, List<Vector3>>();
    private Tween _currentMoveTween;

    private Guid _cardIdentity;

    public void SetCardInfo(CardInfo cardInfo, LocalizeLibrary localizeLibrary)
    {
        var cardLocalizeData = localizeLibrary.Get(LocalizeTitleInfoType.Card, cardInfo.CardDataID);
        _cardIdentity = cardInfo.Identity;
        _title.text = cardLocalizeData.Title;
        _info.text = cardLocalizeData.Info;
        _cost.text = cardInfo.Cost.ToString();
        _power.text = cardInfo.Power.ToString();
    }

    public void OnSelect()
    {
    }
    public void OnDeselect()
    {
    }

    public void EnableHandCardAction(CardInfo cardInfo, IHandCardViewHandler handler)
    {
        _button.interactable = true;
        
        _button.OnPointerEnterAsObservable()
            .Subscribe(_ => handler.FocusStart(cardInfo.Identity)) 
            .AddTo(_disposables);
        _button.OnPointerExitAsObservable()
            .Subscribe(_ => handler.FocusStop(cardInfo.Identity)) 
            .AddTo(_disposables);
        
        _button.OnBeginDragAsObservable()
            .Subscribe(pointerEventData => handler.BeginDrag(cardInfo.Identity, pointerEventData))
            .AddTo(_disposables);
        _button.OnDragAsObservable()
            .Subscribe(pointerEventData => handler.Drag(cardInfo.Identity, pointerEventData))
            .AddTo(_disposables);
        _button.OnEndDragAsObservable()
            .Subscribe(pointerEventData => handler.EndDrag(cardInfo.Identity, pointerEventData))
            .AddTo(_disposables);
    }
    public void EnableSimpleCardAction(CardInfo cardInfo, IAllCardViewHandler handler)
    {
        _button.interactable = true;
        _button.OnClickAsObservable()
            .Subscribe(_ => handler.ReadCard(cardInfo.Identity))
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
        _canvasGroup.alpha = 0f;
    }
    public void HideHandCardFocusContent()
    {
        _canvasGroup.alpha = 1f;
    }

    public void BeginDrag(Vector2 dragPosition)
    {
        RectTransform.rotation = Quaternion.identity;
        _canvasGroup.alpha = 1f;
        transform.SetAsLastSibling();
    }
    public void Drag(Vector2 dragPosition, SelectType selectType, bool isSelecting)
    {
        RectTransform.anchoredPosition = dragPosition;
        _canvasGroup.alpha = 
            isSelecting ? (selectType == SelectType.None ? 1f: 0f) : 0.5f;
    }
    public void EndDrag(Vector2 beginDragPosition, int originSiblingIndex)
    {
        RectTransform.rotation = _localRotation;
        RectTransform.anchoredPosition = beginDragPosition;
        _canvasGroup.alpha = 1f;
        transform.SetSiblingIndex(originSiblingIndex);
    }

    public void Reset()
    {
        _cardIdentity = Guid.Empty;
        _canvasGroup.alpha = 1f;
        _offsets.Clear();
        _localPosition = Vector3.zero;
        _localRotation = Quaternion.identity;
        transform.localRotation = _localRotation;
        _UpdateLocalPosition();
        DisableCardAction();
    } 

    public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        _localPosition = position;
        _localRotation = rotation;
        _UpdateLocalPosition();
        transform.localRotation = _localRotation;
    }

    public void AddLocationOffset(Guid guid, Vector3 offset, float duration)
    {
        if(!_offsets.ContainsKey(guid))
        {
            _offsets.Add(guid, new List<Vector3>());
        }
        _offsets[guid].Add(offset);
        _UpdateLocalPosition(duration);
    }
    public void RemoveLocationOffset(Guid guid, float duration)
    {
        if(_offsets.ContainsKey(guid))
        {
            _offsets.Remove(guid);
        }
        _UpdateLocalPosition(duration);
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
            _currentMoveTween = transform
                .DOLocalMove(_localPosition + offset, 0.5f)
                .SetEase(Ease.OutExpo)
                .OnComplete(() => _currentMoveTween.Kill());
        }
    }
}

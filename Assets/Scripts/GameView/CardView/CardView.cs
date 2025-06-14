using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public interface ICardView : IRecyclable, ISelectableView
{
    Canvas Canvas { get; }
    RectTransform ParentRectTransform { get; }

    void Initialize(IGameInfoModel gameInfoModel, LocalizeLibrary localizeLibrary);
    void SetCardInfo(CardInfo cardInfo);

    void SetPositionAndRotation(Vector3 position, Quaternion rotation);
    void AddLocationOffset(Guid guid, Vector3 offset, float duration);
    void RemoveLocationOffset(Guid guid, float duration);

    void ShowHandCardFocusContent();
    void HideHandCardFocusContent();

    void BeginDrag(Vector2 dragPosition);
    void Drag(Vector2 dragPosition, SelectType selectType, bool isSelecting);
    void EndDrag(Vector2 beginDragPosition, int originSiblingIndex);

    void EnableHandCardAction(CardInfo cardInfo, IHandCardViewHandler handler);
    void DisableCardAction();
}

public class CardView : MonoBehaviour, ICardView
{
    [BoxGroup("Content")]
    [SerializeField]
    private RectTransform _rectTransform;
    [BoxGroup("Content")]
    [SerializeField]
    private Button _button;
    [BoxGroup("Content")]
    [SerializeField]
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private TextMeshProUGUI _title;
    [SerializeField]
    private TextMeshProUGUI _info;
    [SerializeField]
    private TextMeshProUGUI _type;
    [SerializeField]
    private TextMeshProUGUI _cost;
    [SerializeField]
    private IntValueSwitch _costColorSwitch;    
    [SerializeField]
    private IntColorMapping _powerColorMapping;

    [TitleGroup("Effect")]
    [SerializeField]
    private GameObject _sealedEffectObj;

    public RectTransform RectTransform => _rectTransform;
    public TargetType TargetType => TargetType.AllyCard;
    public Guid TargetIdentity => _cardIdentity;

    public RectTransform ParentRectTransform => transform.parent.GetComponent<RectTransform>();
    public Canvas Canvas => transform.GetComponentInParent<Canvas>();

    private CompositeDisposable _disposables = new CompositeDisposable();
    private IDisposable _cardInfoSubscription;
    private Vector3 _localPosition;
    private Quaternion _localRotation; 
    private Dictionary<Guid, List<Vector3>> _offsets = new Dictionary<Guid, List<Vector3>>();
    private Tween _currentMoveTween;

    private IGameInfoModel _gameInfoModel;
    private LocalizeLibrary _localizeLibrary;
    private Guid _cardIdentity;

    public void Initialize(
        IGameInfoModel gameInfoModel,
        LocalizeLibrary localizeLibrary)
    {
        _gameInfoModel = gameInfoModel;
        _localizeLibrary = localizeLibrary;
    }

    public void SetCardInfo(CardInfo cardInfo)
    {
        _cardInfoSubscription = _gameInfoModel.ObserveCardInfo(cardInfo.Identity)
            .Where(info => info != null && info.Identity == cardInfo.Identity)
            .Subscribe(info => _Render(info));
        _Render(cardInfo);
    }
    private void _Render(CardInfo cardInfo)
    {
        var cardLocalizeData = _localizeLibrary.Get(LocalizeTitleInfoType.Card, cardInfo.CardDataID);
        var templateValue = cardInfo.GetTemplateValues();

        _cardIdentity = cardInfo.Identity;
        _cost.text = cardInfo.Cost.ToString();
        _costColorSwitch.Value =
            cardInfo.Cost > cardInfo.OriginCost ? 1 :
            cardInfo.Cost < cardInfo.OriginCost ? 2 : 0;

        var originPowerString = templateValue[CardInfo.KEY_POWER];
        templateValue[CardInfo.KEY_POWER] = 
            cardInfo.Power > cardInfo.OriginPower ? $"<color=#{_powerColorMapping.GetHtmlColor(1)}>{originPowerString}</color>" :
            cardInfo.Power < cardInfo.OriginPower ? $"<color=#{_powerColorMapping.GetHtmlColor(2)}>{originPowerString}</color>" :
            originPowerString;

        _title.text = cardLocalizeData.Title;
        _info.text = cardLocalizeData.Info.ReplaceTemplateKeys(templateValue);
        _type.text = _localizeLibrary.Get(LocalizeTitleInfoType.GameKeyWord, cardInfo.Type.ToString()).Title;

        _sealedEffectObj.SetActive(cardInfo.Properties.Contains(CardProperty.Sealed));
    }

    public void OnSelect()
    {
    }
    public void OnDeselect()
    {
    }

    public void EnableHandCardAction(CardInfo cardInfo, IHandCardViewHandler handler)
    {
        _Render(cardInfo);
        _button.interactable = true;
        
        _button.OnPointerEnterAsObservable()
            .Subscribe(_ => handler.FocusStart(cardInfo.Identity)) 
            .AddTo(_disposables);
        _button.OnPointerExitAsObservable()
            .Subscribe(_ => handler.FocusStop(cardInfo.Identity)) 
            .AddTo(_disposables);
        
        _button.OnBeginDragAsObservable()
            .Subscribe(pointerEventData => handler.BeginDrag(cardInfo.Identity, pointerEventData.position))
            .AddTo(_disposables);
        _button.OnDragAsObservable()
            .Subscribe(pointerEventData => handler.Drag(cardInfo.Identity, pointerEventData.position))
            .AddTo(_disposables);
        _button.OnEndDragAsObservable()
            .Subscribe(pointerEventData => handler.EndDrag(cardInfo.Identity, pointerEventData.position))
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
        _cardInfoSubscription?.Dispose();
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

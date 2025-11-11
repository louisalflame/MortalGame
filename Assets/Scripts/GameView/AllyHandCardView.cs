using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using Optional.Unsafe;
using Optional.Linq;
using Optional.Utilities;
using Optional.Collections;
using Sirenix.OdinInspector;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;


public class AllyHandCardView : MonoBehaviour
{
    [BoxGroup("Card View")]
    [SerializeField]
    private CardViewFactory _cardViewFactory;
    [BoxGroup("Card View")]
    [SerializeField]
    private Transform _cardViewParent;

    [BoxGroup("Arc Setting")]
    [SerializeField]
    private float _arcAngle;
    [BoxGroup("Arc Setting")]
    [SerializeField]
    private float _arcRadiusX;
    [BoxGroup("Arc Setting")]
    [SerializeField]
    private float _arcRadiusY;
    [BoxGroup("Arc Setting")]
    [SerializeField]
    private float _arcStepMinAngle;

    [BoxGroup("Focus")]
    [SerializeField]
    private float _focusOtherOffsetX;
    [BoxGroup("Focus")]
    [SerializeField]
    private float _focusDuration = 0.5f;
    
    [BoxGroup("Arrow Setting")]
    [SerializeField]
    private LineRenderer _lineRenderer;
    [BoxGroup("Arrow Setting")]
    [SerializeField]
    private float _curveHeight;
    [BoxGroup("Arrow Setting")]
    [SerializeField]
    private int _curveResolution;    

    private List<ICardView> _cardViews = new List<ICardView>();
    private Dictionary<Guid, ICardView> _cardViewDict = new Dictionary<Guid, ICardView>();
    private IGameplayStatusWatcher _statusWatcher;
    private IGameViewModel _gameViewModel;
    private IGameplayActionReciever _reciever;
    private LocalizeLibrary _localizeLibrary;

    // Focusing
    private FocusCardDetailView _focusCardDetailView;
    // Dragging
    private Vector2 _beginDragPosition;
    private Vector2 _dragOffset;
    private ISelectableView _currentSelectedView;

    public IEnumerable<ISelectableView> SelectableViews => _cardViews;
    
    private CompositeDisposable _handleDisposables = new CompositeDisposable();
    private readonly ReactiveProperty<Option<CardInfo>> _currentFocusInfo = new(Option.None<CardInfo>());
    private readonly ReactiveProperty<Option<(CardInfo, Vector2)>> _currentDragInfo = new(Option.None<(CardInfo, Vector2)>());
    private IReadOnlyReactiveProperty<bool> IsDragging => _currentDragInfo.Select(info => info.HasValue).ToReactiveProperty();

    public void Init(
        IGameplayStatusWatcher statusWatcher,
        IGameplayActionReciever reciever,
        IGameViewModel gameInfoModel,
        IAllCardDetailPanelView allCardDetailPanelView,
        LocalizeLibrary localizeLibrary)
    {
        _statusWatcher = statusWatcher;
        _reciever = reciever;
        _gameViewModel = gameInfoModel;
        _focusCardDetailView = allCardDetailPanelView.FocusCardDetailView;
        _localizeLibrary = localizeLibrary;
    }

    public void CreateCardView(CardInfo newCardInfo, CardCollectionInfo handCardInfo)
    {
        var cardView = _cardViewFactory.CreatePrefab();
        cardView.Initialize(_gameViewModel, _localizeLibrary);
        cardView.transform.SetParent(_cardViewParent, false);
        cardView.SetCardInfo(newCardInfo);
        _cardViews.Add(cardView);
        _cardViewDict.Add(newCardInfo.Identity, cardView);

        _RearrangeCardViews();
    }
    
    public void RemoveCardView(UsedCardEvent usedCardEvent)
    {
        if(_cardViewDict.TryGetValue(usedCardEvent.UsedCardInfo.Identity, out var cardView))
        {
            _cardViews.Remove(cardView);
            _cardViewDict.Remove(usedCardEvent.UsedCardInfo.Identity);
            _cardViewFactory.RecyclePrefab(cardView as CardView);

            foreach(var view in _cardViews)
                view.RemoveLocationOffset(usedCardEvent.UsedCardInfo.Identity, _focusDuration);
            _RearrangeCardViews();
        }
    }

    public void RemoveCardView(MoveCardEvent moveCardEvent)
    {
        if(_cardViewDict.TryGetValue(moveCardEvent.CardInfo.Identity, out var cardView))
        {
            _cardViews.Remove(cardView);
            _cardViewDict.Remove(moveCardEvent.CardInfo.Identity);
            _cardViewFactory.RecyclePrefab(cardView as CardView);

            foreach(var view in _cardViews)
                view.RemoveLocationOffset(moveCardEvent.CardInfo.Identity, _focusDuration);
            _RearrangeCardViews();
        }
    }

    public void RecycleHandCards(RecycleHandCardEvent recycleHandCardEvent)
    {
        foreach(var cardInfo in recycleHandCardEvent.RecycledCardInfos.Concat(recycleHandCardEvent.ExcludedCardInfos))
        {
            if(_cardViewDict.TryGetValue(cardInfo.Identity, out var cardView))
            {
                _cardViews.Remove(cardView);
                _cardViewDict.Remove(cardInfo.Identity);
                
                _cardViewFactory.RecyclePrefab(cardView as CardView);
                foreach(var view in _cardViews)
                    view.RemoveLocationOffset(cardInfo.Identity, _focusDuration);
            }
        }

        _RearrangeCardViews();
    }
    
    public void EnableHandCardsUseCardAction(PlayerExecuteStartEvent playerExecuteStartEvent)
    {
        _handleDisposables.Dispose();
        _handleDisposables = new CompositeDisposable();

        var handCardInfos = playerExecuteStartEvent.CardManagerInfo.CardZoneInfos[CardCollectionType.HandCard].CardInfos;
        foreach (var cardInfo in handCardInfos.Keys)
        {
            if (_cardViewDict.TryGetValue(cardInfo.Identity, out var cardView))
            {
                cardView.SetCardInfo(cardInfo);
                cardView.Button.OnPointerEnterAsObservable()
                    .Where(_ => IsDragging.Value == false)
                    .Subscribe(_ => _currentFocusInfo.Value = cardInfo.Some())
                    .AddTo(_handleDisposables);
                cardView.Button.OnPointerExitAsObservable()
                    .Subscribe(_ => _currentFocusInfo.Value = Option.None<CardInfo>())
                    .AddTo(_handleDisposables);

                cardView.Button.OnBeginDragAsObservable()
                    .Subscribe(data => _currentDragInfo.Value = (cardInfo, data.position).Some())
                    .AddTo(_handleDisposables);
                cardView.Button.OnDragAsObservable()
                    .Where(_ => _currentDragInfo.Value.Match(
                        info => info.Item1.Identity == cardInfo.Identity,
                        () => false))
                    .Subscribe(data => _currentDragInfo.Value = (cardInfo, data.position).Some())
                    .AddTo(_handleDisposables);
                cardView.Button.OnEndDragAsObservable()
                    .Where(_ => _currentDragInfo.Value.Match(
                        info => info.Item1.Identity == cardInfo.Identity,
                        () => false))
                    .Subscribe(data => _currentDragInfo.Value = Option.None<(CardInfo, Vector2)>())
                    .AddTo(_handleDisposables);
            }
        }

        _currentFocusInfo
            .Scan(
                seed: (Previous: Option.None<CardInfo>(), Current: Option.None<CardInfo>()),
                accumulator: (acc, current) => (Previous: acc.Current, Current: current)
            )
            .DistinctUntilChanged()
            .Subscribe(pair => _HandleFocusInfoChange(pair.Previous, pair.Current, handCardInfos))
            .AddTo(_handleDisposables);

        _currentDragInfo
            .Scan(
                seed: (Previous: Option.None<(CardInfo, Vector2)>(), Current: Option.None<(CardInfo, Vector2)>()),
                accumulator: (acc, current) => (Previous: acc.Current, Current: current)
            )
            .WithLatestFrom(
                _currentFocusInfo,
                (dragInfoPair, focusInfo) => (dragInfoPair.Previous, dragInfoPair.Current, focusInfo))
            .Subscribe(pair => _HandleDragInfoChange(pair.Previous, pair.Current, pair.focusInfo, handCardInfos))
            .AddTo(_handleDisposables);
    }
    private void _HandleFocusInfoChange(
        Option<CardInfo> previousFocusInfoOpt,
        Option<CardInfo> currentFocusInfoOpt,
        IReadOnlyDictionary<CardInfo, int> handCardInfos)
    {
        if (currentFocusInfoOpt.TryGetValue(out var focusCardInfo))
        {
            if (_cardViewDict.TryGetValue(focusCardInfo.Identity, out var focusCardView) &&
                handCardInfos.TryGetValue(focusCardInfo, out var focusCardIndex))
            {
                var smaller = handCardInfos
                    .Where(kvp => kvp.Value < focusCardIndex)
                    .Select(kvp => kvp.Key.Identity);
                var larger = handCardInfos
                    .Where(kvp => kvp.Value > focusCardIndex)
                    .Select(kvp => kvp.Key.Identity);

                foreach (var identity in smaller)
                {
                    if (_cardViewDict.TryGetValue(identity, out var cardView))
                    {
                        var offset = new Vector3(-_focusOtherOffsetX, 0, 0);
                        cardView.AddLocationOffset(focusCardInfo.Identity, offset, _focusDuration);
                    }
                }
                foreach (var identity in larger)
                {
                    if (_cardViewDict.TryGetValue(identity, out var cardView))
                    {
                        var offset = new Vector3(_focusOtherOffsetX, 0, 0);
                        cardView.AddLocationOffset(focusCardInfo.Identity, offset, _focusDuration);
                    }
                }

                focusCardView.ShowHandCardFocusContent();
                _focusCardDetailView.ShowFocus(focusCardInfo, focusCardView.RectTransform);
            }
        }
        else if (previousFocusInfoOpt.TryGetValue(out var previousFocusCardInfo))
        {
            if (_cardViewDict.TryGetValue(previousFocusCardInfo.Identity, out var focusView))
            {
                _FocusStop(focusView, previousFocusCardInfo.Identity);
            }
        }
    }
    private void _HandleDragInfoChange(
        Option<(CardInfo Info, Vector2 Position)> PreviousDragInfoOpt,
        Option<(CardInfo Info, Vector2 Position)> CurrentDragInfoOpt,
        Option<CardInfo> FocusInfoOpt,
        IReadOnlyDictionary<CardInfo, int> handCardInfos)
    {
        if (!PreviousDragInfoOpt.HasValue && CurrentDragInfoOpt.TryGetValue(out var newDragInfo))
        {
            if (_cardViewDict.TryGetValue(newDragInfo.Info.Identity, out var dragView))
            {
                // Clear focus content
                if (FocusInfoOpt.TryGetValue(out var focusInfo) &&
                    _cardViewDict.TryGetValue(focusInfo.Identity, out var focusView))
                {
                    _FocusStop(focusView, focusInfo.Identity);
                }

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    dragView.ParentRectTransform, newDragInfo.Position, dragView.Canvas.worldCamera, out Vector2 localPoint);
                _beginDragPosition = dragView.RectTransform.anchoredPosition;
                _dragOffset = _beginDragPosition - localPoint;
                dragView.BeginDrag(_beginDragPosition);
            }
        }
        else if (PreviousDragInfoOpt.TryGetValue(out var latestDragInfo) && !CurrentDragInfoOpt.HasValue)
        {
            if (_cardViewDict.TryGetValue(latestDragInfo.Info.Identity, out var dragView) &&
                handCardInfos.TryGetValue(latestDragInfo.Info, out var originIndex))
            {
                _lineRenderer.gameObject.SetActive(false);
                dragView.EndDrag(_beginDragPosition, originIndex);

                if (latestDragInfo.Info.MainSelectable.SelectType != SelectType.None)
                {
                    var selectView = _reciever.SelectableViews
                        .Where(view => view != dragView as ISelectableView)
                        .Where(view => latestDragInfo.Info.MainSelectable.SelectType.IsSelectable(view.TargetType))
                        .Where(view => RectTransformUtility.RectangleContainsScreenPoint(
                            view.RectTransform, latestDragInfo.Position, dragView.Canvas.worldCamera))
                        .FirstOrDefault();
                    if (selectView != null)
                    {
                        _reciever.RecieveEvent(new UseCardAction(
                            latestDragInfo.Info.Identity, selectView.TargetType, selectView.TargetIdentity));
                    }
                }
                else
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(
                        _reciever.BasicSelectableView.RectTransform, latestDragInfo.Position, dragView.Canvas.worldCamera))
                    {
                        _reciever.RecieveEvent(new UseCardAction(latestDragInfo.Info.Identity));
                    }
                }
            }
        }
        else if (CurrentDragInfoOpt.TryGetValue(out var currentDragInfo) &&
                 PreviousDragInfoOpt.HasValue)
        {
            if (_cardViewDict.TryGetValue(currentDragInfo.Info.Identity, out var dragView))
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    dragView.ParentRectTransform, currentDragInfo.Position, dragView.Canvas.worldCamera, out Vector2 localPoint);
                var localDragPoint = localPoint + _dragOffset;

                if (currentDragInfo.Info.MainSelectable.SelectType != SelectType.None)
                {
                    var selectView = _reciever.SelectableViews
                        .Where(view => view != dragView as ISelectableView)
                        .Where(view => currentDragInfo.Info.MainSelectable.SelectType.IsSelectable(view.TargetType))
                        .Where(view => RectTransformUtility.RectangleContainsScreenPoint(
                            view.RectTransform, currentDragInfo.Position, dragView.Canvas.worldCamera))
                        .FirstOrDefault();
                    if (_currentSelectedView != selectView)
                    {
                        _currentSelectedView?.OnDeselect();
                        selectView?.OnSelect();
                        _currentSelectedView = selectView;
                    }

                    if (selectView != null)
                    {
                        var beginDragWorldPos = dragView.ParentRectTransform.TransformPoint(_beginDragPosition);
                        var beginDragLinePos = _lineRenderer.transform.parent.InverseTransformPoint(beginDragWorldPos);
                        var selectViewLinePos = _lineRenderer.transform.parent.InverseTransformPoint(selectView.RectTransform.position);
                        _lineRenderer.gameObject.SetActive(true);
                        var points = _GenerateCurvePoints(beginDragLinePos, selectViewLinePos);
                        _lineRenderer.positionCount = points.Length;
                        _lineRenderer.SetPositions(points);
                        dragView.Drag(localDragPoint, currentDragInfo.Info.MainSelectable.SelectType, true);
                    }
                    else
                    {
                        _lineRenderer.gameObject.SetActive(false);
                        dragView.Drag(localDragPoint, currentDragInfo.Info.MainSelectable.SelectType, false);
                    }
                }
                else
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(
                        _reciever.BasicSelectableView.RectTransform, currentDragInfo.Position, dragView.Canvas.worldCamera))
                    {
                        dragView.Drag(localDragPoint, currentDragInfo.Info.MainSelectable.SelectType, true);
                    }
                    else
                    {
                        dragView.Drag(localDragPoint, currentDragInfo.Info.MainSelectable.SelectType, false);
                    }
                }
            }        
        }
    }
    private void _FocusStop(ICardView focusView, Guid focusIdentity)
    {    
        focusView.HideHandCardFocusContent();
        _focusCardDetailView.HideFocus();
        
        foreach(var cardView in _cardViews)
        {
            cardView.RemoveLocationOffset(
                focusIdentity,
                // focusing view dont need to play animation of return location
                cardView == focusView ? 0f : _focusDuration);
        }
    }

    public void DisableAllHandCardsAction()
    {
        _handleDisposables.Dispose();
        _handleDisposables = new CompositeDisposable();
    }

    private void _RearrangeCardViews()
    {
        var cardCount = _cardViews.Count; 
        if(cardCount <= 0 ) return;
        
        float centerIndex = (cardCount - 1) / 2f;
        var angleStep = _arcAngle / (cardCount-1);
        angleStep = Mathf.Min(angleStep, _arcStepMinAngle);

        for(var i = 0; i < cardCount; i++)
        {
            var cardView = _cardViews[i];
            float angle = 90 + (centerIndex - i) * angleStep; 

            var x = _arcRadiusX * Mathf.Cos(angle * Mathf.Deg2Rad);
            var y = _arcRadiusY * Mathf.Sin(angle * Mathf.Deg2Rad);
            var localPosition = new Vector3(x, y, 0);
            var localRotation = Quaternion.Euler(0, 0, angle - 90);
            cardView.SetPositionAndRotation(localPosition, localRotation);
        }
    }
     
    private Vector3[] _GenerateCurvePoints(Vector3 startPoint, Vector3 endPoint)
    {
        var points = new Vector3[_curveResolution];
        Vector3 midPoint = (startPoint + endPoint) / 2;
        midPoint += Vector3.up * _curveHeight;

        Vector3 controlPoint0 = startPoint + (startPoint - midPoint);
        Vector3 controlPoint1 = startPoint;
        Vector3 controlPoint2 = endPoint;
        Vector3 controlPoint3 = endPoint + (endPoint - midPoint);

        for (int i = 0; i < _curveResolution; i++)
        {
            float t = (float)i / (_curveResolution - 1);
            Vector3 point = _CatmullRom(controlPoint0, controlPoint1, controlPoint2, controlPoint3, t);
            points[i] = point;
        }

        return points;
    }
    private Vector3 _CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * ((2.0f * p1) +
                       (-p0 + p2) * t +
                       (2.0f * p0 - 5.0f * p1 + 4.0f * p2 - p3) * t2 +
                       (-p0 + 3.0f * p1 - 3.0f * p2 + p3) * t3);
    }
}
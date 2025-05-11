using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IHandCardViewHandler
{
    void FocusStart(Guid cardIdentity);
    void FocusStop(Guid cardIdentity);
    void BeginDrag(Guid cardIdentity, PointerEventData pointerEventData);
    void Drag(Guid cardIdentity, PointerEventData pointerEventData);
    void EndDrag(Guid cardIdentity, PointerEventData pointerEventData);
}

public class AllyHandCardView : MonoBehaviour, IHandCardViewHandler
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
    private Canvas _canvas;
    [BoxGroup("Arrow Setting")]
    [SerializeField]
    private LineRenderer _lineRenderer;
    [BoxGroup("Arrow Setting")]
    [SerializeField]
    private float _curveHeight;
    [BoxGroup("Arrow Setting")]
    [SerializeField]
    private int _curveResolution;    

    private List<CardView> _cardViews = new List<CardView>();
    private Dictionary<Guid, CardView> _cardViewDict = new Dictionary<Guid, CardView>();
    private IGameplayStatusWatcher _statusWatcher;
    private IGameplayActionReciever _reciever;
    private LocalizeLibrary _localizeLibrary;
    private CardCollectionInfo _cardCollectionInfo;

    // Focusing
    private KeyValuePair<CardInfo, int> _focusingCardInfo;
    private FocusCardDetailView _focusCardDetailView;
    // Dragging
    private KeyValuePair<CardInfo, int> _draggingCardInfo;
    private Vector2 _beginDragPosition;
    private Vector2 _dragOffset;
    private ISelectableView _currentSelectedView;

    public IEnumerable<ISelectableView> SelectableViews => _cardViews;

    public void Init(
        IGameplayStatusWatcher statusWatcher, 
        IGameplayActionReciever reciever, 
        IAllCardDetailPanelView allCardDetailPanelView,
        LocalizeLibrary localizeLibrary)
    {
        _statusWatcher = statusWatcher;
        _reciever = reciever;
        _focusCardDetailView = allCardDetailPanelView.FocusCardDetailView;
        _localizeLibrary = localizeLibrary;
    }

    public void CreateCardView(CardInfo newCardInfo, CardCollectionInfo handCardInfo)
    {
        _cardCollectionInfo = handCardInfo;
        var cardView = _cardViewFactory.CreatePrefab();
        cardView.transform.SetParent(_cardViewParent, false);
        cardView.SetCardInfo(newCardInfo, _localizeLibrary);
        _cardViews.Add(cardView);
        _cardViewDict.Add(newCardInfo.Identity, cardView);

        _RearrangeCardViews();
    }

    public void FocusStart(Guid focusIdentity)
    {
        if (_draggingCardInfo.Key == null &&
            _focusingCardInfo.Key == null &&
            _cardViewDict.TryGetValue(focusIdentity, out var focusView))
        {
            _focusingCardInfo = _cardCollectionInfo.CardInfos
                .FirstOrDefault(kvp => kvp.Key.Identity == focusIdentity);
            
            var smaller = _cardCollectionInfo.CardInfos
                .Where(kvp => kvp.Value < _focusingCardInfo.Value)
                .Select(kvp => kvp.Key.Identity);
            var larger = _cardCollectionInfo.CardInfos
                .Where(kvp => kvp.Value > _focusingCardInfo.Value)
                .Select(kvp => kvp.Key.Identity);
            
            foreach(var identity in smaller)
            {
                if(_cardViewDict.TryGetValue(identity, out var cardView))
                {
                    var offset = new Vector3(-_focusOtherOffsetX, 0, 0);
                    cardView.AddLocationOffset(focusIdentity, offset, _focusDuration);
                }
            }
            foreach(var identity in larger)
            {
                if(_cardViewDict.TryGetValue(identity, out var cardView))
                {
                    var offset = new Vector3(_focusOtherOffsetX, 0, 0);
                    cardView.AddLocationOffset(focusIdentity, offset, _focusDuration);
                }
            }

            focusView.ShowHandCardFocusContent();
            _focusCardDetailView.ShowFocus(_focusingCardInfo.Key, focusView.RectTransform);
        }
    }
    public void FocusStop(Guid focusIdentity)
    {
        if (_draggingCardInfo.Key == null &&
            _focusingCardInfo.Key != null &&
            _cardViewDict.TryGetValue(focusIdentity, out var focusView))
        {
            focusView.HideHandCardFocusContent();
            _focusCardDetailView.HideFocus();
            _focusingCardInfo = default;
            
            foreach(var cardView in _cardViews)
            {
                cardView.RemoveLocationOffset(focusIdentity, _focusDuration);
            }
        }
    }

    public void BeginDrag(Guid dragIdentity, PointerEventData pointerEventData)
    {
        if (_draggingCardInfo.Key == null &&
            _cardViewDict.TryGetValue(dragIdentity, out var dragView))
        {
            // Clear focus content
            if (_focusingCardInfo.Key != null && 
                _cardViewDict.TryGetValue(_focusingCardInfo.Key.Identity, out var focusView))
            {
                focusView.HideHandCardFocusContent();
                _focusCardDetailView.HideFocus();
                _focusingCardInfo = default;
                foreach(var cardView in _cardViews.Where(view => view != focusView))
                {
                    cardView.RemoveLocationOffset(dragIdentity, _focusDuration);
                }
            }

            _draggingCardInfo = _cardCollectionInfo.CardInfos
                .FirstOrDefault(kvp => kvp.Key.Identity == dragIdentity);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform, pointerEventData.position, _canvas.worldCamera, out Vector2 localPoint);    
            _beginDragPosition = dragView.transform.GetComponent<RectTransform>().anchoredPosition;
            _dragOffset = _beginDragPosition - localPoint;
            dragView.BeginDrag(_beginDragPosition);
        }
    }
    public void Drag(Guid dragIdentity, PointerEventData pointerEventData)
    {
        if (_draggingCardInfo.Key != null &&
            _cardViewDict.TryGetValue(dragIdentity, out var dragView))
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform, pointerEventData.position, _canvas.worldCamera, out Vector2 localPoint);
            var localDragPoint = localPoint + _dragOffset;

            var draggingInfo = _draggingCardInfo.Key;
            if (draggingInfo.MainSelectable.SelectType != SelectType.None)
            {
                var selectView = _reciever.SelectableViews
                    .Where(view => view != dragView as ISelectableView)
                    .Where(view => draggingInfo.MainSelectable.SelectType.IsSelectable(view.TargetType))
                    .Where(view => RectTransformUtility.RectangleContainsScreenPoint(view.RectTransform, pointerEventData.position, _canvas.worldCamera))
                    .FirstOrDefault();
                if (_currentSelectedView != selectView)
                {
                    _currentSelectedView?.OnDeselect();
                    selectView?.OnSelect();
                    _currentSelectedView = selectView;
                }

                if (selectView != null)
                {
                    var beginDragWorldPos = dragView.transform.parent.TransformPoint(_beginDragPosition);
                    var beginDragLinePos = _lineRenderer.transform.parent.InverseTransformPoint(beginDragWorldPos);
                    var selectViewLinePos = _lineRenderer.transform.parent.InverseTransformPoint(selectView.RectTransform.position);
                    _lineRenderer.gameObject.SetActive(true);
                    var points = _GenerateCurvePoints(beginDragLinePos, selectViewLinePos);
                    _lineRenderer.positionCount = points.Length;
                    _lineRenderer.SetPositions(points);                    
                    dragView.Drag(localDragPoint, draggingInfo.MainSelectable.SelectType, true);
                }
                else
                {
                    _lineRenderer.gameObject.SetActive(false);
                    dragView.Drag(localDragPoint, draggingInfo.MainSelectable.SelectType, false);  
                }
            }
            else
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(_reciever.BasicSelectableView.RectTransform, pointerEventData.position, _canvas.worldCamera))
                {
                    dragView.Drag(localDragPoint, draggingInfo.MainSelectable.SelectType, true);
                }
                else
                {
                    dragView.Drag(localDragPoint, draggingInfo.MainSelectable.SelectType, false);
                }
            }
        }
    }
    public void EndDrag(Guid dragIdentity, PointerEventData pointerEventData)
    {
        if (_draggingCardInfo.Key != null &&
            _cardViewDict.TryGetValue(dragIdentity, out var dragView))
        {
            _lineRenderer.gameObject.SetActive(false);
            dragView.EndDrag(_beginDragPosition, _draggingCardInfo.Value);

            var draggingInfo = _draggingCardInfo.Key;
            if (draggingInfo.MainSelectable.SelectType != SelectType.None)
            {
                var selectView = _reciever.SelectableViews
                    .Where(view => view != dragView as ISelectableView)
                    .Where(view => draggingInfo.MainSelectable.SelectType.IsSelectable(view.TargetType))
                    .Where(view => RectTransformUtility.RectangleContainsScreenPoint(view.RectTransform, pointerEventData.position, _canvas.worldCamera))
                    .FirstOrDefault();
                if (selectView != null)
                {
                    _reciever.RecieveEvent(new UseCardAction(
                        dragIdentity, selectView.TargetType, selectView.TargetIdentity));
                }
            }
            else
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(_reciever.BasicSelectableView.RectTransform, pointerEventData.position, _canvas.worldCamera))
                {
                    _reciever.RecieveEvent(new UseCardAction(dragIdentity));
                }
            }

            _draggingCardInfo = default;
        }
    }

    public void RemoveCardView(UsedCardEvent usedCardEvent)
    {
        _cardCollectionInfo = usedCardEvent.HandCardInfo;
        if(_cardViewDict.TryGetValue(usedCardEvent.UsedCardInfo.Identity, out var cardView))
        {
            _cardViews.Remove(cardView);
            _cardViewDict.Remove(usedCardEvent.UsedCardInfo.Identity);
            _cardViewFactory.RecyclePrefab(cardView);

            foreach(var view in _cardViews)
                view.RemoveLocationOffset(usedCardEvent.UsedCardInfo.Identity, _focusDuration);
            _RearrangeCardViews();
        }
    }

    public void RemoveCardView(DiscardCardEvent discardCardEvent)
    {
        _cardCollectionInfo = discardCardEvent.StartZoneInfo;
        if(_cardViewDict.TryGetValue(discardCardEvent.CardInfo.Identity, out var cardView))
        {
            _cardViews.Remove(cardView);
            _cardViewDict.Remove(discardCardEvent.CardInfo.Identity);
            _cardViewFactory.RecyclePrefab(cardView);

            foreach(var view in _cardViews)
                view.RemoveLocationOffset(discardCardEvent.CardInfo.Identity, _focusDuration);
            _RearrangeCardViews();
        }
    }
    public void RemoveCardView(ConsumeCardEvent consumeCardEvent)
    {
        _cardCollectionInfo = consumeCardEvent.StartZoneInfo;
        if(_cardViewDict.TryGetValue(consumeCardEvent.CardInfo.Identity, out var cardView))
        {
            _cardViews.Remove(cardView);
            _cardViewDict.Remove(consumeCardEvent.CardInfo.Identity);
            _cardViewFactory.RecyclePrefab(cardView);

            foreach(var view in _cardViews)
                view.RemoveLocationOffset(consumeCardEvent.CardInfo.Identity, _focusDuration);
            _RearrangeCardViews();
        }
    }
    public void RemoveCardView(DisposeCardEvent disposeCardEvent)
    {
        _cardCollectionInfo = disposeCardEvent.StartZoneInfo;
        if(_cardViewDict.TryGetValue(disposeCardEvent.CardInfo.Identity, out var cardView))
        {
            _cardViews.Remove(cardView);
            _cardViewDict.Remove(disposeCardEvent.CardInfo.Identity);
            _cardViewFactory.RecyclePrefab(cardView);

            foreach(var view in _cardViews)
                view.RemoveLocationOffset(disposeCardEvent.CardInfo.Identity, _focusDuration);
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
                _cardViewFactory.RecyclePrefab(cardView);
                foreach(var view in _cardViews)
                    view.RemoveLocationOffset(cardInfo.Identity, _focusDuration);
            }
        }

        _RearrangeCardViews();
    }

    public void EnableHandCardsUseCardAction(PlayerExecuteStartEvent playerExecuteStartEvent)
    {
        foreach(var cardInfo in playerExecuteStartEvent.HandCardInfo.CardInfos.Keys)
        {
            if(_cardViewDict.TryGetValue(cardInfo.Identity, out var cardView))
            {
                cardView.EnableHandCardAction(cardInfo, this);
            }
        }
    }
    public void DisableAllHandCards()
    {
        foreach(var cardView in _cardViewDict.Values)
        {
            cardView.DisableCardAction();
        }
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
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IHandCardViewHandler
{
    void UseCard(Guid cardIdentity);
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
    [BoxGroup("Arc Setting")]
    [SerializeField]
    private float _focusOffsetX;
    
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
    
    [SerializeField]
    private CardPropertyHint _cardPropertyHint;

    private List<CardView> _cardViews = new List<CardView>();
    private Dictionary<Guid, CardView> _cardViewDict = new Dictionary<Guid, CardView>();
    private IGameplayStatusWatcher _statusWatcher;
    private IGameplayActionReciever _reciever;
    private CardCollectionInfo _cardCollectionInfo;

    // Dragging
    private bool _isDragging = false;
    private Vector2 _beginDragPosition;
    private Vector2 _dragOffset;

    public IEnumerable<ISelectableView> SelectableViews => _cardViews;

    public void Init(IGameplayStatusWatcher statusWatcher, IGameplayActionReciever reciever)
    {
        _statusWatcher = statusWatcher;
        _reciever = reciever;
    }

    public void CreateCardView(DrawCardEvent drawCardEvent)
    {
        _cardCollectionInfo = drawCardEvent.HandCardInfo;
        var cardView = _cardViewFactory.CreatePrefab();
        cardView.transform.SetParent(_cardViewParent, false);
        cardView.SetCardInfo(drawCardEvent.NewCardInfo);
        _cardViews.Add(cardView);
        _cardViewDict.Add(drawCardEvent.NewCardInfo.Indentity, cardView);

        _RearrangeCardViews();
    }

    public void UseCard(Guid cardIdentity)
    {
        _reciever.RecieveEvent(new UseCardAction{ CardIndentity = cardIdentity });
    }   
    public void FocusStart(Guid focusIdentity)
    {
        if (!_isDragging &&
            _cardViewDict.TryGetValue(focusIdentity, out var focusView))
        {
            var focusKvp = _cardCollectionInfo.CardInfos
                .FirstOrDefault(kvp => kvp.Key.Indentity == focusIdentity);
            var focusCardInfo = focusKvp.Key;
            var focusIndex = focusKvp.Value;
            focusView.ShowHandCardFocusContent();
            
            var smaller = _cardCollectionInfo.CardInfos
                .Where(kvp => kvp.Value < focusIndex)
                .Select(kvp => kvp.Key.Indentity);
            var larger = _cardCollectionInfo.CardInfos
                .Where(kvp => kvp.Value > focusIndex)
                .Select(kvp => kvp.Key.Indentity);
            
            foreach(var identity in smaller)
            {
                if(_cardViewDict.TryGetValue(identity, out var cardView))
                {
                    cardView.AddLocationOffset(focusIdentity, new Vector3(_focusOffsetX, 0, 0));
                }
            }
            foreach(var identity in larger)
            {
                if(_cardViewDict.TryGetValue(identity, out var cardView))
                {
                    cardView.AddLocationOffset(focusIdentity, new Vector3(-_focusOffsetX, 0, 0));
                }
            }
 
            _cardPropertyHint.ShowHint(focusCardInfo, focusView, smaller.Count() < larger.Count());
        }
    }
    public void FocusStop(Guid focusIdentity)
    {
        if (!_isDragging &&
            _cardViewDict.TryGetValue(focusIdentity, out var focusView))
        {
            var originSiblingIndex = _cardCollectionInfo.CardInfos
                .FirstOrDefault(kvp => kvp.Key.Indentity == focusIdentity).Value;
            focusView.HideHandCardFocusContent(originSiblingIndex);
            
            foreach(var cardView in _cardViews)
            {
                cardView.RemoveLocationOffset(focusIdentity);
            }

            _cardPropertyHint.HideHint();
        }
    }

    public void BeginDrag(Guid dragIdentity, PointerEventData pointerEventData)
    {
        if (!_isDragging &&
            _cardViewDict.TryGetValue(dragIdentity, out var dragView))
        {
            _isDragging = true;

            // Clear focus content
            var originSiblingIndex = _cardCollectionInfo.CardInfos
                .FirstOrDefault(kvp => kvp.Key.Indentity == dragIdentity).Value;
            dragView.HideHandCardFocusContent(originSiblingIndex);            
            foreach(var cardView in _cardViews.Where(view => view != dragView))
            {
                cardView.RemoveLocationOffset(dragIdentity);
            }
            _cardPropertyHint.HideHint();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform, pointerEventData.position, _canvas.worldCamera, out Vector2 localPoint);    
            _beginDragPosition = dragView.transform.GetComponent<RectTransform>().anchoredPosition;
            _dragOffset = _beginDragPosition - localPoint;
            dragView.BeginDrag(_beginDragPosition);
        }
    }
    public void Drag(Guid dragIdentity, PointerEventData pointerEventData)
    {
        if (_isDragging &&
            _cardViewDict.TryGetValue(dragIdentity, out var dragView))
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform, pointerEventData.position, _canvas.worldCamera, out Vector2 localPoint);
            var localDragPoint = localPoint + _dragOffset;

            var selectView = _reciever.SelectableViews
                .Where(view => RectTransformUtility.RectangleContainsScreenPoint(view.RectTransform, pointerEventData.position, _canvas.worldCamera))
                .FirstOrDefault();
            if (selectView != null && selectView != dragView as ISelectableView)
            {
                _lineRenderer.gameObject.SetActive(true);
                var points = _GenerateCurvePoints(_beginDragPosition, localDragPoint);
                _lineRenderer.positionCount = points.Length;
                _lineRenderer.SetPositions(points);
                dragView.Drag(localDragPoint, true);  
            }
            else
            {
                _lineRenderer.gameObject.SetActive(false);
                dragView.Drag(localDragPoint, false);  
            }      
        }
    }
    public void EndDrag(Guid dragIdentity, PointerEventData pointerEventData)
    {
        if (_isDragging &&
            _cardViewDict.TryGetValue(dragIdentity, out var dragView))
        {
            _isDragging = false;
            _lineRenderer.gameObject.SetActive(false);

            dragView.EndDrag(_beginDragPosition);

            // TODO: append select targets
            _reciever.RecieveEvent(new UseCardAction{ CardIndentity = dragIdentity });
        }
    }

    public void RemoveCardView(UsedCardEvent usedCardEvent)
    {
        _cardCollectionInfo = usedCardEvent.HandCardInfo;
        if(_cardViewDict.TryGetValue(usedCardEvent.UsedCardInfo.Indentity, out var cardView))
        {
            _cardViews.Remove(cardView);
            _cardViewDict.Remove(usedCardEvent.UsedCardInfo.Indentity);
            _cardViewFactory.RecyclePrefab(cardView);

            foreach(var view in _cardViews)
                view.RemoveLocationOffset(usedCardEvent.UsedCardInfo.Indentity);
            _RearrangeCardViews();
        }
    }

    public void RecycleHandCards(RecycleHandCardEvent recycleHandCardEvent)
    {
        foreach(var cardInfo in recycleHandCardEvent.RecycledCardInfos.Concat(recycleHandCardEvent.ExcludedCardInfos))
        {
            if(_cardViewDict.TryGetValue(cardInfo.Indentity, out var cardView))
            {
                _cardViews.Remove(cardView);
                _cardViewDict.Remove(cardInfo.Indentity);
                _cardViewFactory.RecyclePrefab(cardView);
                foreach(var view in _cardViews)
                    view.RemoveLocationOffset(cardInfo.Indentity);
            }
        }

        _RearrangeCardViews();
    }

    public void EnableHandCardsUseCardAction(PlayerExecuteStartEvent playerExecuteStartEvent)
    {
        foreach(var cardInfo in playerExecuteStartEvent.HandCardInfo.CardInfos.Keys)
        {
            if(_cardViewDict.TryGetValue(cardInfo.Indentity, out var cardView))
            {
                cardView.EnableHandCardAction(cardInfo, this);
            }
        }
    }
    public void DisableHandCardsUseCardAction(PlayerExecuteEndEvent playerExecuteEndEvent)
    {
        foreach(var cardInfo in playerExecuteEndEvent.HandCardInfo.CardInfos.Keys)
        {
            if(_cardViewDict.TryGetValue(cardInfo.Indentity, out var cardView))
            {
                cardView.DisableCardAction();
            }
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
            float angle = 90+ (i - centerIndex) * angleStep; 

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

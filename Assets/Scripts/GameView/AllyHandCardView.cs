using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IHandCardViewHandler
{
    void UseCard(Guid cardIdentity);
    void FocusStart(Guid cardIdentity);
    void FocusStop(Guid cardIdentity);
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
    
    [SerializeField]
    private CardPropertyHint _cardPropertyHint;
    [SerializeField]
    private LineRenderer _lineRenderer;

    private List<CardView> _cardViews = new List<CardView>();
    private Dictionary<Guid, CardView> _cardViewDict = new Dictionary<Guid, CardView>();
    private IGameplayStatusWatcher _statusWatcher;
    private IGameplayActionReciever _reciever;
    private CardCollectionInfo _cardCollectionInfo;

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
        if (_cardViewDict.TryGetValue(focusIdentity, out var focusView))
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
        if (_cardViewDict.TryGetValue(focusIdentity, out var focusView))
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
}

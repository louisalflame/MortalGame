using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AllyHandCardView : MonoBehaviour
{
    [SerializeField]
    private CardViewFactory _cardViewFactory;
    [SerializeField]
    private Transform _cardViewParent;
    [SerializeField]
    private float _arcAngle;
    [SerializeField]
    private float _arcRadiusX;
    [SerializeField]
    private float _arcRadiusY;
    [SerializeField]
    private float _arcStepMinAngle;
    [SerializeField]
    private float _focusOffsetX;

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
        var cardView = _cardViewFactory.CreateCardView();
        cardView.transform.SetParent(_cardViewParent);
        cardView.SetCardInfo(drawCardEvent.NewCardInfo);

        cardView.OnFocusStart += _FocusStart;
        cardView.OnFocusStop += _FocusStop;

        _cardViews.Add(cardView);
        _cardViewDict.Add(drawCardEvent.NewCardInfo.Indentity, cardView);

        _RearrangeCardViews();
    }

    private void _FocusStart(CardView focusView)
    {
        var x = _cardViewDict.First(kvp => kvp.Value == focusView);
        if(x.Value != null)
        { 
            focusView.transform.SetAsLastSibling();
            var focusIdentity = x.Key;
            var focusIndex = _cardCollectionInfo.CardInfos
                .First(kvp => kvp.Key.Indentity == focusIdentity).Value;
            
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
        }
    }
    private void _FocusStop(CardView focusView)
    {
        var x = _cardViewDict.First(kvp => kvp.Value == focusView);
        if(x.Value != null)
        { 
            var focusIdentity = x.Key;
            var focusIndex = _cardCollectionInfo.CardInfos
                .First(kvp => kvp.Key.Indentity == focusIdentity).Value;
            focusView.transform.SetSiblingIndex(focusIndex);
            
            foreach(var cardView in _cardViews)
            {
                cardView.RemoveLocationOffset(focusIdentity);
            }
        }
    }

    public void RemoveCardView(UsedCardEvent usedCardEvent)
    {
        _cardCollectionInfo = usedCardEvent.HandCardInfo;
        if(_cardViewDict.TryGetValue(usedCardEvent.UsedCardInfo.Indentity, out var cardView))
        {
            _cardViews.Remove(cardView);
            _cardViewDict.Remove(usedCardEvent.UsedCardInfo.Indentity);
            _cardViewFactory.RecycleCardView(cardView);

            foreach(var view in _cardViews)
                view.RemoveLocationOffset(usedCardEvent.UsedCardInfo.Indentity);
            _RearrangeCardViews();
        }
    }

    public void RecycleHandCards(RecycleHandCardEvent recycleHandCardEvent)
    {
        foreach(var cardInfo in recycleHandCardEvent.RecycledCardInfos)
        {
            if(_cardViewDict.TryGetValue(cardInfo.Indentity, out var cardView))
            {
                _cardViews.Remove(cardView);
                _cardViewDict.Remove(cardInfo.Indentity);
                _cardViewFactory.RecycleCardView(cardView);
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
                cardView.EnableUseCardAction(cardInfo, _reciever);
            }
        }
    }
    public void DisableHandCardsUseCardAction(PlayerExecuteEndEvent playerExecuteEndEvent)
    {
        foreach(var cardInfo in playerExecuteEndEvent.HandCardInfo.CardInfos.Keys)
        {
            if(_cardViewDict.TryGetValue(cardInfo.Indentity, out var cardView))
            {
                cardView.DisableUseCardAction();
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

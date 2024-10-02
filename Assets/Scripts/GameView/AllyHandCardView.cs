using System;
using System.Collections.Generic;
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

    private List<CardView> _cardViews = new List<CardView>();
    private Dictionary<Guid, CardView> _cardViewDict = new Dictionary<Guid, CardView>();
    private IGameplayStatusWatcher _statusWatcher;
    private IGameplayActionReciever _reciever;

    public void Init(IGameplayStatusWatcher statusWatcher, IGameplayActionReciever reciever)
    {
        _statusWatcher = statusWatcher;
        _reciever = reciever;
    }

    public void CreateCardView(DrawCardEvent drawCardEvent)
    {
        var cardView = _cardViewFactory.CreateCardView();
        cardView.transform.SetParent(_cardViewParent);
        cardView.SetCardInfo(drawCardEvent.NewCardInfo);

        _cardViews.Add(cardView);
        _cardViewDict.Add(drawCardEvent.NewCardInfo.Indentity, cardView);

        if(_cardViews.Count > 0)
        {
            _RearrangeCardViews();
        }
    }

    public void RemoveCardView(UsedCardEvent usedCardEvent)
    {
        if(_cardViewDict.TryGetValue(usedCardEvent.UsedCardInfo.Indentity, out var cardView))
        {
            _cardViews.Remove(cardView);
            _cardViewDict.Remove(usedCardEvent.UsedCardInfo.Indentity);
            _cardViewFactory.RecycleCardView(cardView);

            if(_cardViews.Count > 0)
            {
                _RearrangeCardViews();
            }
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
            }
        }

        if(_cardViews.Count > 0)
        {
            _RearrangeCardViews();
        }
    }

    public void EnableHandCardsUseCardAction(PlayerExecuteStartEvent playerExecuteStartEvent)
    {
        foreach(var cardInfo in playerExecuteStartEvent.HandCardInfos)
        {
            if(_cardViewDict.TryGetValue(cardInfo.Indentity, out var cardView))
            {
                cardView.EnableUseCardAction(cardInfo, _reciever);
            }
        }
    }
    public void DisableHandCardsUseCardAction(PlayerExecuteEndEvent playerExecuteEndEvent)
    {
        foreach(var cardInfo in playerExecuteEndEvent.HandCardInfos)
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
            cardView.transform.localPosition = new Vector3(x, y, 0);
            cardView.transform.localRotation = Quaternion.Euler(0, 0, angle - 90);
        }
    }
}

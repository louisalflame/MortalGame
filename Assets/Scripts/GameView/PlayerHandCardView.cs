using System.Collections.Generic;
using UnityEngine;

public class PlayerHandCardView : MonoBehaviour
{
    [SerializeField]
    private CardViewFactory _cardViewFactory;
    [SerializeField]
    private Transform _cardViewParent;
    [SerializeField]
    private RectTransform _handCardArea;
    [SerializeField]
    private AnimationCurve _positionCurve;
    [SerializeField]
    private AnimationCurve _rotationCurve;

    
    [SerializeField]
    private float _cardWidth = 100f;
    [SerializeField]
    private float _widthInterval = 20f;

    private List<CardView> _cardViews = new List<CardView>();
    private IGameplayStatusWatcher _statusWatcher;

    public void Init(IGameplayStatusWatcher statusWatcher)
    {
        _statusWatcher = statusWatcher;
    }

    public void CreateCardView(DrawCardEvent drawCardEvent, IGameplayActionReciever reciever)
    {
        foreach(var cardInfo in drawCardEvent.CardInfos)
        {
            var cardView = _cardViewFactory.CreateCardView();
            cardView.transform.SetParent(_cardViewParent);
            cardView.SetCardInfo(cardInfo, reciever);
            _cardViews.Add(cardView);
        }

        if(_cardViews.Count > 0)
        {
            _RearrangeCardViews();
        }
    }

    private void _RearrangeCardViews()
    {
        var widthInterval = _handCardArea.rect.width / _cardViews.Count; 
        widthInterval = widthInterval > (_cardWidth + _widthInterval) ? (_cardWidth + _widthInterval) : widthInterval;

        for (var i = 0; i < _cardViews.Count; i++)
        {
            var cardView = _cardViews[i];
            var x = widthInterval * i - _handCardArea.rect.width / 2 + widthInterval / 2;
            cardView.transform.localPosition = new Vector3(x, 0, 0);
        }
    }
}

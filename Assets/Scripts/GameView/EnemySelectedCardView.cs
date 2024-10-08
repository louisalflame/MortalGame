using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemySelectedCardView : MonoBehaviour
{
    [SerializeField]
    private AiCardViewFactory _cardViewFactory;
    [SerializeField]
    private Transform _cardViewParent;
    [SerializeField]
    private RectTransform _handCardArea;

    [SerializeField]
    private TextMeshProUGUI _deckCountText;
    
    [SerializeField]
    private float _cardWidth = 100f;
    [SerializeField]
    private float _widthInterval = 20f;

    private List<AiCardView> _cardViews = new List<AiCardView>();
    private Dictionary<Guid, AiCardView> _cardViewDict = new Dictionary<Guid, AiCardView>();

    private IGameplayStatusWatcher _statusWatcher;
    private IGameplayActionReciever _reciever;

    public void Init(IGameplayStatusWatcher statusWatcher, IGameplayActionReciever reciever)
    {
        _statusWatcher = statusWatcher;
        _reciever = reciever;
    }

    public void UpdateDeckView(DrawCardEvent drawCardEvent)
    {
        _deckCountText.text = drawCardEvent.DeckInfo.Count.ToString();
    }
    public void UpdateDeckView(RecycleGraveyardEvent recycleGraveyardEvent)
    {
        _deckCountText.text = recycleGraveyardEvent.DeckInfo.Count.ToString();
    }
 
    public void CreateCardView(EnemySelectCardEvent enemySelectCardEvent)
    {
        var cardView = _cardViewFactory.CreatePrefab();
        cardView.transform.SetParent(_cardViewParent);
        cardView.SetCardInfo(enemySelectCardEvent.SelectedCardInfo, _reciever);

        _cardViews.Add(cardView);
        _cardViewDict.Add(enemySelectCardEvent.SelectedCardInfo.Indentity, cardView);

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
            _cardViewFactory.RecyclePrefab(cardView);

            _RearrangeCardViews();
        }
    }
    public void RemoveCardView(EnemyUnselectedCardEvent enemyUnselectedCardEvent)
    {
        if(_cardViewDict.TryGetValue(enemyUnselectedCardEvent.SelectedCardInfo.Indentity, out var cardView))
        {
            _cardViews.Remove(cardView);
            _cardViewDict.Remove(enemyUnselectedCardEvent.SelectedCardInfo.Indentity);
            _cardViewFactory.RecyclePrefab(cardView);

            _RearrangeCardViews();
        }
    }
    public void RemoveCardView(RecycleHandCardEvent recycleHandCardEvent)
    {
        foreach(var cardInfo in recycleHandCardEvent.RecycledCardInfos)
        {
            if(_cardViewDict.TryGetValue(cardInfo.Indentity, out var cardView))
            {
                _cardViews.Remove(cardView);
                _cardViewDict.Remove(cardInfo.Indentity);
                _cardViewFactory.RecyclePrefab(cardView);
            }
        }

        _RearrangeCardViews();
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

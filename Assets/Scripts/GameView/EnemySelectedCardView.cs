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
    
    public IEnumerable<ISelectableView> SelectableViews => _cardViews;

    private List<IAiCardView> _cardViews = new List<IAiCardView>();
    private Dictionary<Guid, IAiCardView> _cardViewDict = new Dictionary<Guid, IAiCardView>();

    private IGameplayStatusWatcher _statusWatcher;
    private IGameplayActionReciever _reciever;
    private LocalizeLibrary _localizeLibrary;

    public void Init(IGameplayStatusWatcher statusWatcher, IGameplayActionReciever reciever, LocalizeLibrary localizeLibrary)
    {
        _statusWatcher = statusWatcher;
        _reciever = reciever;
        _localizeLibrary = localizeLibrary;
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
        cardView.transform.SetParent(_cardViewParent, false);
        cardView.SetCardInfo(enemySelectCardEvent.SelectedCardInfo, _localizeLibrary);

        _cardViews.Add(cardView);
        _cardViewDict.Add(enemySelectCardEvent.SelectedCardInfo.Identity, cardView);
        if(_cardViews.Count > 0)
        {
            _RearrangeCardViews();
        }
    }
    public void CreateCardView(CloneCardEvent cloneCardEvent)
    {
        var cardView = _cardViewFactory.CreatePrefab();
        cardView.transform.SetParent(_cardViewParent, false);
        cardView.SetCardInfo(cloneCardEvent.CardInfo, _localizeLibrary);

        _cardViews.Add(cardView);
        _cardViewDict.Add(cloneCardEvent.CardInfo.Identity, cardView);
        if(_cardViews.Count > 0)
        {
            _RearrangeCardViews();
        }
    }

    public void RemoveCardView(UsedCardEvent usedCardEvent)
    {
        if(_cardViewDict.TryGetValue(usedCardEvent.UsedCardInfo.Identity, out var cardView))
        {
            _cardViews.Remove(cardView);
            _cardViewDict.Remove(usedCardEvent.UsedCardInfo.Identity);
            _cardViewFactory.RecyclePrefab(cardView as AiCardView);

            _RearrangeCardViews();
        }
    }
    public void RemoveCardView(DiscardCardEvent discardCardEvent)
    {
        if(_cardViewDict.TryGetValue(discardCardEvent.CardInfo.Identity, out var cardView))
        {
            _cardViews.Remove(cardView);
            _cardViewDict.Remove(discardCardEvent.CardInfo.Identity);
            _cardViewFactory.RecyclePrefab(cardView as AiCardView);

            _RearrangeCardViews();
        }
    }
    public void RemoveCardView(ConsumeCardEvent consumeCardEvent)
    {
        if(_cardViewDict.TryGetValue(consumeCardEvent.CardInfo.Identity, out var cardView))
        {
            _cardViews.Remove(cardView);
            _cardViewDict.Remove(consumeCardEvent.CardInfo.Identity);
            _cardViewFactory.RecyclePrefab(cardView as AiCardView);

            _RearrangeCardViews();
        }
    }
    public void RemoveCardView(DisposeCardEvent disposeCardEvent)
    {
        if(_cardViewDict.TryGetValue(disposeCardEvent.CardInfo.Identity, out var cardView))
        {
            _cardViews.Remove(cardView);
            _cardViewDict.Remove(disposeCardEvent.CardInfo.Identity);
            _cardViewFactory.RecyclePrefab(cardView as AiCardView);

            _RearrangeCardViews();
        }
    }

    public void RemoveCardView(EnemyUnselectedCardEvent enemyUnselectedCardEvent)
    {
        if(_cardViewDict.TryGetValue(enemyUnselectedCardEvent.SelectedCardInfo.Identity, out var cardView))
        {
            _cardViews.Remove(cardView);
            _cardViewDict.Remove(enemyUnselectedCardEvent.SelectedCardInfo.Identity);
            _cardViewFactory.RecyclePrefab(cardView as AiCardView);

            _RearrangeCardViews();
        }
    }
    public void RemoveCardView(RecycleHandCardEvent recycleHandCardEvent)
    {
        foreach(var cardInfo in recycleHandCardEvent.RecycledCardInfos)
        {
            if(_cardViewDict.TryGetValue(cardInfo.Identity, out var cardView))
            {
                _cardViews.Remove(cardView);
                _cardViewDict.Remove(cardInfo.Identity);
                _cardViewFactory.RecyclePrefab(cardView as AiCardView);
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
            cardView.SetPositionAndRotation( new Vector3(x, 0, 0), Quaternion.identity );
        }
    }
}

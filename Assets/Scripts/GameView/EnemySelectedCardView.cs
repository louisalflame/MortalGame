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
    private Dictionary<int, AiCardView> _cardViewDict = new Dictionary<int, AiCardView>();

    private IGameplayStatusWatcher _statusWatcher;
    private IGameplayActionReciever _reciever;

    public void Init(IGameplayStatusWatcher statusWatcher, IGameplayActionReciever reciever)
    {
        _statusWatcher = statusWatcher;
        _reciever = reciever;
    }

    public void UpdateDeckView(DrawCardEvent drawCardEvent)
    {
        _deckCountText.text = drawCardEvent.DeckCardInfos.Count.ToString();
    }

    public void CreateCardView(EnemySelectCardEvent enemySelectCardEvent)
    {
        var cardView = _cardViewFactory.CreateCardView();
        cardView.transform.SetParent(_cardViewParent);
        cardView.SetCardInfo(enemySelectCardEvent.SelectedCardInfo, _reciever);

        _cardViews.Add(cardView);
        _cardViewDict.Add(enemySelectCardEvent.SelectedCardInfo.CardIndentity, cardView);

        if(_cardViews.Count > 0)
        {
            _RearrangeCardViews();
        }
    }

    public void RemoveCardView(UsedCardEvent usedCardEvent)
    {
        if(_cardViewDict.TryGetValue(usedCardEvent.UsedCardInfo.CardIndentity, out var cardView))
        {
            _cardViews.Remove(cardView);
            _cardViewDict.Remove(usedCardEvent.UsedCardInfo.CardIndentity);
            _cardViewFactory.RecycleCardView(cardView);

            if(_cardViews.Count > 0)
            {
                _RearrangeCardViews();
            }
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

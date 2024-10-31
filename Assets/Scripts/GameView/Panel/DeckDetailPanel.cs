using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public interface ISimpleCardViewHandler
{
    void ReadCard(Guid cardIdentity);
}

public enum DeckDetailPanelState
{
    Idle = 0,
    SinglePopup,
    Close,
}

public class DeckDetailPanel : MonoBehaviour, ISimpleCardViewHandler
{
    [SerializeField]
    private Button[] _closeButtons;
    [SerializeField]
    private GameObject _panel;
    [SerializeField]
    private Transform _cardViewParent;
    [SerializeField]
    private CardViewFactory _cardViewFactory;
    [SerializeField]
    private SingleCardDetailPopupPanel _singleCardDetailPopupPanel;

    private DeckDetailPanelState _state;
    private CardInfo _selectedCardInfo;
    private IGameplayStatusWatcher _statusWatcher;
    private Dictionary<Guid, CardView> _cardViewDict = new Dictionary<Guid, CardView>();

    public void Init(IGameplayStatusWatcher statusWatcher)
    {
        _statusWatcher = statusWatcher;
    }

    public async UniTask Run()
    {
        var disposables = new CompositeDisposable();
        foreach (var button in _closeButtons)
        {
            button.OnClickAsObservable()
                .Subscribe(_ => _state = DeckDetailPanelState.Close)
                .AddTo(disposables);
        }

        var cardInfos = _statusWatcher.GameStatus.Ally.CardManager.Deck.Cards
            .ToCardInfos(_statusWatcher.GameContext);
        foreach (var cardInfo in cardInfos)
        {
            var cardView = _cardViewFactory.CreatePrefab();
            cardView.transform.SetParent(_cardViewParent, false);
            cardView.SetCardInfo(cardInfo);
            cardView.EnableSimpleCardAction(cardInfo, this);
            _cardViewDict.Add(cardInfo.Indentity, cardView);
        }
        
        using (disposables)
        {
            _state = DeckDetailPanelState.Idle;
            _panel.SetActive(true);

            while (_state != DeckDetailPanelState.Close)
            {
                switch (_state)
                {
                    case DeckDetailPanelState.Idle:
                        await UniTask.NextFrame();
                        break;
                    case DeckDetailPanelState.SinglePopup:
                        await _singleCardDetailPopupPanel.Run(_selectedCardInfo);
                        _state = DeckDetailPanelState.Idle;
                        _selectedCardInfo = null;
                        break;
                }
            }

            _panel.SetActive(false);
        }

        foreach (var cardView in _cardViewDict.Values)
        {
            cardView.DisableCardAction();
            _cardViewFactory.RecyclePrefab(cardView);
        }
        _cardViewDict.Clear();
    }

    public void ReadCard(Guid cardIdentity)
    {
        _selectedCardInfo = _statusWatcher.GameStatus.Ally.CardManager.Deck.Cards
            .Where(card => card.Indentity == cardIdentity)
            .Select(card => new CardInfo(card, _statusWatcher.GameContext))
            .FirstOrDefault();
        if (_selectedCardInfo != null)
            _state = DeckDetailPanelState.SinglePopup;
    }
}

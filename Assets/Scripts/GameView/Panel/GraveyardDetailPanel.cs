using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public enum GraveyardDetailPanelState
{
    Idle = 0,
    SinglePopup,
    Close,
}

public class GraveyardDetailPanel : MonoBehaviour, ISimpleCardViewHandler
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

    private GraveyardDetailPanelState _state;
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
                .Subscribe(_ => _state = GraveyardDetailPanelState.Close)
                .AddTo(disposables);
        }

        var cardInfos = _statusWatcher.GameStatus.Ally.CardManager.Graveyard.CardCollectionInfo.CardInfos.Keys;
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
            _state = GraveyardDetailPanelState.Idle;
            _panel.SetActive(true);

            while (_state != GraveyardDetailPanelState.Close)
            {
                switch (_state)
                {
                    case GraveyardDetailPanelState.Idle:
                        await UniTask.NextFrame();
                        break;
                    case GraveyardDetailPanelState.SinglePopup:
                        await _singleCardDetailPopupPanel.Run(_selectedCardInfo);
                        _state = GraveyardDetailPanelState.Idle;
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
        _selectedCardInfo = _statusWatcher.GameStatus.Ally.CardManager.Graveyard
            .CardCollectionInfo.CardInfos.Keys
            .FirstOrDefault(kvp => kvp.Indentity == cardIdentity);
        if (_selectedCardInfo != null)
            _state = GraveyardDetailPanelState.SinglePopup;
    }
}

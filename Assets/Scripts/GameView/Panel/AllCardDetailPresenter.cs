using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public enum AllCardDetailPanelState
{
    ShowDeck,
    ShowGraveyard,
    ShowHandCard,
    Idle,
    Close,
    SinglePopup,
}

public interface IAllCardViewHandler
{
    void ReadCard(Guid cardIdentity);
    void ShowDeckDetail();
    void ShowGraveyardDetail();
    void ShowHandCardDetail();
    void Close();
}

public interface IAllCardDetailPresenter : IAllCardViewHandler
{
    UniTask Run();
}

public class AllCardDetailPresenter : IAllCardDetailPresenter
{
    private IAllCardDetailPanel _detailPanel;
    private SingleCardDetailPopupPanel _singlePopupPanel;
    private IGameplayStatusWatcher _statusWatcher;
    private AllCardDetailPanelState _state;

    private CardInfo _selectedCardInfo;
    private IReadOnlyCollection<CardInfo> _cardInfos;

    public AllCardDetailPresenter(IAllCardDetailPanelView view, IGameplayStatusWatcher statusWatcher)
    {
        _detailPanel = view.DetailPanel;
        _singlePopupPanel = view.SinglePopupPanel;
        _statusWatcher = statusWatcher;

        _detailPanel.Init(this);

        _state = AllCardDetailPanelState.Close;
    }

    public async UniTask Run()
    {
        while(true)
        {
            switch (_state)
            {
                case AllCardDetailPanelState.ShowDeck:
                    _state = AllCardDetailPanelState.Idle;
                    break;
                case AllCardDetailPanelState.ShowGraveyard:
                    _state = AllCardDetailPanelState.Idle;
                    break;
                case AllCardDetailPanelState.ShowHandCard:
                    _state = AllCardDetailPanelState.Idle;
                    break;
                case AllCardDetailPanelState.SinglePopup:
                    await _singlePopupPanel.Run(_selectedCardInfo);
                    _state = AllCardDetailPanelState.Idle;
                    _selectedCardInfo = null;
                    break;
                case AllCardDetailPanelState.Idle:
                case AllCardDetailPanelState.Close:
                    await UniTask.Yield();
                    break;
            }
        }
    }

    public void ShowDeckDetail()
    {
        if (_state == AllCardDetailPanelState.Close)
            _detailPanel.Open();
        _state = AllCardDetailPanelState.ShowDeck;

        _cardInfos = _statusWatcher.GameStatus.Ally.CardManager.Deck.Cards
            .ToCardInfos(_statusWatcher.GameContext);
        _detailPanel.ShowCardInfoCollections(this, _cardInfos);
    }
    public void ShowGraveyardDetail()
    {
        if (_state == AllCardDetailPanelState.Close)
            _detailPanel.Open();
        _state = AllCardDetailPanelState.ShowGraveyard;

        _cardInfos = _statusWatcher.GameStatus.Ally.CardManager.Graveyard.Cards
            .ToCardInfos(_statusWatcher.GameContext);
        _detailPanel.ShowCardInfoCollections(this, _cardInfos);
    }
    public void ShowHandCardDetail()
    {
        if (_state == AllCardDetailPanelState.Close)
            _detailPanel.Open();
        _state = AllCardDetailPanelState.ShowHandCard;

        _cardInfos = _statusWatcher.GameStatus.Ally.CardManager.HandCard.Cards
            .ToCardInfos(_statusWatcher.GameContext);
        _detailPanel.ShowCardInfoCollections(this, _cardInfos);
    }
    public void Close()
    {
        _detailPanel.Close();
        _state = AllCardDetailPanelState.Close;
    }

    public void ReadCard(Guid cardIdentity)
    {
        if(_state == AllCardDetailPanelState.Idle)
        {
            _selectedCardInfo = _cardInfos.FirstOrDefault(x => x.Indentity == cardIdentity);
            _state = AllCardDetailPanelState.SinglePopup;
        }
    }
}

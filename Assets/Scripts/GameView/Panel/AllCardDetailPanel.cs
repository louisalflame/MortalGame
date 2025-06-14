using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public interface IAllCardDetailPanel
{
    void Init(IAllCardViewHandler handler, IGameInfoModel gameInfoModel, LocalizeLibrary localizeLibrary);
    void ShowCardInfoCollections(IAllCardViewHandler handler, IEnumerable<CardInfo> cardInfos);
    void Open();
    void Close();
}

public class AllCardDetailPanel : MonoBehaviour, IAllCardDetailPanel
{
    [SerializeField]
    private Button[] _closeButtons;
    [SerializeField]
    private Button _deckButton;
    [SerializeField]
    private Button _handCardButton;
    [SerializeField]
    private Button _graveyardButton;
    [SerializeField]
    private GameObject _panel;
    [SerializeField]
    private Transform _cardViewParent;
    [SerializeField]
    private CardViewFactory _cardViewFactory;

    private CompositeDisposable _disposables = new CompositeDisposable();
    private CardInfo _selectedCardInfo;
    private IGameInfoModel _gameInfoModel;
    private LocalizeLibrary _localizeLibrary;
    private Dictionary<Guid, CardView> _cardViewDict = new Dictionary<Guid, CardView>();

    public void Init(
        IAllCardViewHandler handler,
        IGameInfoModel gameInfoModel,
        LocalizeLibrary localizeLibrary)
    {
        _gameInfoModel = gameInfoModel;
        _localizeLibrary = localizeLibrary;
        foreach (var button in _closeButtons)
        {
            button.OnClickAsObservable()
                .Subscribe(_ => handler.Close())
                .AddTo(_disposables);
        }

        _deckButton.OnClickAsObservable()
            .Subscribe(_ => handler.ShowDeckDetail())
            .AddTo(_disposables);
        _handCardButton.OnClickAsObservable()
            .Subscribe(_ => handler.ShowHandCardDetail())
            .AddTo(_disposables);
        _graveyardButton.OnClickAsObservable()
            .Subscribe(_ => handler.ShowGraveyardDetail())
            .AddTo(_disposables);
    }

    public void ShowCardInfoCollections(IAllCardViewHandler handler, IEnumerable<CardInfo> cardInfos)
    {
        _Clear();
        foreach (var cardInfo in cardInfos)
        {
            var cardView = _cardViewFactory.CreatePrefab();
            cardView.transform.SetParent(_cardViewParent, false);
            cardView.Initialize(_gameInfoModel, _localizeLibrary);
            cardView.SetCardInfo(cardInfo);
            cardView.EnableSimpleCardAction(cardInfo, handler);
            _cardViewDict.Add(cardInfo.Identity, cardView);
        }
    }

    public void Open()
    {
        _panel.SetActive(true);
    }
    public void Close()
    {
        _Clear();
        _panel.SetActive(false);
    }

    private void _Clear()
    {
        foreach (var cardView in _cardViewDict.Values)
        {
            cardView.DisableCardAction();
            _cardViewFactory.RecyclePrefab(cardView);
        }
        _cardViewDict.Clear();
    }
}

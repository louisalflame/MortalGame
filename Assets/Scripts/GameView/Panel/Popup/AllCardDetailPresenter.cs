using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Optional;
using UniRx;

public interface IAllCardDetailPresenter
{
    UniTask Run(
        Faction faction,
        CardCollectionType type,
        CancellationToken cancellationToken);
}

public record CardDetailProperty(
    ICardView.CardSimpleProperty CardProperty,
    CardPropertyHint.ViewData CardBuffHint,
    CardPropertyHint.ViewData CardKeywordHint)
{
    public static CardDetailProperty Create(CardInfo cardInfo)
    {
        return new CardDetailProperty(
            CardProperty: new ICardView.CardSimpleProperty(cardInfo),
            CardBuffHint: new CardPropertyHint.ViewData(
                cardInfo.BuffInfos
                    .Select(buffInfo =>
                        new CardPropertyHint.InfoCellViewData(
                            LocalizeType.CardBuff,
                            buffInfo.CardBuffDataId,
                            buffInfo.GetTemplateValues()))
                    .ToArray()),
            CardKeywordHint: new CardPropertyHint.ViewData(
                cardInfo.Keywords
                    .Select(keyword =>
                        new CardPropertyHint.InfoCellViewData(
                            LocalizeType.KeyWord,
                            keyword,
                            Utility.Dictionary<string, string>.EMPTY))
                    .ToArray()));
    }
}

public class AllCardDetailPresenter : IAllCardDetailPresenter
{
    private readonly IAllCardDetailPanel _detailPanel;
    private readonly SingleCardDetailPopupPanel _singlePopupPanel;
    private readonly IGameViewModel _gameViewModel;

    private CardInfo _selectedCardInfo;
    private IUniTaskPresenter<Unit> _uniTaskPresenter;
    private bool _isClose;

    public AllCardDetailPresenter(
        IAllCardDetailPanelView panelView,
        IGameViewModel gameViewModel,
        LocalizeLibrary localizeLibrary)
    {
        _detailPanel = panelView.DetailPanel;
        _singlePopupPanel = panelView.SinglePopupPanel;
        _gameViewModel = gameViewModel;

        _detailPanel.Init(_gameViewModel, localizeLibrary);
        _uniTaskPresenter = new UniTaskPresenter<Unit>();
    }

    public async UniTask Run(
        Faction faction,
        CardCollectionType type,
        CancellationToken cancellationToken)
    {
        _isClose = false;
        _detailPanel.Open();

        var cardCollectionInfo = _gameViewModel.ObservableCardCollectionInfo(faction, type);

        await _uniTaskPresenter.Run(
            _SetupSubscriptions(faction),
            () => !_isClose,
            cancellationToken,
            _ShowCardCollectionInfos(cardCollectionInfo.Value));

        _detailPanel.Close();
    }

    private IDisposable _SetupSubscriptions(Faction faction)
    {
        var disposables = new CompositeDisposable();

        _detailPanel.DeckButton.OnClickAsObservable()
            .WithLatestFrom(
                _gameViewModel.ObservableCardCollectionInfo(faction, CardCollectionType.Deck),
                (_, deckInfo) => deckInfo)
            .Subscribe(deckInfo => _uniTaskPresenter.TryEnqueueTask(_ShowCardCollectionInfos(deckInfo)))
            .AddTo(disposables);
        _detailPanel.HandCardButton.OnClickAsObservable()
            .WithLatestFrom(
                _gameViewModel.ObservableCardCollectionInfo(faction, CardCollectionType.HandCard),
                (_, handCardsInfo) => handCardsInfo)
            .Subscribe(handCardsInfo => _uniTaskPresenter.TryEnqueueTask(_ShowCardCollectionInfos(handCardsInfo)))
            .AddTo(disposables);
        _detailPanel.GraveyardButton.OnClickAsObservable()
            .WithLatestFrom(
                _gameViewModel.ObservableCardCollectionInfo(faction, CardCollectionType.Graveyard),
                (_, graveyardInfo) => graveyardInfo)
            .Subscribe(graveyardInfo => _uniTaskPresenter.TryEnqueueTask(_ShowCardCollectionInfos(graveyardInfo)))
            .AddTo(disposables);
        _detailPanel.CloseButtons
            .Select(button => button.OnClickAsObservable())
            .Merge()
            .Subscribe(_ => _isClose = true)
            .AddTo(disposables);
        
        return disposables;
    }

    private UniTask _ShowCardCollectionInfos(CardCollectionInfo cardCollectionInfo)
    {
        var property = new IAllCardDetailPanel.Property(
            cardCollectionInfo.CardInfos.Keys.Select(cardInfo =>
                new ICardView.CardClickableProperty(
                    CardInfo: cardInfo,
                    IsClickable: true,
                    OnClickCard: (info, cardView) => _uniTaskPresenter.TryEnqueueTask(
                        _singlePopupPanel.Run(CardDetailProperty.Create(info)))))
            );
        _detailPanel.Render(property);

        return UniTask.CompletedTask;
    }
}

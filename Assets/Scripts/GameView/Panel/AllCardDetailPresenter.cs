using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Optional;
using UniRx;
using UnityEngine;

public interface IAllCardDetailPresenter
{
    UniTask Run(
        Faction faction,
        CardCollectionType type,
        CancellationToken cancellationToken);
}

public record CardDetailProperty(
    ICardView.CardDetailProperty CardProperty,
    CardPropertyHint.ViewData CardBuffHint,
    CardPropertyHint.ViewData CardKeywordHint)
{
    public static CardDetailProperty Create(CardInfo cardInfo)
    {
        return new CardDetailProperty(
            CardProperty: new ICardView.CardDetailProperty(cardInfo),
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
    private Option<UniTask> _currentTask = Option.None<UniTask>();

    public AllCardDetailPresenter(
        IAllCardDetailPanelView panelView,
        IGameViewModel gameViewModel,
        LocalizeLibrary localizeLibrary)
    {
        _detailPanel = panelView.DetailPanel;
        _singlePopupPanel = panelView.SinglePopupPanel;
        _gameViewModel = gameViewModel;

        _detailPanel.Init(_gameViewModel, localizeLibrary);
    }

    public async UniTask Run(
        Faction faction,
        CardCollectionType type,
        CancellationToken cancellationToken)
    {
        var disposables = new CompositeDisposable();
        var isClose = false;

        _detailPanel.Open();
        _detailPanel.DeckButton.OnClickAsObservable()
            .WithLatestFrom(
                _gameViewModel.ObservableCardCollectionInfo(faction, CardCollectionType.Deck),
                (_, deckInfo) => deckInfo)
            .Subscribe(deckInfo => _TryEnqueueTask(_ShowCardCollectionInfos(deckInfo)))
            .AddTo(disposables);
        _detailPanel.HandCardButton.OnClickAsObservable()
            .WithLatestFrom(
                _gameViewModel.ObservableCardCollectionInfo(faction, CardCollectionType.HandCard),
                (_, handCardsInfo) => handCardsInfo)
            .Subscribe(handCardsInfo => _TryEnqueueTask(_ShowCardCollectionInfos(handCardsInfo)))
            .AddTo(disposables);
        _detailPanel.GraveyardButton.OnClickAsObservable()
            .WithLatestFrom(
                _gameViewModel.ObservableCardCollectionInfo(faction, CardCollectionType.Graveyard),
                (_, graveyardInfo) => graveyardInfo)
            .Subscribe(graveyardInfo => _TryEnqueueTask(_ShowCardCollectionInfos(graveyardInfo)))
            .AddTo(disposables);
        _detailPanel.CloseButtons
            .Select(button => button.OnClickAsObservable())
            .Merge()
            .Subscribe(_ => isClose = true)
            .AddTo(disposables);

        var cardCollectionInfo = _gameViewModel.ObservableCardCollectionInfo(faction, type);
        _TryEnqueueTask(_ShowCardCollectionInfos(cardCollectionInfo.Value));

        while (!cancellationToken.IsCancellationRequested && !isClose)
        {
            if (_TryPopOutNextTask(out var task))
            {
                await task;
            }
            else
            {
                await UniTask.NextFrame();
            }
        }

        disposables.Dispose();
        _detailPanel.Close();
    }

    private bool _TryPopOutNextTask(out UniTask task)
    {
        if (_currentTask.HasValue)
        {
            task = _currentTask.ValueOr(UniTask.CompletedTask);
            _currentTask = Option.None<UniTask>();
            return true;
        }

        task = UniTask.CompletedTask;
        return false;
    }
    private void _TryEnqueueTask(UniTask task)
    {
        if (!_currentTask.HasValue)
        {
            _currentTask = Option.Some(task);
        }
    }

    private UniTask _ShowCardCollectionInfos(CardCollectionInfo cardCollectionInfo)
    {
        var property = new IAllCardDetailPanel.Property(
            cardCollectionInfo.CardInfos.Keys.Select(cardInfo =>
                new ICardView.CardClickableProperty(
                    CardInfo: cardInfo,
                    IsClickable: true,
                    OnClickCard: info => _TryEnqueueTask(
                        _singlePopupPanel.Run(CardDetailProperty.Create(info)))))
            );
        _detailPanel.Render(property);

        return UniTask.CompletedTask;
    }
}

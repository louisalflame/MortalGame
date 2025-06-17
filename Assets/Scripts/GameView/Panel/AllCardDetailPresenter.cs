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

public class AllCardDetailPresenter : IAllCardDetailPresenter
{
    private readonly IAllCardDetailPanel _detailPanel;
    private readonly SingleCardDetailPopupPanel _singlePopupPanel;
    private readonly IGameViewModel _gameViewModel;

    private CardInfo _selectedCardInfo;
    private CompositeDisposable _disposables = new CompositeDisposable();
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
        _detailPanel.Open();

        var disposables = new CompositeDisposable();
        var isClose = false;

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
        _disposables?.Dispose();
        _disposables = new CompositeDisposable();

        foreach (var kvp in _detailPanel.ShowCardInfoCollections(cardCollectionInfo.CardInfos.Keys))
        {
            var cardInfo = kvp.Key;
            var button = kvp.Value;
            button.interactable = true;
            button.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _selectedCardInfo = cardInfo;
                    _TryEnqueueTask(_singlePopupPanel.Run(cardInfo));
                })
                .AddTo(_disposables);
        }
        
        return UniTask.CompletedTask;
    }
}

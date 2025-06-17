using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Optional;
using UniRx;
using UnityEngine;
public interface IUIPresenter
{
    UniTask Run(CancellationToken cancellationToken);
}

public class UIPresenter : IUIPresenter
{
    private readonly DeckCardView _deckCardView;
    private readonly GraveyardCardView _graveyardCardView;
    private readonly IGameViewModel _gameViewModel;

    private IAllCardDetailPresenter _allCardDetailPresenter;

    private Option<UniTask> _currentTask = Option.None<UniTask>();

    public UIPresenter(
        IInteractionButtonView buttonView,
        IAllCardDetailPanelView panelView,
        IGameViewModel gameViewModel,
        LocalizeLibrary localizeLibrary)
    {
        _deckCardView = buttonView.DeckCardView;
        _graveyardCardView = buttonView.GraveyardCardView;
        _gameViewModel = gameViewModel;

        _allCardDetailPresenter = new AllCardDetailPresenter(panelView, gameViewModel, localizeLibrary);
    }

    public async UniTask Run(CancellationToken cancellationToken)
    {
        var disposables = new CompositeDisposable();
        _deckCardView.DeckButton.OnClickAsObservable()
            .Subscribe(deckInfo => _TryEnqueueTask(
                _allCardDetailPresenter.Run(Faction.Ally, CardCollectionType.Deck, CancellationToken.None)))
            .AddTo(disposables);
        _graveyardCardView.GraveyardButton.OnClickAsObservable()
            .Subscribe(deckInfo => _TryEnqueueTask(
                _allCardDetailPresenter.Run(Faction.Ally, CardCollectionType.Graveyard, CancellationToken.None)))
            .AddTo(disposables);

        while (!cancellationToken.IsCancellationRequested)
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
}

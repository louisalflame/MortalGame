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
    UniTask<bool> Run(CancellationToken cancellationToken);
}

public class UIPresenter : IUIPresenter
{
    private readonly DeckCardView _deckCardView;
    private readonly GraveyardCardView _graveyardCardView;
    private readonly EnemySelectedCardView _enemySelectedCardView;

    private IAllCardDetailPresenter _allCardDetailPresenter;

    private UniTaskPresenter<bool> _uniTaskPresenter;

    public UIPresenter(
        IInteractionButtonView buttonView,
        IAllCardDetailPanelView panelView,
        IGameViewModel gameViewModel,
        LocalizeLibrary localizeLibrary)
    {
        _deckCardView = buttonView.DeckCardView;
        _graveyardCardView = buttonView.GraveyardCardView;
        _enemySelectedCardView = buttonView.EnemySelectedCardView;

        _allCardDetailPresenter = new AllCardDetailPresenter(panelView, gameViewModel, localizeLibrary);        
        _uniTaskPresenter = new UniTaskPresenter<bool>();
    }

    public async UniTask<bool> Run(CancellationToken cancellationToken)
    {
        var result = await _uniTaskPresenter.Run(
            _SetupSubscriptions(),
            () => true,
            cancellationToken);
        return result.ValueOr(true);
    }

    private IDisposable _SetupSubscriptions()
    {
        var disposables = new CompositeDisposable();
        _deckCardView.DeckButton.OnClickAsObservable()
            .Subscribe(_ => _uniTaskPresenter.TryEnqueueTask(
                _allCardDetailPresenter.Run(Faction.Ally, CardCollectionType.Deck, CancellationToken.None)))
            .AddTo(disposables);                
        _graveyardCardView.GraveyardButton.OnClickAsObservable()
            .Subscribe(_ => _uniTaskPresenter.TryEnqueueTask(
                _allCardDetailPresenter.Run(Faction.Ally, CardCollectionType.Graveyard, CancellationToken.None)))
            .AddTo(disposables);
        _enemySelectedCardView.DeckButton.OnClickAsObservable()
            .Subscribe(_ => _uniTaskPresenter.TryEnqueueTask(
                _allCardDetailPresenter.Run(Faction.Enemy, CardCollectionType.Deck, CancellationToken.None)))
            .AddTo(disposables);
        return disposables;
    }
}

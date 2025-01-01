using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IGameplayActionReciever
{
    void RecieveEvent(IGameAction gameplayAction);
    void ShowDeckDetailPanel();
    void ShowGraveyardDetailPanel();

    IEnumerable<ISelectableView> SelectableViews { get; }
    ISelectableView BasicSelectableView { get; }
}

public class GameplayPresenter : IGameplayActionReciever
{
    private IGameplayView _gameplayView;
    private GameplayManager _gameplayManager;
    private IAllCardDetailPresenter _allCardDetailPresenter;

    public IEnumerable<ISelectableView> SelectableViews => _gameplayView.SelectableViews;
    public ISelectableView BasicSelectableView => _gameplayView.BasicSelectableView;

    public GameplayPresenter(
        GameplayView gameplayView,
        GameStatus gameStatus,
        GameContextManager gameContextManager
    )
    {
        _gameplayView = gameplayView;
        _gameplayManager = new GameplayManager(gameStatus, gameContextManager);

        _gameplayView.Init(this, _gameplayManager, gameContextManager.LocalizeLibrary, gameContextManager.DispositionLibrary);
        _allCardDetailPresenter = new AllCardDetailPresenter(_gameplayView, _gameplayManager, gameContextManager.LocalizeLibrary);
    }

    public async UniTask<GameResult> Run()
    {
        _gameplayManager.Start();
        _allCardDetailPresenter.Run();

        while(!_gameplayManager.IsEnd)
        {
            await UniTask.Yield();
            
            var events = _gameplayManager.PopAllEvents();
            _gameplayView.Render(events, this);
        }

        return _gameplayManager.GameResult;
    }

    public void RecieveEvent(IGameAction gameAction)
    {
        Debug.Log($"-- GameplayPresenter.RecieveEvent:[{gameAction}] --");
        _gameplayManager.EnqueueAction(gameAction);
    }

    public void ShowDeckDetailPanel()
    {
        _allCardDetailPresenter.ShowDeckDetail();
    }
    public void ShowGraveyardDetailPanel()
    {
        _allCardDetailPresenter.ShowGraveyardDetail();
    }
}

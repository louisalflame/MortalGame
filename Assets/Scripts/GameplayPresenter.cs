using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IGameplayActionReciever
{
    void RecieveEvent(IGameAction gameplayAction);

    IEnumerable<ISelectableView> SelectableViews { get; }
    ISelectableView BasicSelectableView { get; }
}

public class GameplayPresenter : IGameplayActionReciever
{
    private IGameplayView _gameplayView;
    private GameViewModel _gameInfoModel;
    private GameplayManager _gameplayManager;
    private IUIPresenter _uiPresenter;

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
        
        _gameInfoModel = new GameViewModel();
        _gameplayView.Init(_gameInfoModel, this, _gameplayManager, gameContextManager.LocalizeLibrary, gameContextManager.DispositionLibrary);
        _uiPresenter = new UIPresenter(_gameplayView, _gameplayView, _gameInfoModel, gameContextManager.LocalizeLibrary);
    }

    public async UniTask<GameResult> Run()
    {
        var cts = new CancellationTokenSource();
        
        var gameplayLoopTask = _RunGameplayLoop(cts);
        var cardPresenterTask = _uiPresenter.Run(cts.Token);

        await UniTask.WhenAll(gameplayLoopTask, cardPresenterTask);

        return _gameplayManager.GameResult;
    }

    private async UniTask _RunGameplayLoop(CancellationTokenSource cts)
    {
        _gameplayManager.Start();
        while (!_gameplayManager.IsEnd)
        {
            await UniTask.NextFrame();

            var events = _gameplayManager.PopAllEvents();
            _gameplayView.Render(events, this);
        }
    }

    public void RecieveEvent(IGameAction gameAction)
    {
        Debug.Log($"-- GameplayPresenter.RecieveEvent:[{gameAction}] --");
        _gameplayManager.EnqueueAction(gameAction);
        _gameplayView.DisableAllHandCards();
    }
}

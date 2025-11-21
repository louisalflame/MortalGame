using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Optional;
using Rayark.Mast;
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
    private IGameResultLosePresenter _gameResultLosePresenter;
    private IGameResultWinPresenter _gameResultWinPresenter;

    private readonly Queue<UniTask> _pendingActionTasks = new Queue<UniTask>();
    private bool _isProcessingAction = false;

    public IEnumerable<ISelectableView> SelectableViews => _gameplayView.SelectableViews;
    public ISelectableView BasicSelectableView => _gameplayView.BasicSelectableView;

    public GameplayPresenter(
        GameplayView gameplayView,
        IGameResultWinPanel gameResultWinPanel,
        IGameResultLosePanel gameResultLosePanel,
        GameStatus gameStatus,
        GameContextManager gameContextManager
    )
    {
        _gameplayView = gameplayView;
        _gameplayManager = new GameplayManager(gameStatus, gameContextManager);
        
        _gameInfoModel = new GameViewModel();
        _gameplayView.Init(_gameInfoModel, this, _gameplayManager, gameContextManager.LocalizeLibrary, gameContextManager.DispositionLibrary);
        _uiPresenter = new UIPresenter(_gameplayView, _gameplayView, _gameInfoModel, gameContextManager.LocalizeLibrary);
        _gameResultWinPresenter = new GameResultWinPresenter(gameResultWinPanel);
        _gameResultLosePresenter = new GameResultLosePresenter(gameResultLosePanel);
    }

    public async UniTask<GameplayResultCommand> Run()
    {
        var cts = new CancellationTokenSource();
        
        var gameplayLoopTask = _RunGameplayLoop(cts);
        var uiPresenterTask = _uiPresenter.Run(cts.Token);

        var (battleResult, _) = await UniTask.WhenAll(gameplayLoopTask, uiPresenterTask);

        if (battleResult.IsAllyWin)
        {
            var winResult = await _gameResultWinPresenter.Run();
            return new GameplayResultCommand(winResult);
        }
        else
        {
            var loseResult = await _gameResultLosePresenter.Run();
            return new GameplayResultCommand(loseResult);
        }
    }

    private async UniTask<BattleResult> _RunGameplayLoop(CancellationTokenSource cts)
    {
        _gameplayManager.Start();

        BattleResult result;
        while (!_gameplayManager.BattleResult.TryGetValue(out result))
        {
            // Process any pending action tasks first
            await _ProcessPendingActionTasks();

            await UniTask.NextFrame();

            var events = _gameplayManager.PopAllEvents();
            _gameplayView.Render(events, this);
        }

        cts.Cancel();
        _gameplayView.DisableAllInteraction();

        Debug.Log($"-- GameplayPresenter.Run: GameResult[{result}] --");
        return result;
    }

    private async UniTask _ProcessPendingActionTasks()
    {
        while (_pendingActionTasks.Count > 0)
        {
            var actionTask = _pendingActionTasks.Dequeue();
            await actionTask;
        }
    }

    public void RecieveEvent(IGameAction gameAction)
    {
        Debug.Log($"-- GameplayPresenter.RecieveEvent:[{gameAction}] --");
        
        _gameplayView.DisableAllHandCards();
        _pendingActionTasks.Enqueue(_ProcessGameAction(gameAction));
    }

    private async UniTask _ProcessGameAction(IGameAction gameAction)
    {
        var postProcessAction = await _PostProcessAction(gameAction);

        _gameplayManager.EnqueueAction(postProcessAction);
    }

    private UniTask<IGameAction> _PostProcessAction(IGameAction gameAction)
    {


        return UniTask.FromResult(gameAction);
    }
}

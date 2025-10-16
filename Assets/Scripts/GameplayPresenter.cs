using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
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
        var cardPresenterTask = _uiPresenter.Run(cts.Token);

        var (battleResult, _) = await UniTask.WhenAll(gameplayLoopTask, cardPresenterTask);

        if (battleResult.IsAllyWin)
        {
            await _gameResultWinPresenter.Run();
            return new GameplayResultCommand(new GameplayWinResult());
        }
        else
        {
            var battleLoseReaction = await _gameResultLosePresenter.Run();
            return new GameplayResultCommand(
                new GameplayLoseResult(
                    battleLoseReaction.ReactionType
                )
            );
        }
    }

    private async UniTask<BattleResult> _RunGameplayLoop(CancellationTokenSource cts)
    {
        _gameplayManager.Start();

        BattleResult result;
        while (!_gameplayManager.BattleResult.TryGetValue(out result))
        {
            await UniTask.NextFrame();

            var events = _gameplayManager.PopAllEvents();
            _gameplayView.Render(events, this);
        }

        cts.Cancel();
        _gameplayView.DisableAllInteraction();

        Debug.Log($"-- GameplayPresenter.Run: GameResult[{result}] --");
        return result;
    }

    public void RecieveEvent(IGameAction gameAction)
    {
        Debug.Log($"-- GameplayPresenter.RecieveEvent:[{gameAction}] --");
        _gameplayManager.EnqueueAction(gameAction);
        _gameplayView.DisableAllHandCards();
    }
}

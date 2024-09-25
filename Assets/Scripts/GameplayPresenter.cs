using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IGameplayActionReciever
{
    void RecieveEvent(IGameAction gameplayAction);
}

public class GameplayPresenter : IGameplayActionReciever
{
    private IGameplayView _gameplayView;
    private GameplayManager _gameplayManager;
    private UniTask _viewTask;

    public GameStatus GameStatus => _gameplayManager.GameStatus;

    public GameplayPresenter(
        IGameplayView gameplayView,
        GameStatus gameStatus,
        GameContextManager gameContextManager
    )
    {
        _gameplayView = gameplayView;
        _gameplayManager = new GameplayManager(gameStatus, gameContextManager);

        _gameplayView.Init(this, _gameplayManager);
    }

    public async UniTask<GameResult> Run()
    {
        Debug.Log("-- GameplayPresenter.Run --");

        _gameplayManager.Start();
        _viewTask = _gameplayView.Run();

        while(!_gameplayManager.IsEnd)
        {
            await UniTask.NextFrame();
            
            var events = _gameplayManager.PopAllEvents();
            _gameplayView.Render(events, this);
        }

        await _viewTask;

        return _gameplayManager.GameResult;
    }

    public void RecieveEvent(IGameAction gameAction)
    {
        Debug.Log($"-- GameplayPresenter.RecieveEvent:[{gameAction}] --");
        _gameplayManager.EnqueueAction(gameAction);
    }
}

using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IGameplayActionReciever
{
    void RecieveEvent(IGameAction gameplayAction);
}

public class GameplayPresenter : IGameplayActionReciever
{
    private GameplayView _gameplayView;
    private GameplayManager _gameplayManager;

    public GameStatus GameStatus => _gameplayManager.GameStatus;

    public GameplayPresenter(
        GameplayView gameplayView,
        GameStatus gameStatus
    )
    {
        _gameplayView = gameplayView;
        _gameplayManager = new GameplayManager(gameStatus);

        _gameplayView.Init(this, _gameplayManager);
    }

    public async UniTask<GameResult> Run()
    {
        Debug.Log("-- GameplayPresenter.Run --");

        _gameplayManager.Start();
 
        while(!_gameplayManager.IsEnd)
        {
            await UniTask.NextFrame();

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
}

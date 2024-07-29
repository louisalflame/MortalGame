using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameplayPresenter
{
    private GameplayView _gameplayView;
    private GameplayManager _gameplayManager;

    public GameplayPresenter(
        GameplayView gameplayView,
        GameStatus gameStatus
    )
    {
        _gameplayView = gameplayView;
        _gameplayManager = new GameplayManager(gameStatus);
    }

    public async UniTask<GameResult> Run()
    {
        Debug.Log("-- GameplayPresenter.Run --");

        _gameplayManager.Start();
 
        while(!_gameplayManager.IsEnd)
        {
            await UniTask.NextFrame();

            var events = _gameplayManager.PopAllEvents();
            _gameplayView.Render(events);
        }

        return _gameplayManager.GameResult;
    }
}

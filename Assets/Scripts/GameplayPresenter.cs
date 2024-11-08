using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IGameplayActionReciever
{
    void RecieveEvent(IGameAction gameplayAction);
    void ShowDeckDetailPanel();
    void ShowGraveyardDetailPanel();
}

public class GameplayPresenter : IGameplayActionReciever
{
    private IGameplayView _gameplayView;
    private GameplayManager _gameplayManager;
    private IAllCardDetailPresenter _allCardDetailPresenter;

    public GameplayPresenter(
        GameplayView gameplayView,
        GameStatus gameStatus,
        GameContextManager gameContextManager
    )
    {
        _gameplayView = gameplayView;
        _gameplayManager = new GameplayManager(gameStatus, gameContextManager);

        _gameplayView.Init(this, _gameplayManager);
        _allCardDetailPresenter = new AllCardDetailPresenter(_gameplayView, _gameplayManager);
    }

    public async UniTask<GameResult> Run()
    {
        _gameplayManager.Start();
        var v = _allCardDetailPresenter.Run();

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

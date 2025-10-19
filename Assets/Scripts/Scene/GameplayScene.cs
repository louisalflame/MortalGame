using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameplayScene : MonoBehaviour
{
    [SerializeField]
    private GameplayView _gameplayView;
    [SerializeField]
    private GameResultWinPanel _gameResultWinPanel;
    [SerializeField]
    private GameResultLosePanel _gameResultLosePanel;

    public async UniTask<GameplayResultCommand> Run(Context context)
    {
        var battleBuilder = new BattleBuidler(context);

        var gameContextManager = battleBuilder.ConstructGameContextManager();
        var initialState = battleBuilder.ConstructBattle(gameContextManager);
        var gameplayPresenter = new GameplayPresenter(
            _gameplayView,
            _gameResultWinPanel,
            _gameResultLosePanel,
            initialState,
            gameContextManager);

        var result = await gameplayPresenter.Run();

        return result; 
    }
}


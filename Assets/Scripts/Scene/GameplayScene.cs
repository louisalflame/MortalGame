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

    private Context _context;
    private BattleBuidler _battleBuidler;

    public void Initialize(Context context)
    {
        _context = context;
        _battleBuidler = new BattleBuidler(_context);
    }

    public async UniTask<GameplayResultCommand> Run()
    {
        var gameContextManager = _battleBuidler.ConstructGameContextManager();
        var initialState = _battleBuidler.ConstructBattle(gameContextManager);
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


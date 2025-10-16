using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public interface IGameResultLosePresenter
{
    UniTask<BattleLoseReaction> Run();
}

public record BattleLoseReaction(
    LoseReactionType ReactionType);

public class GameResultLosePresenter : IGameResultLosePresenter
{
    private readonly IGameResultLosePanel _losePanel;

    public GameResultLosePresenter(IGameResultLosePanel losePanel)
    {
        _losePanel = losePanel;
    }

    public async UniTask<BattleLoseReaction> Run()
    {
        var reactionType = LoseReactionType.Quit;
        var isClose = false;
        var disposables = new CompositeDisposable();

        _losePanel.RetryButton.OnClickAsObservable()
            .Subscribe(_ => {
                reactionType = LoseReactionType.Retry;
                isClose = true;
            })
            .AddTo(disposables);
        _losePanel.RestartButton.OnClickAsObservable()
            .Subscribe(_ => {
                reactionType = LoseReactionType.Restart;
                isClose = true;
            })
            .AddTo(disposables);
        _losePanel.QuitButton.OnClickAsObservable()
            .Subscribe(_ => {
                reactionType = LoseReactionType.Quit;
                isClose = true;
            })
            .AddTo(disposables);

        _losePanel.Open();
        while (!isClose)
        {
            await UniTask.NextFrame();
        }

        _losePanel.Close();
        disposables.Dispose();

        return new BattleLoseReaction(reactionType);
    }
}

using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IGameResultWinPresenter
{
    UniTask Run();
}

public class GameResultWinPresenter : IGameResultWinPresenter
{
    private readonly IGameResultWinPanel _winPanel;

    public GameResultWinPresenter(IGameResultWinPanel winPanel)
    {
        _winPanel = winPanel;
    }

    public async UniTask Run()
    {
        var isClose = false;
        while (!isClose)
        {
            await UniTask.NextFrame();
        }
    }
}
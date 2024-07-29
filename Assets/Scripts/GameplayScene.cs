using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameplayScene : MonoBehaviour
{
    [SerializeField]
    private GameplayView _gameplayView;

    public void Initialize()
    {

    }

    public async UniTask Run()
    {
        Debug.Log("-- GameplayScene.Run --");
 
        var initialState = new GameStatus(
            state: GameState.None,
            player: new PlayerEntity(),
            enemy: new PlayerEntity()
        ); 
        var gameplayPresenter = new GameplayPresenter(_gameplayView, initialState);

        var result = await gameplayPresenter.Run();
    }
}


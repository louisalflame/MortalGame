using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameplayScene : MonoBehaviour
{
    [SerializeField]
    private GameplayView _gameplayView;

    private BattleBuidler _battleBuidler;

    public void Initialize()
    {
        _battleBuidler = new BattleBuidler();
    }

    public async UniTask Run()
    {
        Debug.Log("-- GameplayScene.Run --");
 
        var initialState = _battleBuidler.ConstructBattle();
        var gameplayPresenter = new GameplayPresenter(_gameplayView, initialState);

        var result = await gameplayPresenter.Run();
    }
}


using System.Collections;
using Cysharp.Threading.Tasks;
using Rayark.Mast;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager
{
    public async UniTask<MenuScene> LoadMenuScene()
    { 
        await SceneManager.LoadSceneAsync("Menu");

        var menuScene = Object.FindFirstObjectByType<MenuScene>();

        return menuScene;
    }

    public async UniTask<GameplayScene> LoadGameplayScene()
    {
        await SceneManager.LoadSceneAsync("Gameplay");

        var gameplayScene = Object.FindFirstObjectByType<GameplayScene>();

        return gameplayScene;
    }

    public async UniTask<LoadingScene> LoadLoadingScene()
    {
        await SceneManager.LoadSceneAsync("Loading");

        var loadingScene = Object.FindFirstObjectByType<LoadingScene>();

        return loadingScene;
    }
}

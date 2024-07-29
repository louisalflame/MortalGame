using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Rayark.Mast;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    private SceneLoadManager _sceneLoadManager;

    async UniTaskVoid Start()
    {
        DontDestroyOnLoad(this);

        Application.targetFrameRate = 60;

        _sceneLoadManager = new SceneLoadManager();

        await _Gameloop();
    }

    private async UniTask _Gameloop()
    {
        var menuScene = await _sceneLoadManager.LoadMenuScene();

        await menuScene.Run();

        var gameplayScene = await _sceneLoadManager.LoadGameplayScene();

        await gameplayScene.Run();
    }
}
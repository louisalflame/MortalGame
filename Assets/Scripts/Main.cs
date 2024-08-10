using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Rayark.Mast;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    [SerializeField]
    private ScriptableDataLoader _scriptableDataLoader;

    private SceneLoadManager _sceneLoadManager;
    private Context _context;

    async UniTaskVoid Start()
    {
        DontDestroyOnLoad(this);

        Application.targetFrameRate = 60;

        _sceneLoadManager = new SceneLoadManager();
        _context = new Context(
            _scriptableDataLoader);

        await _Gameloop();
    }

    private async UniTask _Gameloop()
    {
        var menuScene = await _sceneLoadManager.LoadMenuScene();

        await menuScene.Run();

        var gameplayScene = await _sceneLoadManager.LoadGameplayScene();

        gameplayScene.Initialize(_context);
        await gameplayScene.Run();
    }
}
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MenuScene : MonoBehaviour
{
    // Get the reference to the CanvasScaler component attached to the Canvas
    [SerializeField] private CanvasScaler canvasScaler;

    public async UniTask Run()
    {
        Debug.Log("-- MenuScene.Run --");

        await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0));
    }
}

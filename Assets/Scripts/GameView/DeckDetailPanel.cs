using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DeckDetailPanel : MonoBehaviour
{
    [SerializeField]
    private Button[] _closeButtons;
    [SerializeField]
    private GameObject _panel;

    public async UniTask Run()
    {
        Debug.Log("-- DeckDetailPanel.Run --");
        
        var isOpen = true;
        var disposables = new CompositeDisposable();
        foreach (var button in _closeButtons)
        {
            button.OnClickAsObservable()
                .Subscribe(_ => isOpen = false)
                .AddTo(disposables);
        }
        
        using (disposables)
        {
            _panel.SetActive(true);

            while (isOpen)
            {
                await UniTask.NextFrame();
            }

            _panel.SetActive(false);
        }
    }
}

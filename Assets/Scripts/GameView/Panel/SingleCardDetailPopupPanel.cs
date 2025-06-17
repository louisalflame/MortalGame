using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SingleCardDetailPopupPanel : MonoBehaviour
{
    [SerializeField]
    private Button[] _closeButtons;
    [SerializeField]
    private GameObject _panel;
    [SerializeField]
    private CardView _cardView;
    [SerializeField]
    private CardPropertyHint _cardPropertyHint;

    private IGameViewModel _gameViewModel;
    private LocalizeLibrary _localizeLibrary;

    public void Init(IGameViewModel gameInfoModel, LocalizeLibrary localizeLibrary)
    {
        _gameViewModel = gameInfoModel;
        _localizeLibrary = localizeLibrary;
        _cardPropertyHint.Init(_localizeLibrary);
        _cardView.Initialize(_gameViewModel, _localizeLibrary);
    }

    public async UniTask Run(CardInfo cardInfo)
    {
        _cardView.SetCardInfo(cardInfo);

        var disposables = new CompositeDisposable();
        var isClose = false;
        foreach (var button in _closeButtons)
        {
            button.OnClickAsObservable()
                .Subscribe(_ => isClose = true)
                .AddTo(disposables);
        }
        
        using (disposables)
        {
            _panel.SetActive(true);

            _cardPropertyHint.ShowHint(cardInfo, false, _cardView.RectTransform);

            while (!isClose)
            {
                await UniTask.NextFrame();
            }

            _panel.SetActive(false);

            _cardPropertyHint.HideHint();
        }
    }
}

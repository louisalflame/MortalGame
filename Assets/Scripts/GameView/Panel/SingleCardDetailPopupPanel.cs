using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public enum SingleCardDetailPopupPanelState
{
    Idle = 0,
    Close,
}

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

    private IGameInfoModel _gameInfoModel;
    private LocalizeLibrary _localizeLibrary;
    private SingleCardDetailPopupPanelState _state;

    public void Init(IGameInfoModel gameInfoModel, LocalizeLibrary localizeLibrary)
    {
        _gameInfoModel = gameInfoModel;
        _localizeLibrary = localizeLibrary;
        _cardPropertyHint.Init(_localizeLibrary);
        _cardView.Initialize(_gameInfoModel, _localizeLibrary);
    }

    public async UniTask Run(CardInfo cardInfo)
    {
        _cardView.SetCardInfo(cardInfo);

        var disposables = new CompositeDisposable();
        foreach (var button in _closeButtons)
        {
            button.OnClickAsObservable()
                .Subscribe(_ => _state = SingleCardDetailPopupPanelState.Close)
                .AddTo(disposables);
        }
        
        using (disposables)
        {
            _state = SingleCardDetailPopupPanelState.Idle;
            _panel.SetActive(true);

            _cardPropertyHint.ShowHint(cardInfo, false, _cardView.RectTransform);

            while (_state != SingleCardDetailPopupPanelState.Close)
            {
                await UniTask.Yield();
            }

            _panel.SetActive(false);

            _cardPropertyHint.HideHint();
        }
    }
}

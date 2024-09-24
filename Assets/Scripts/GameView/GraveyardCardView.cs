using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GraveyardCardView : MonoBehaviour
{
    [SerializeField]
    private Button _deckButton;
    [SerializeField]
    private TextMeshProUGUI _graveyardCountText;
    
    private IGameplayStatusWatcher _statusWatcher;
    private CompositeDisposable _disposables = new CompositeDisposable();

    public void Init(IGameplayStatusWatcher statusWatcher)
    {
        _statusWatcher = statusWatcher;
        
        _deckButton.OnClickAsObservable()
            .Subscribe(_ => Debug.Log($" Graveyard:[{_statusWatcher.GameStatus.Ally.CardManager.Graveyard.Cards.Count}]  "))
            .AddTo(_disposables);
        _graveyardCountText.text = _statusWatcher.GameStatus.Ally.CardManager.Graveyard.Cards.Count.ToString();
    }

    public void UpdateDeckView(UsedCardEvent usedCardEvent)
    {
        _graveyardCountText.text = usedCardEvent.GraveyardCardInfos.Count.ToString();
    }
    public void UpdateDeckView(RecycleHandCardEvent recycleHandCardEvent)
    {
        _graveyardCountText.text = recycleHandCardEvent.GraveyardCardInfos.Count.ToString();
    }
    public void UpdateDeckView(RecycleGraveyardEvent recycleGraveyardEvent)
    {
        _graveyardCountText.text = recycleGraveyardEvent.GraveyardCardInfos.Count.ToString();
    }
}

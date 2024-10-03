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

    public void Init(IGameplayStatusWatcher statusWatcher, IGameplayView gameplayView)
    {
        _statusWatcher = statusWatcher;
        
        _deckButton.OnClickAsObservable()
            .Subscribe(_ => gameplayView.ClickGraveyardDetailPanel())
            .AddTo(_disposables);
        _graveyardCountText.text = _statusWatcher.GameStatus.Ally.CardManager.Graveyard.Cards.Count.ToString();
    }

    public void UpdateDeckView(UsedCardEvent usedCardEvent)
    {
        _graveyardCountText.text = usedCardEvent.GraveyardInfo.Count.ToString();
    }
    public void UpdateDeckView(RecycleHandCardEvent recycleHandCardEvent)
    {
        _graveyardCountText.text = recycleHandCardEvent.GraveyardInfo.Count.ToString();
    }
    public void UpdateDeckView(RecycleGraveyardEvent recycleGraveyardEvent)
    {
        _graveyardCountText.text = recycleGraveyardEvent.GraveyardInfo.Count.ToString();
    }
}

using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DeckCardView : MonoBehaviour
{
    [SerializeField]
    private Button _deckButton;
    [SerializeField]
    private TextMeshProUGUI _deckCountText;
    
    private IGameplayStatusWatcher _statusWatcher;
    private CompositeDisposable _disposables = new CompositeDisposable();

    public void Init(IGameplayStatusWatcher statusWatcher)
    {
        _statusWatcher = statusWatcher;
        
        _deckButton.OnClickAsObservable()
            .Subscribe(_ => Debug.Log($" Deck:[{_statusWatcher.GameStatus.Ally.Deck.Cards.Count}]  "))
            .AddTo(_disposables);
        _deckCountText.text = _statusWatcher.GameStatus.Ally.Deck.Cards.Count.ToString();
    }

    public void UpdateDeckView(DrawCardEvent drawCardEvent)
    {
        _deckCountText.text = drawCardEvent.DeckCardInfos.Count.ToString();
    }
    public void UpdateDeckView(RecycleGraveyardEvent recycleGraveyardEvent)
    {
        _deckCountText.text = recycleGraveyardEvent.DeckCardInfos.Count.ToString();
    }
}
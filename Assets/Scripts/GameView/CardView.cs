using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _title;
    [SerializeField]
    private TextMeshProUGUI _cost;
    [SerializeField]
    private TextMeshProUGUI _power;
    [SerializeField]
    private Button _button;

    private CompositeDisposable _disposables = new CompositeDisposable();

    public void SetCardInfo(CardInfo cardInfo)
    {
        _title.text = cardInfo.Title;

        _button.OnClickAsObservable()
            .Subscribe(_ => Debug.Log("Button Click"))
            .AddTo(_disposables);
        _button.OnPointerEnterAsObservable()
            .Subscribe(_ => Debug.Log("Pointer Enter"))
            .AddTo(_disposables);
    }

    public void Reset()
    {
        _disposables.Clear();
    }
}

public class CardInfo
{
    public string Title { get; private set; }

    public CardInfo(CardEntity card)
    {
        Title = card.Title;
    }
}
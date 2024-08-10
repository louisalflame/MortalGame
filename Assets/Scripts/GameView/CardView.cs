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

    public void SetCardInfo(CardInfo cardInfo, IGameplayActionReciever reciever)
    {
        _title.text = cardInfo.Title;

        _button.OnClickAsObservable()
            .Subscribe(_ => 
                reciever.RecieveEvent(
                    new UseCardAction{ CardIndentity = cardInfo.CardIndentity }))
            .AddTo(_disposables);
        _button.OnPointerEnterAsObservable()
            .Subscribe(_ => {})
            .AddTo(_disposables);
        _button.OnPointerExitAsObservable()
            .Subscribe(_ => {})
            .AddTo(_disposables);
    }

    public void Reset()
    {
        _disposables.Clear();
    }
}

public class CardInfo
{
    public string CardIndentity { get; private set; }
    public string Title { get; private set; }
    public string Info { get; private set; }
    public int Cost { get; private set; }
    public int Power { get; private set; }

    public CardInfo(CardEntity card)
    {
        CardIndentity = card.Indentity;
        Title = card.Title;
        Info = card.Info;
        Cost = card.Cost;
        Power = card.Power;
    }
}
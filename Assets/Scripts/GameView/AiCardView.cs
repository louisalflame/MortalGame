using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class AiCardView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _title;
    [SerializeField]
    private TextMeshProUGUI _cost;
    [SerializeField]
    private TextMeshProUGUI _power;

    private CompositeDisposable _disposables = new CompositeDisposable();

    public void SetCardInfo(CardInfo cardInfo, IGameplayActionReciever reciever)
    {
        _title.text = cardInfo.Title;
    }

    public void Reset()
    {
        _disposables.Clear();
    }
}

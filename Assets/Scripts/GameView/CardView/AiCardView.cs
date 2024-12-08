using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class AiCardView : MonoBehaviour, IRecyclable, ISelectableView
{
    [SerializeField]
    private RectTransform _rectTransform;
    [SerializeField]
    private TextMeshProUGUI _title;
    [SerializeField]
    private TextMeshProUGUI _info;
    [SerializeField]
    private TextMeshProUGUI _cost;
    [SerializeField]
    private TextMeshProUGUI _power;


    private CompositeDisposable _disposables = new CompositeDisposable();

    public RectTransform RectTransform => _rectTransform;

    public void SetCardInfo(CardInfo cardInfo, LocalizeLibrary localizeLibrary)
    {
        var cardLocalizeData = localizeLibrary.Get(LocalizeTitleInfoType.Card, cardInfo.CardDataID);
        _title.text = cardLocalizeData.Title;
        _info.text = cardLocalizeData.Info;
        _cost.text = cardInfo.Cost.ToString();
        _power.text = cardInfo.Power.ToString();
    }

    public void Reset()
    {
        _disposables.Clear();
    }

    public void OnSelect()
    {
        throw new System.NotImplementedException();
    }
}

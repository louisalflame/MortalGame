using System;
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
    public TargetType TargetType => TargetType.Card;
    public Guid TargetIdentity => _cardIdentity;

    private Guid _cardIdentity;

    public void SetCardInfo(CardInfo cardInfo, LocalizeLibrary localizeLibrary)
    {
        var cardLocalizeData = localizeLibrary.Get(LocalizeTitleInfoType.Card, cardInfo.CardDataID);
        _cardIdentity = cardInfo.Identity;
        _title.text = cardLocalizeData.Title;
        _info.text = cardLocalizeData.Info;
        _cost.text = cardInfo.Cost.ToString();
        _power.text = cardInfo.Power.ToString();
    }

    public void Reset()
    {
        _disposables.Clear();
        _cardIdentity = Guid.Empty;
    }

    public void OnSelect()
    {
    }
    public void OnDeselect()
    {
    }
}

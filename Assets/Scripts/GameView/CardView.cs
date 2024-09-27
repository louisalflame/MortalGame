using System;
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
    private CompositeDisposable _useCardDisposables = new CompositeDisposable();

    public void SetCardInfo(CardInfo cardInfo)
    {
        _title.text = cardInfo.Title;
        _cost.text = cardInfo.Cost.ToString();
        _power.text = cardInfo.Power.ToString();

        _button.OnPointerEnterAsObservable()
            .Subscribe(_ => {}) //TODO: Show card detail info
            .AddTo(_disposables);
        _button.OnPointerExitAsObservable()
            .Subscribe(_ => {}) //TODO: Hide card detail info
            .AddTo(_disposables);
    }

    public void EnableUseCardAction(CardInfo cardInfo, IGameplayActionReciever reciever)
    {
        _button.OnClickAsObservable()
            .Subscribe(_ => 
                reciever.RecieveEvent(
                    new UseCardAction{ CardIndentity = cardInfo.Indentity }))
            .AddTo(_useCardDisposables);
    }
    public void DisableUseCardAction()
    {
        _useCardDisposables.Clear();
    }


    public void Reset()
    {
        _disposables.Clear();
        _useCardDisposables.Clear();
    }  
}

public class CardInfo
{
    public Guid Indentity { get; private set; }
    public string Title { get; private set; }
    public string Info { get; private set; }
    public int Cost { get; private set; }
    public int Power { get; private set; }

    public CardInfo(CardEntity card)
    {
        Indentity = card.Indentity;
        Title = card.Title;
        Info = card.Info;
        Cost = card.Cost;
        Power = card.Power;
    }
}
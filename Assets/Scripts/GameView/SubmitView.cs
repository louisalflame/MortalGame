using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SubmitView : MonoBehaviour
{
    [SerializeField]
    private Button _submitButton;

    public void Init(IGameplayActionReciever reciever)
    {
        _submitButton.OnClickAsObservable()
            .Subscribe(_ => 
                reciever.RecieveEvent(
                    new TurnSubmitAction { Faction = Faction.Player}))
            .AddTo(this);
    }
}

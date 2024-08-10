using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllyInfoView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _TurnText;

    [SerializeField]
    private TextMeshProUGUI _nameText;

    [SerializeField]
    private TextMeshProUGUI _powerText;

    [SerializeField]
    private HealthBarView _healthBarView;

    [SerializeField]
    private EnergyBarView _energyBarView;
    
    private IGameplayStatusWatcher _statusWatcher;

    public void Init(IGameplayStatusWatcher statusWatcher)
    {
        _statusWatcher = statusWatcher;
    }

    public void SetPlayerInfo(int round, AllyEntity ally)
    {
        _TurnText.text = round.ToString();
        _nameText.text = ally.Name;
        _powerText.text = ally.Character.PowerManager.Power.ToString();
        _healthBarView.SetHealth(ally.Character.HealthManager.Hp, ally.Character.HealthManager.MaxHp);
        _energyBarView.SetEnergy(ally.Character.EnergyManager.Energy, ally.Character.EnergyManager.MaxEnergy);
    }
    
    public void UpdateEnergy(ConsumeEnergyEvent consumeEnergyEvent)
    {
        _energyBarView.SetEnergy(consumeEnergyEvent.Energy, consumeEnergyEvent.MaxEnergy);
    }
    public void UpdateEnergy(GainEnergyEvent gainEnergyEvent)
    {
        _energyBarView.SetEnergy(gainEnergyEvent.Energy, gainEnergyEvent.MaxEnergy);
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInfoView : MonoBehaviour
{
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

    public void SetPlayerInfo(EnemyEntity enemy)
    {
        _nameText.text = enemy.Name;
        _powerText.text = enemy.Character.PowerManager.Power.ToString();
        _healthBarView.SetHealth(enemy.Character.HealthManager.Hp, enemy.Character.HealthManager.MaxHp);
        _energyBarView.SetEnergy(enemy.Character.EnergyManager.Energy, enemy.Character.EnergyManager.MaxEnergy);
    }

    public void UpdateEnergy(ConsumeEnergyEvent consumeEnergyEvent)
    {
        _energyBarView.SetEnergy(consumeEnergyEvent.Energy, consumeEnergyEvent.MaxEnergy);
    }
    public void UpdateEnergy(GainEnergyEvent gainEnergyEvent)
    {
        _energyBarView.SetEnergy(gainEnergyEvent.Energy, gainEnergyEvent.MaxEnergy);
    }

    public void UpdateHealth(TakeDamageEvent takeDamageEvent)
    {
        _healthBarView.SetHealth(takeDamageEvent.Hp, takeDamageEvent.MaxHp);
    }
}

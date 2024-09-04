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
        _healthBarView.SetHealth(ally.Character.CurrentHealth, ally.Character.MaxHealth);
        _healthBarView.SetShield(ally.Character.CurrentArmor);    
        _energyBarView.SetEnergy(ally.Character.CurrentEnergy, ally.Character.MaxEnergy);
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
        _healthBarView.SetShield(takeDamageEvent.Dp);    
    }
    public void UpdateHealth(GetHealEvent getHealEvent)
    {
        _healthBarView.SetHealth(getHealEvent.Hp, getHealEvent.MaxHp);
        _healthBarView.SetShield(getHealEvent.Dp);    
    }
    public void UpdateHealth(GetShieldEvent getShieldEvent)
    {
        _healthBarView.SetHealth(getShieldEvent.Hp, getShieldEvent.MaxHp);
        _healthBarView.SetShield(getShieldEvent.Dp);    
    }
}

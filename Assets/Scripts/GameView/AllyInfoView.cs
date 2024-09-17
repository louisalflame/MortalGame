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

    [SerializeField]
    private BuffCollectionView _buffCollectionView;
    
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

    public void UpdateHealth(HealthEvent healthEvent)
    {
        _healthBarView.SetHealth(healthEvent.Hp, healthEvent.MaxHp);
        _healthBarView.SetShield(healthEvent.Dp);    
    }

    public void AddBuff(AddBuffEvent addBuffEvent)
    {
        _buffCollectionView.AddBuff(addBuffEvent.Buff);
    }
    public void RemoveBuff(RemoveBuffEvent removeBuffEvent)
    {
        _buffCollectionView.RemoveBuff(removeBuffEvent.Buff);
    }
    public void UpdateBuff(UpdateBuffEvent updateBuffEvent)
    {
        _buffCollectionView.UpdateBuff(updateBuffEvent.Buff);
    }
}

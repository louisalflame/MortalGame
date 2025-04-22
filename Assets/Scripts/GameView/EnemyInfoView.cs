using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInfoView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _nameText;

    [SerializeField]
    private HealthBarView _healthBarView;

    [SerializeField]
    private EnergyBarView _energyBarView;

    [SerializeField]
    private PlayerBuffCollectionView _buffCollectionView;
    
    
    private IGameplayStatusWatcher _statusWatcher;
    private LocalizeLibrary _localizeLibrary;

    public void Init(
        IGameplayStatusWatcher statusWatcher,
        SimpleTitleIInfoHintView simpleHintView,
        LocalizeLibrary localizeLibrary)
    {
        _statusWatcher = statusWatcher;
        _localizeLibrary = localizeLibrary;
        _buffCollectionView.Init(simpleHintView);
    }

    public void SetPlayerInfo(EnemyEntity enemy)
    {
        _nameText.text = _localizeLibrary.Get(LocalizeSimpleType.PlayerName, enemy.MainCharacter.NameKey);
        _healthBarView.SetHealth(enemy.MainCharacter.CurrentHealth, enemy.MainCharacter.MaxHealth);
        _healthBarView.SetShield(enemy.MainCharacter.CurrentArmor);
        _energyBarView.SetEnergy(enemy.CurrentEnergy, enemy.MaxEnergy);
    }

    public void UpdateEnergy(LoseEnergyEvent loseEnergyEvent)
    {
        _energyBarView.SetEnergy(loseEnergyEvent.Energy, loseEnergyEvent.MaxEnergy);
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

    public void AddBuff(AddPlayerBuffEvent addBuffEvent)
    {
        _buffCollectionView.AddBuff(addBuffEvent.Buff);
    }
    public void RemoveBuff(RemovePlayerBuffEvent removeBuffEvent)
    {
        _buffCollectionView.RemoveBuff(removeBuffEvent.Buff);
    }
    public void UpdateBuff(UpdatePlayerBuffEvent updateBuffEvent)
    {
        _buffCollectionView.UpdateBuff(updateBuffEvent.Buff);
    }
}

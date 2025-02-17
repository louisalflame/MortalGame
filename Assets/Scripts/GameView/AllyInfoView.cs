using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllyInfoView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _nameText;

    [SerializeField]
    private HealthBarView _healthBarView;

    [SerializeField]
    private EnergyBarView _energyBarView;

    [SerializeField]
    private DispositionView _dispositionView;

    [SerializeField]
    private BuffCollectionView _buffCollectionView;
    
    private IGameplayStatusWatcher _statusWatcher;
    private TopBarInfoView _topBarInfoView;
    private LocalizeLibrary _localizeLibrary;

    public void Init(
        IGameplayStatusWatcher statusWatcher, 
        TopBarInfoView topBarInfoView, 
        SimpleTitleIInfoHintView simpleHintView,
        LocalizeLibrary localizeLibrary, 
        DispositionLibrary dispositionLibrary)
    {
        _statusWatcher = statusWatcher;
        _topBarInfoView = topBarInfoView;
        _localizeLibrary = localizeLibrary;
        _dispositionView.Init(localizeLibrary, dispositionLibrary);
        _buffCollectionView.Init(simpleHintView);
    }

    public void SetPlayerInfo(int round, AllyEntity ally)
    {
        _topBarInfoView.UpdateTurnInfo(round);
        _nameText.text = _localizeLibrary.Get(LocalizeSimpleType.PlayerName, ally.MainCharacter.NameKey);
        _healthBarView.SetHealth(ally.MainCharacter.CurrentHealth, ally.MainCharacter.MaxHealth);
        _healthBarView.SetShield(ally.MainCharacter.CurrentArmor);    
        _energyBarView.SetEnergy(ally.CurrentEnergy, ally.MaxEnergy);
        _dispositionView.SetDisposition(ally.DispositionManager.CurrentDisposition, ally.DispositionManager.MaxDisposition);
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

    public void UpdateDisposition()
    { 
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

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoView : MonoBehaviour
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

    public void SetPlayerInfo(int round, PlayerEntity player)
    {
        _TurnText.text = round.ToString();
        _nameText.text = player.Name;
        _powerText.text = player.Character.PowerManager.Power.ToString();
        _healthBarView.SetHealth(player.Character.HealthManager.Hp, player.Character.HealthManager.MaxHp);
        _energyBarView.SetEnergy(player.Character.EnergyManager.Energy, player.Character.EnergyManager.MaxEnergy);
    }
}

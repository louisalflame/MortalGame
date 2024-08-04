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
    
    private IGameplayStatusWatcher _statusWatcher;

    public void Init(IGameplayStatusWatcher statusWatcher)
    {
        _statusWatcher = statusWatcher;
    }

    public void SetPlayerInfo(PlayerEntity player)
    {
        _nameText.text = player.Name;
        _powerText.text = player.Character.PowerManager.Power.ToString();
        _healthBarView.SetHealth(player.Character.HealthManager.Hp, player.Character.HealthManager.MaxHp);
    }
}

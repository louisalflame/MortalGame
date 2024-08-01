using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarView : MonoBehaviour
{
    [SerializeField]
    private Image _image;
    [SerializeField]
    private TextMeshProUGUI _hpText;
    [SerializeField]
    private TextMeshProUGUI _maxHpText;

    public void SetHealth(int health, int maxHealth)
    {
        _image.fillAmount = (float)health / maxHealth;
        _hpText.text = health.ToString();
        _maxHpText.text = maxHealth.ToString();
    }
}

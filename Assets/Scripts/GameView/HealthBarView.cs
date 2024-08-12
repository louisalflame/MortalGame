using Sirenix.Utilities;
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
    [SerializeField]
    private TextMeshProUGUI _shieldText;
    [SerializeField]
    private GameObject[] _shieldObjects;

    public void SetHealth(int health, int maxHealth)
    {
        _image.fillAmount = (float)health / maxHealth;
        _hpText.text = health.ToString();
        _maxHpText.text = maxHealth.ToString();
    }
    public void SetShield(int shield)
    {
        _shieldObjects.ForEach(obj => obj.SetActive(shield > 0));
        _shieldText.text = shield.ToString();
    }
}

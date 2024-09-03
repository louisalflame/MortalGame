using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffView : MonoBehaviour
{
    [SerializeField]
    private Image _buffIcon;

    [SerializeField]
    private TextMeshProUGUI _levelText;

    public void Reset()
    {
        _buffIcon.sprite = null;
        _levelText.text = string.Empty;
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DispositionView : MonoBehaviour
{
    [SerializeField]
    private Image _image;
    [SerializeField]
    private TextMeshProUGUI _dispositionText;

    public void SetDisposition(int disposition, int maxDisposition)
    {
        _image.fillAmount = (float)disposition / maxDisposition; 
    }
}

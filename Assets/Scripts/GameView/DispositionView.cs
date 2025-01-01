using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class DispositionView : MonoBehaviour
{
    [SerializeField]
    private Image _image;
    [SerializeField]
    private TextMeshProUGUI _dispositionText;

    private LocalizeLibrary _localizeLibrary;
    private DispositionLibrary _dispositionLibrary;

    public void Init(LocalizeLibrary localizeLibrary, DispositionLibrary dispositionLibrary)
    {
        _localizeLibrary = localizeLibrary;
        _dispositionLibrary = dispositionLibrary;
    }

    public void SetDisposition(int disposition, int maxDisposition)
    {
        _image.fillAmount = (float)disposition / maxDisposition;

        var dispositionId = _dispositionLibrary.GetDispositionId(disposition);
        var localizeInfo = _localizeLibrary.Get(LocalizeTitleInfoType.Disposition, dispositionId);
        _dispositionText.text = localizeInfo.Title;
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleTitleIInfoHintView : MonoBehaviour
{    
    [SerializeField]
    private TextMeshProUGUI _titleText;

    [SerializeField]
    private TextMeshProUGUI _infoText;

    [SerializeField]
    private RectTransform _hintRectTransform;

    private LocalizeLibrary _localizeLibrary;

    public void Init(LocalizeLibrary localizeLibrary)
    {
        _localizeLibrary = localizeLibrary;
    }

    public void ShowBuffInfo(PlayerBuffInfo buffInfo, RectTransform targetRect)
    {
        gameObject.SetActive(true);

        var localizeData = _localizeLibrary.Get(LocalizeTitleInfoType.Buff, buffInfo.Id);
        _titleText.text = localizeData.Title;
        _infoText.text = localizeData.Info;

        var upperPos = targetRect.TransformPoint(new Vector3(0, targetRect.rect.height / 2f, 0));
        var localUpperPos = _hintRectTransform.parent.InverseTransformPoint(upperPos);
        _hintRectTransform.localPosition = (targetRect.position.x < Screen.width / 2f) ?
            new Vector2(localUpperPos.x + (targetRect.rect.width / 2f) + (_hintRectTransform.rect.width / 2f), localUpperPos.y) :
            new Vector2(localUpperPos.x - (targetRect.rect.width / 2f) - (_hintRectTransform.rect.width / 2f), localUpperPos.y);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_hintRectTransform);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        _titleText.text = string.Empty;
        _infoText.text = string.Empty;
    }
}

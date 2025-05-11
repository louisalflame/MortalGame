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
        var templateValue = buffInfo.GetTemplateValues();
        
        _titleText.text = localizeData.Title;
        _infoText.text = localizeData.Info.ReplaceTemplateKeys(templateValue);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_hintRectTransform);

        _SetAnchorPosition(targetRect);
    }

    private void _SetAnchorPosition(RectTransform targetRect)
    {
        var canvas = _hintRectTransform.GetComponentInParent<Canvas>();
        var rectOnCanvas = canvas.GetRectOnCanvas(targetRect);

        if(rectOnCanvas.center.x > 0)
        {
            _hintRectTransform.anchoredPosition = new Vector2(
                rectOnCanvas.min.x - _hintRectTransform.rect.width / 2,
                rectOnCanvas.max.y - _hintRectTransform.rect.height / 2);
        }
        else
        {
            _hintRectTransform.anchoredPosition = new Vector2(
                rectOnCanvas.max.x + _hintRectTransform.rect.width / 2,
                rectOnCanvas.max.y - _hintRectTransform.rect.height / 2);
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
        _titleText.text = string.Empty;
        _infoText.text = string.Empty;
    }
}
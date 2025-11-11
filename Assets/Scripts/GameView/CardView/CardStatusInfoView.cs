using TMPro;
using UnityEngine;

public class CardBuffInfoView : MonoBehaviour, IRecyclable
{
    [SerializeField]
    private TextMeshProUGUI _cardBuffTitleText;
    [SerializeField]
    private TextMeshProUGUI _cardBuffInfoText;

    public void SetInfo(CardBuffInfo info, LocalizeLibrary localizeLibrary)
    {
        var localizeData = localizeLibrary.Get(LocalizeType.CardBuff, info.CardBuffDataId);
        var templateValue = info.GetTemplateValues();

        _cardBuffTitleText.text = localizeData.Title;
        _cardBuffInfoText.text = localizeData.Info.ReplaceTemplateKeys(templateValue);
    }

    public void Reset()
    {
    }
}

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
        var localizeData = localizeLibrary.Get(LocalizeTitleInfoType.CardBuff, info.CardBuffDataId);
        _cardBuffTitleText.text = localizeData.Title;
        _cardBuffInfoText.text = localizeData.Info;
    }

    public void Reset()
    {
    }
}

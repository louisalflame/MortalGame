using TMPro;
using UnityEngine;

public class CardStatusInfoView : MonoBehaviour, IRecyclable
{
    [SerializeField]
    private TextMeshProUGUI _cardStatusTitleText;
    [SerializeField]
    private TextMeshProUGUI _cardStatusInfoText;

    public void SetInfo(CardStatusInfo info, LocalizeLibrary localizeLibrary)
    {
        var localizeData = localizeLibrary.Get(LocalizeTitleInfoType.CardStatus, info.CardStatusDataId);
        _cardStatusTitleText.text = localizeData.Title;
        _cardStatusInfoText.text = localizeData.Info;
    }

    public void Reset()
    {
    }
}

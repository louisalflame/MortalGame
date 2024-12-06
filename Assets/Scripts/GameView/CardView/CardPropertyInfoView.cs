using TMPro;
using UnityEngine;

public class CardPropertyInfoView : MonoBehaviour, IRecyclable
{
    [SerializeField]
    private TextMeshProUGUI _propertyTitleText;
    [SerializeField]
    private TextMeshProUGUI _propertyInfoText;

    public void SetInfo(CardPropertyInfo info, LocalizeLibrary localizeLibrary)
    {
        _propertyTitleText.text = localizeLibrary.Get(LocalizeType.CardPropertyTitle, info.TitleKey);
        _propertyInfoText.text = localizeLibrary.Get(LocalizeType.CardPropertyInfo, info.InfoKey);
    }

    public void Reset()
    {
    }
}

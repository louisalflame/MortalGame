using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _title;

    public void SetCardInfo(CardInfo cardInfo)
    {
        _title.text = cardInfo.Title;
    }
}

public class CardInfo
{
    public string Title { get; private set; }

    public CardInfo(CardEntity card)
    {
        Title = card.Title;
    }
}
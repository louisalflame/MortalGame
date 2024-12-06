using Sirenix.OdinInspector;
using UnityEngine;

public class FocusCardDetailView : MonoBehaviour
{
    [SerializeField]
    private GameObject _panel;
    [SerializeField]
    private RectTransform _content;
    [SerializeField]
    private CardView _cardView;
    [SerializeField]
    private CardPropertyHint _cardPropertyHint;

    private Canvas _canvas;
    private LocalizeLibrary _localizeLibrary;

    public void Init(Canvas canvas, LocalizeLibrary localizeLibrary)
    {
        _canvas = canvas;
        _localizeLibrary = localizeLibrary;
        _cardPropertyHint.Init(localizeLibrary);
    }   

    public void ShowFocus(CardInfo cardInfo, CardView cardView, bool smallDirection)
    {
        _cardView.SetCardInfo(cardInfo, _localizeLibrary);
        _cardPropertyHint.ShowHint(cardInfo, smallDirection);

        var cardViewPosition = Camera.main.WorldToScreenPoint(cardView.transform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform, cardViewPosition, null, out Vector2 overlayPosition);

        _content.anchoredPosition = new Vector2(overlayPosition.x, _content.anchoredPosition.y);

        _panel.SetActive(true);
    }

    public void HideFocus()
    {
        _cardPropertyHint.HideHint();

        _panel.SetActive(false);
    }
}

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

    private IGameViewModel _gameViewModel;
    private LocalizeLibrary _localizeLibrary;

    public void Init(IGameViewModel gameInfoModel, LocalizeLibrary localizeLibrary)
    {
        _gameViewModel = gameInfoModel;
        _localizeLibrary = localizeLibrary;
        _cardPropertyHint.Init(localizeLibrary);
        _cardView.Initialize(gameInfoModel, localizeLibrary);
    }

    public void ShowFocus(CardInfo cardInfo, RectTransform targetRect)
    {
        _panel.SetActive(true);

        var canvas = _content.GetComponentInParent<Canvas>();
        var rectOnCanvas = canvas.GetRectOnCanvas(targetRect, _content.parent as RectTransform);

        _content.anchoredPosition = new Vector2(rectOnCanvas.center.x, _content.anchoredPosition.y);
        var hintDirection = rectOnCanvas.center.x > 0;

        _cardView.SetCardInfo(cardInfo);
        _cardPropertyHint.ShowHint(cardInfo, hintDirection, _cardView.RectTransform);
    }

    public void HideFocus()
    {
        _cardPropertyHint.HideHint();

        _panel.SetActive(false);
    }
}

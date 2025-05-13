using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardPropertyHint : MonoBehaviour
{
    [SerializeField]
    private CardBuffInfoViewFactory _cardBuffInfoViewFactory;
    [SerializeField]
    private GameKeyWordInfoViewFactory _gameKeyWordInfoViewFactory;
    [SerializeField]
    private RectTransform _layoutParent;
    [SerializeField]
    private RectTransform _content;
    [SerializeField]
    private float _offsetX;

    private LocalizeLibrary _localizeLibrary;
    private List<CardBuffInfoView> _propertyViews = new List<CardBuffInfoView>();

    public void Init(LocalizeLibrary localizeLibrary)
    {
        _localizeLibrary = localizeLibrary;
    }

    public void ShowHint(CardInfo cardInfo, bool smallDirection, RectTransform referenceRect)
    {
        if(cardInfo.StatusInfos.Count == 0)
        {
            HideHint();
            return;
        }

        _content.gameObject.SetActive(true);
        foreach(var statusInfo in cardInfo.StatusInfos)
        {
            var cardBuffInfoView = _cardBuffInfoViewFactory.CreatePrefab();
            cardBuffInfoView.transform.SetParent(_layoutParent, false);
            _propertyViews.Add(cardBuffInfoView);

            cardBuffInfoView.SetInfo(statusInfo, _localizeLibrary);
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutParent);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
        
        var canvas = _content.GetComponentInParent<Canvas>();
        var rectOnCanvas = canvas.GetRectOnCanvas(referenceRect, _content.parent as RectTransform);

        var anchorY = _content.rect.height > rectOnCanvas.height ?
            rectOnCanvas.min.y + _content.rect.height / 2 : 
            rectOnCanvas.max.y - _content.rect.height / 2;

        _content.pivot = new Vector2(smallDirection ? 0 : 1, _content.pivot.y);
        _content.anchoredPosition = new Vector2(
            smallDirection ? _offsetX : -_offsetX, 
            anchorY);
    }

    public void HideHint()
    {
        foreach (var propertyView in _propertyViews)
        {
            _cardBuffInfoViewFactory.RecyclePrefab(propertyView);
        }
        _propertyViews.Clear();
        _content.gameObject.SetActive(false);
    }
}

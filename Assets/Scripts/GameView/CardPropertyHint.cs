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
    private Transform _cardPropertyInfoViewParent;
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

    public void ShowHint(CardInfo cardInfo, bool smallDirection)
    {
        _UpdateContentAnchorPivotX(smallDirection);

        foreach(var statusInfo in cardInfo.StatusInfos)
        {
            var cardBuffInfoView = _cardBuffInfoViewFactory.CreatePrefab();
            cardBuffInfoView.transform.SetParent(_cardPropertyInfoViewParent, false);
            _propertyViews.Add(cardBuffInfoView);

            cardBuffInfoView.SetInfo(statusInfo, _localizeLibrary);
        }
    }

    public void HideHint()
    {
        foreach (var propertyView in _propertyViews)
        {
            _cardBuffInfoViewFactory.RecyclePrefab(propertyView);
        }
        _propertyViews.Clear();
    }

    private void _UpdateContentAnchorPivotX(bool isRightSide)
    {
        _content.pivot = new Vector2(isRightSide ? 0 : 1, _content.pivot.y);
        _content.anchoredPosition = new Vector2(isRightSide ? _offsetX : -_offsetX, _content.anchoredPosition.y);
    }
}

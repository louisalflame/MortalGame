using System.Collections.Generic;
using UnityEngine;

public class CardPropertyHint : MonoBehaviour
{
    [SerializeField]
    private Transform _hintTransform;
    [SerializeField]
    private CardPropertyInfoViewFactory _cardPropertyInfoViewFactory;
    [SerializeField]
    private Transform _cardPropertyInfoViewParent;
    [SerializeField]
    private RectTransform _layoutGroupRectTransfrom;

    private List<CardPropertyInfoView> _propertyViews = new List<CardPropertyInfoView>();

    public void ShowHint(CardInfo cardInfo, CardView cardView, bool smallDirection)
    {
        _hintTransform.gameObject.SetActive(true);
        _hintTransform.SetParent(cardView.transform, false);

        foreach(var appendProperty in cardInfo.AppendProperties)
        {
            var cardPropertyInfoView = _cardPropertyInfoViewFactory.CreatePrefab();
            cardPropertyInfoView.transform.SetParent(_cardPropertyInfoViewParent, false);
            _propertyViews.Add(cardPropertyInfoView);
            //TODO : cardPropertyInfoView.setInfo(appendProperty);

        }

        if(_propertyViews.Count > 5) // If layout reach bottom, move anchor/pivot to bottom
        {
            _layoutGroupRectTransfrom.anchorMin = new Vector2(_layoutGroupRectTransfrom.anchorMin.x, 0);
            _layoutGroupRectTransfrom.anchorMax = new Vector2(_layoutGroupRectTransfrom.anchorMax.x, 0);
            _layoutGroupRectTransfrom.pivot = new Vector2(_layoutGroupRectTransfrom.pivot.x, 0);
        }
        else
        {
            _layoutGroupRectTransfrom.anchorMin = new Vector2(_layoutGroupRectTransfrom.anchorMin.x, 1);
            _layoutGroupRectTransfrom.anchorMax = new Vector2(_layoutGroupRectTransfrom.anchorMax.x, 1);
            _layoutGroupRectTransfrom.pivot = new Vector2(_layoutGroupRectTransfrom.pivot.x, 1);
        }
    }

    public void HideHint()
    {
        _hintTransform.gameObject.SetActive(false);

        foreach (var propertyView in _propertyViews)
        {
            _cardPropertyInfoViewFactory.RecyclePrefab(propertyView);
        }
    }
}

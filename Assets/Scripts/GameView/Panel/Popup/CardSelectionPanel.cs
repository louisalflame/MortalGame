using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public interface ICardSelectionPanel
{
    public record Property(
        bool IsMustSelect,
        bool IsConfirmable,
        int MaxSelectCount,
        IEnumerable<SelectionProperty> Selections);
    public record SelectionProperty(
        string Id,
        CardInfo CardInfo,
        Action<CardInfo, ICardView> OnClick,
        Action<CardInfo, ICardView> OnLongPress);
    public record UpdateProperty(
        bool IsVisible,
        bool IsConfirmable,
        int MaxSelectCount,
        IEnumerable<CardInfo> SelectedCards);

    void Init(IGameViewModel gameInfoModel, LocalizeLibrary localizeLibrary);
    void Open(Property property);
    void RenderUpdate(UpdateProperty property);
    void Close();

    Button VisibleToggleButton { get; }
    Button ConfirmButton { get; }
    Button[] CloseButtons { get; }
}

public class CardSelectionPanel : MonoBehaviour, ICardSelectionPanel
{
    [SerializeField]
    private GameObject _panel;
    [SerializeField]
    private Transform _cardViewParent;
    [SerializeField]
    private CardViewFactory _cardViewFactory;

    [SerializeField]
    private TextMeshProUGUI _selectionDescriptionText;

    [Header("Objects")]
    [SerializeField]
    private GameObject[] _visibleToggleObjects;
    [SerializeField]
    private GameObject _closeObject;

    [Header("Buttons")]
    [SerializeField]
    private Button[] _closeButtons;
    [SerializeField]
    private Button _visibleToggleButton;
    [SerializeField]
    private Button _confirmButton;

    [Header("LocalizeKey")]
    [SerializeField]
    private string _selectionInfoLocalizeKey;

    public Button VisibleToggleButton => _visibleToggleButton;
    public Button ConfirmButton => _confirmButton;
    public Button[] CloseButtons => _closeButtons;

    private IGameViewModel _gameViewModel;
    private LocalizeLibrary _localizeLibrary;
    private Dictionary<CardInfo, CardView> _cardViewMap = new();

    public void Init(IGameViewModel gameInfoModel, LocalizeLibrary localizeLibrary)
    {
        _gameViewModel = gameInfoModel;
        _localizeLibrary = localizeLibrary;
    }

    public void Open(ICardSelectionPanel.Property property)
    {
        _Clear();

        _panel.SetActive(true);
        foreach (var obj in _visibleToggleObjects)
        {
            obj.SetActive(true);
        }

        _closeObject.SetActive(!property.IsMustSelect);
        _confirmButton.interactable = property.IsConfirmable;

        _selectionDescriptionText.text = string.Format(
            _localizeLibrary.Get(LocalizeType.UI, _selectionInfoLocalizeKey).Title,
            0,
            property.MaxSelectCount);

        foreach (var selection in property.Selections)
        {
            var cardView = _cardViewFactory.CreatePrefab();
            cardView.transform.SetParent(_cardViewParent, false);
            cardView.Initialize(_gameViewModel, _localizeLibrary);
            cardView.Render(
                new ICardView.CardClickableProperty(
                    selection.CardInfo,
                    true,
                    selection.OnClick,
                    selection.OnLongPress));
            _cardViewMap.Add(selection.CardInfo, cardView);
        }
    }
    public void RenderUpdate(ICardSelectionPanel.UpdateProperty property)
    {
        foreach (var obj in _visibleToggleObjects)
        {
            obj.SetActive(property.IsVisible);
        }

        _confirmButton.interactable = property.IsConfirmable;

        _selectionDescriptionText.text = string.Format(
            _localizeLibrary.Get(LocalizeType.UI, _selectionInfoLocalizeKey).Title,
            property.SelectedCards.Count(),
            property.MaxSelectCount);
        
        foreach (var kvp in _cardViewMap)
        {
            if (property.SelectedCards.Contains(kvp.Key))
            {
                kvp.Value.OnSelect();
            }
            else
            {
                kvp.Value.OnDeselect();
            }
        }
    }

    public void Close()
    {
        _panel.SetActive(false);
        _Clear();
    }

    private void _Clear()
    {        
        foreach (var cardView in _cardViewMap.Values)
        {
            _cardViewFactory.RecyclePrefab(cardView);
        }
        _cardViewMap.Clear();
    }
}

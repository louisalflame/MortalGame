using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public interface ICardSelectionPanel
{
    public record Property(
        IEnumerable<SelectionProperty> Selections);
    public record SelectionProperty(
        string Id,
        CardInfo CardInfo);

    void Init(IGameViewModel gameInfoModel, LocalizeLibrary localizeLibrary);
    void Open();
    void Render(Property property);
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

    [Header("Buttons")]
    [SerializeField]
    private Button[] _closeButtons;
    [SerializeField]
    private Button _visibleToggleButton;
    [SerializeField]
    private Button _confirmButton;

    public Button VisibleToggleButton => _visibleToggleButton;
    public Button ConfirmButton => _confirmButton;
    public Button[] CloseButtons => _closeButtons;

    private IGameViewModel _gameViewModel;
    private LocalizeLibrary _localizeLibrary;
    private List<CardView> _cardViews = new();

    public void Init(IGameViewModel gameInfoModel, LocalizeLibrary localizeLibrary)
    {
        _gameViewModel = gameInfoModel;
        _localizeLibrary = localizeLibrary;
    }

    public void Render(ICardSelectionPanel.Property property)
    {
        _Clear();

        foreach (var selection in property.Selections)
        {
            var cardView = _cardViewFactory.CreatePrefab();
            cardView.transform.SetParent(_cardViewParent, false);
            cardView.Initialize(_gameViewModel, _localizeLibrary);
            cardView.SetCardInfo(selection.CardInfo);
            _cardViews.Add(cardView);
        }
    }

    public void Open()
    {
        _panel.SetActive(true);
    }
    public void Close()
    {
        _Clear();
        _panel.SetActive(false);
    }

    private void _Clear()
    {        
        foreach (var cardView in _cardViews)
        {
            _cardViewFactory.RecyclePrefab(cardView);
        }
        _cardViews.Clear();
    }
}

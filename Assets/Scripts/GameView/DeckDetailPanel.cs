using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DeckDetailPanel : MonoBehaviour
{
    [SerializeField]
    private Button[] _closeButtons;
    [SerializeField]
    private GameObject _panel;
    [SerializeField]
    private Transform _cardViewParent;
    [SerializeField]
    private CardViewFactory _cardViewFactory;

    private IGameplayStatusWatcher _statusWatcher;
    private Dictionary<Guid, CardView> _cardViewDict = new Dictionary<Guid, CardView>();

    public void Init(IGameplayStatusWatcher statusWatcher)
    {
        _statusWatcher = statusWatcher;
    }

    public async UniTask Run()
    {
        var isOpen = true;
        var disposables = new CompositeDisposable();
        foreach (var button in _closeButtons)
        {
            button.OnClickAsObservable()
                .Subscribe(_ => isOpen = false)
                .AddTo(disposables);
        }

        var cardInfos = _statusWatcher.GameStatus.Ally.CardManager.Deck.CardCollectionInfo.CardInfos.Keys;
        foreach (var cardInfo in cardInfos)
        {
            var cardView = _cardViewFactory.CreateCardView();
            cardView.transform.SetParent(_cardViewParent, false);
            cardView.SetCardInfo(cardInfo);
            _cardViewDict.Add(cardInfo.Indentity, cardView);
        }
        
        using (disposables)
        {
            _panel.SetActive(true);

            while (isOpen)
            {
                await UniTask.NextFrame();
            }

            _panel.SetActive(false);
        }

        foreach (var cardView in _cardViewDict.Values)
        {
            _cardViewFactory.RecycleCardView(cardView);
        }
        _cardViewDict.Clear();
    }
}

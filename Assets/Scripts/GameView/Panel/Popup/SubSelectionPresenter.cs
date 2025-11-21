using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;


public interface ISubSelectionPresenter
{
    UniTask<IReadOnlyDictionary<string, ISubSelectionAction>> RunSubSelection(SubSelectionInfo subSelectionInfoOpt);
}

public class SubSelectionPresenter : ISubSelectionPresenter
{
    private readonly ICardSelectionPanel _cardSelectionPanel;
    private readonly SingleCardDetailPopupPanel _singleCardDetailPopupPanel;
    private readonly IUniTaskPresenter<IReadOnlyList<ISubSelectionAction>> _uniTaskPresenter;

    public SubSelectionPresenter(
        IGameViewModel gameViewModel,
        LocalizeLibrary localizeLibrary,
        SingleCardDetailPopupPanel singleCardDetailPopupPanel,
        ICardSelectionPanel cardSelectionPanel)
    {
        _singleCardDetailPopupPanel = singleCardDetailPopupPanel;
        _singleCardDetailPopupPanel.Init(gameViewModel, localizeLibrary);
        _cardSelectionPanel = cardSelectionPanel;
        _cardSelectionPanel.Init(gameViewModel, localizeLibrary);

        _uniTaskPresenter = new UniTaskPresenter<IReadOnlyList<ISubSelectionAction>>();
    }

    public async UniTask<IReadOnlyDictionary<string, ISubSelectionAction>> RunSubSelection(SubSelectionInfo subSelectionInfo)
    {
        var subSelectionActions = new Dictionary<string, ISubSelectionAction>();

        foreach (var kvp in subSelectionInfo.SelectionInfos)
        {
            switch (kvp.Value)
            {
                case ExistCardSelectionInfo existCardSelection:
                    var subSelectionAction = await _RunExistCardSelection(existCardSelection);
                    subSelectionActions[kvp.Key] = subSelectionAction;
                    break;
            }
        }

        return subSelectionActions;
    }
    
    private async UniTask<ExistCardSubSelectionAction> _RunExistCardSelection(ExistCardSelectionInfo existCardSelection)
    {
        var isClose = false;
        var selectedCardIds = new List<Guid>();
        var disposables = new CompositeDisposable();

        if (!existCardSelection.IsMustSelect)
        {
            _cardSelectionPanel.CloseButtons
                .Select(button => button.OnClickAsObservable())
                .Merge()
                .Subscribe(_ => isClose = true)
                .AddTo(disposables);
        }
        _cardSelectionPanel.ConfirmButton
            .OnClickAsObservable()
            .Subscribe(_ => isClose = true)
            .AddTo(disposables); 
        _cardSelectionPanel.VisibleToggleButton
            .OnClickAsObservable()
            .Subscribe(_ => _cardSelectionPanel.ToggleVisible())
            .AddTo(disposables);

        _cardSelectionPanel.Open();

        _cardSelectionPanel.Render(new ICardSelectionPanel.Property(
            existCardSelection.CardInfos
                .Select(cardInfo => new ICardSelectionPanel.SelectionProperty(
                    cardInfo.Identity.ToString(),
                    cardInfo,
                    (cardInfo) =>
                    {
                        if (selectedCardIds.Contains(cardInfo.Identity))
                        {
                            selectedCardIds.Remove(cardInfo.Identity);
                        }
                        else if (selectedCardIds.Count < existCardSelection.Count)
                        {
                            selectedCardIds.Add(cardInfo.Identity);
                        }
                    },
                    (cardInfo) =>
                    {
                        _uniTaskPresenter.TryEnqueueTask(
                            _singleCardDetailPopupPanel.Run(CardDetailProperty.Create(cardInfo)));
                    }
                ))
        ));

        var selectionsOpt = await _uniTaskPresenter.Run(
            disposables,
            () => !isClose,
            CancellationToken.None);

        _cardSelectionPanel.Close();

        return new ExistCardSubSelectionAction(selectedCardIds);
    }
}

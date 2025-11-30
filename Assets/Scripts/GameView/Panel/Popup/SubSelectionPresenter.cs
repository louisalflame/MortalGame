using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        var isVisible = true;
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
            .Subscribe(_ => isClose = existCardSelection.IsMustSelect 
                ? selectedCardIds.Count >= existCardSelection.Count 
                : true)
            .AddTo(disposables); 
        _cardSelectionPanel.VisibleToggleButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                isVisible = !isVisible;
                _cardSelectionPanel.RenderUpdate(GetUpdateProperty());
            })
            .AddTo(disposables);

        _cardSelectionPanel.Open(CreateProperty());

        var selectionsOpt = await _uniTaskPresenter.Run(
            disposables,
            () => !isClose,
            CancellationToken.None);

        _cardSelectionPanel.Close();

        return new ExistCardSubSelectionAction(selectedCardIds);

        ICardSelectionPanel.SelectionProperty CreateSelectionProperty(CardInfo cardInfo)
        {
            return new ICardSelectionPanel.SelectionProperty(
                cardInfo.Identity.ToString(),
                cardInfo,
                (info, cardView) =>
                {
                    if (selectedCardIds.Contains(info.Identity))
                    {
                        selectedCardIds.Remove(info.Identity);
                    }
                    else if (selectedCardIds.Count < existCardSelection.Count)
                    {
                        selectedCardIds.Add(info.Identity);
                    }

                    _cardSelectionPanel.RenderUpdate(GetUpdateProperty());
                },
                (info, cardView) =>
                {
                    _uniTaskPresenter.TryEnqueueTask(
                        _singleCardDetailPopupPanel.Run(CardDetailProperty.Create(info)));
                }
            );
        }
        ICardSelectionPanel.Property CreateProperty()
        {
            return new ICardSelectionPanel.Property(
                existCardSelection.IsMustSelect,
                !existCardSelection.IsMustSelect,
                existCardSelection.Count,
                existCardSelection.CardInfos
                    .Select(cardInfo => CreateSelectionProperty(cardInfo))
            );
        }
        ICardSelectionPanel.UpdateProperty GetUpdateProperty()
        {
            return new ICardSelectionPanel.UpdateProperty(
                isVisible,
                existCardSelection.IsMustSelect
                    ? selectedCardIds.Count >= existCardSelection.Count
                    : true,
                existCardSelection.Count,
                existCardSelection.CardInfos
                    .Where(cardInfo => selectedCardIds.Contains(cardInfo.Identity))
            );
        }
    }
}

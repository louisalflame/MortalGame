using System;
using System.Collections.Generic;
using Optional;
using UnityEngine;

public interface IGameAction
{

}

public record UseCardAction(
    Guid CardIndentity,
    MainSelectableInfo MainSelectableInfo,
    MainSelectionAction MainSelectionAction) : IGameAction
{
    public static UseCardAction Create(CardInfo cardInfo)
    {
        return new UseCardAction(
            cardInfo.Identity,
            cardInfo.MainSelectable,
            MainSelectionAction.Empty);
    }

    public static UseCardAction Create(CardInfo cardInfo, ISelectionTarget selectionTarget)
    {
        return new UseCardAction(
            cardInfo.Identity,
            cardInfo.MainSelectable,
            MainSelectionAction.Create(selectionTarget));
    }
}

public record MainSelectionAction(
    TargetType TargetType,
    Option<Guid> SelectedTarget)
{
    public static MainSelectionAction Empty = new MainSelectionAction(TargetType.None, Option.None<Guid>());
    public static MainSelectionAction Create(ISelectionTarget selectionTarget)
    {
        return new MainSelectionAction(selectionTarget.TargetType, selectionTarget.TargetIdentity.Some());
    }
}

public interface ISubSelectionAction
{
}
public record ExistCardSubSelectionAction(
    IReadOnlyList<Guid> CardIdentity) : ISubSelectionAction;
public record NewCardSubSelectionAction() : ISubSelectionAction;
public record NewPartialCardSubSelectionAction() : ISubSelectionAction;
public record NewEffecrtSubSelectionAction() : ISubSelectionAction;

public class TurnSubmitAction : IGameAction
{
    public Faction Faction;
}
using System;
using System.Collections.Generic;
using Optional;
using UnityEngine;

public interface IGameAction
{

}

public class UseCardAction : IGameAction
{
    public Guid CardIndentity;

    public Option<Guid> SelectedTarget;
    public TargetType TargetType;

    public UseCardAction(Guid cardIndentity)
    {
        CardIndentity = cardIndentity;
        SelectedTarget = Option.None<Guid>();
        TargetType = TargetType.None;
    }

    public UseCardAction(Guid cardIndentity, TargetType targetType, Guid selectedTarget)
    {
        CardIndentity = cardIndentity;
        TargetType = targetType;
        SelectedTarget = selectedTarget.Some();
    }
}

public class TurnSubmitAction : IGameAction
{
    public Faction Faction;
}
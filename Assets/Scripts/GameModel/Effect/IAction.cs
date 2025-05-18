using Optional;
using UnityEngine;

public interface IIntentAction
{
    UpdateAction ActionType { get; }
    IActionSource Source { get; }
    IActionTarget Target { get; }
}

public interface IResultAction
{
    UpdateAction ActionType { get; }
    IActionSource Source { get; }
    IActionTarget Target { get; }
}
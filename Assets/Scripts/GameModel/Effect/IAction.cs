using Optional;
using UnityEngine;

public interface IActionUnit
{ 
}

public interface IIntentAction : IActionUnit
{
    UpdateAction ActionType { get; }
    IActionSource Source { get; }
    IActionTarget Target { get; }
}

public interface IResultAction : IActionUnit
{
    UpdateAction ActionType { get; }
    IActionSource Source { get; }
    IActionTarget Target { get; }
}
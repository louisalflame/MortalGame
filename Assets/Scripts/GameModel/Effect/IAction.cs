using Optional;
using UnityEngine;


public interface IIntentAction
{
    IActionSource Source { get; }
    IActionTarget Target { get; }
}

public interface IResultAction
{
    IActionSource Source { get; }
    IActionTarget Target { get; }
}
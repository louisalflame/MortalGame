using System;
using UnityEngine;

public interface ISelectableView
{
    RectTransform RectTransform { get; }

    TargetType TargetType { get; }
    Guid TargetIdentity { get; }

    void OnSelect();
    void OnDeselect();
}

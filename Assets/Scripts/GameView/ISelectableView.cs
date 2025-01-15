using UnityEngine;

public interface ISelectableView
{
    RectTransform RectTransform { get; }

    TargetType TargetType { get; }

    void OnSelect();
    void OnDeselect();
}

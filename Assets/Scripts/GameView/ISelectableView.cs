using UnityEngine;

public interface ISelectableView
{
    RectTransform RectTransform { get; }

    void  OnSelect();
}

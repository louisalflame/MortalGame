using UnityEngine;

public class PlaygroundView : MonoBehaviour, ISelectableView
{
    public RectTransform RectTransform => transform.GetComponent<RectTransform>();

    public void OnSelect()
    {
    }
}

using UnityEngine;

public class SimpleTitleIInfoHintView : MonoBehaviour
{
    public void ShowBuffInfo(BuffInfo buffInfo)
    {
        Debug.Log($"BuffInfo: {buffInfo}");
    }

    public void Close()
    {
        Debug.Log("Close");
    }
}

using UnityEngine;

public class BuffViewFactory : MonoBehaviour
{
    [SerializeField]
    private BuffView _buffViewPrefab;
    [SerializeField]
    private Transform _recycleRoot;

    public BuffView CreateBuffView()
    {
        return Instantiate(_buffViewPrefab, transform);
    }

    public void RecycleBuffView(BuffView buffView)
    {
        buffView.Reset();
        buffView.transform.SetParent(_recycleRoot);
    }
}

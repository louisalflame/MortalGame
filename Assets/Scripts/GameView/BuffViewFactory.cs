using UnityEngine;

public class BuffViewFactory : MonoBehaviour
{
    [SerializeField]
    private BuffView _buffViewPrefab;
    [SerializeField]
    private Transform _recycleRoot;

    public BuffView CreateCardView()
    {
        return Instantiate(_buffViewPrefab, transform);
    }

    public void RecycleCardView(BuffView buffView)
    {
        buffView.Reset();
        buffView.transform.SetParent(_recycleRoot);
    }
}

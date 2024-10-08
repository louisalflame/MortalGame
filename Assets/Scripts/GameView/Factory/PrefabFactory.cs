using UnityEngine;

public interface IRecyclable
{
    void Reset();
}

public class PrefabFactory<T> : MonoBehaviour where T : MonoBehaviour, IRecyclable
{
    [SerializeField]
    private T _prefab;
    [SerializeField]
    private Transform _recycleRoot;

    public T CreatePrefab()
    {
        return Instantiate(_prefab, transform);
    }

    public void RecyclePrefab(T view)
    {
        view.Reset();
        view.transform.SetParent(_recycleRoot);
    }
}

using System.Collections.Generic;
using UnityEngine;

public class BuffCollectionView : MonoBehaviour
{
    [SerializeField]
    private BuffViewFactory _buffViewFactory;

    [SerializeField]
    private Transform _buffViewParent;

    private List<BuffView> _buffViews = new List<BuffView>();
    private Dictionary<string, BuffView> _buffViewDict = new Dictionary<string, BuffView>();

    public void Init()
    {
    }

    public void AddBuff(BuffInfo buffInfo)
    {
        var buffView = _buffViewFactory.CreateBuffView();
        buffView.transform.SetParent(_buffViewParent);
        buffView.SetBuffInfo(buffInfo);

        _buffViews.Add(buffView);
        _buffViewDict.Add(buffInfo.BuffIdentity, buffView);
    }
}

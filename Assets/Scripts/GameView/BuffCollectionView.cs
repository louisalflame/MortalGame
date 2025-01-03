using System;
using System.Collections.Generic;
using UnityEngine;

public class BuffCollectionView : MonoBehaviour
{
    [SerializeField]
    private BuffViewFactory _buffViewFactory;

    [SerializeField]
    private Transform _buffViewParent;

    private List<BuffView> _buffViews = new List<BuffView>();
    private Dictionary<Guid, BuffView> _buffViewDict = new Dictionary<Guid, BuffView>();
    private SimpleTitleIInfoHintView _simpleHintView;

    public void Init(SimpleTitleIInfoHintView simpleHintView)
    {
        _simpleHintView = simpleHintView;
    }

    public void AddBuff(BuffInfo buffInfo)
    {
        var buffView = _buffViewFactory.CreatePrefab();
        buffView.transform.SetParent(_buffViewParent, false);
        buffView.SetBuffInfo(buffInfo, _simpleHintView);

        _buffViews.Add(buffView);
        _buffViewDict.Add(buffInfo.Identity, buffView);
    }
    public void RemoveBuff(BuffInfo buffInfo)
    {
        if (_buffViewDict.TryGetValue(buffInfo.Identity, out var buffView))
        {
            _buffViews.Remove(buffView);
            _buffViewDict.Remove(buffInfo.Identity);
            _buffViewFactory.RecyclePrefab(buffView);
        }
    }
    public void UpdateBuff(BuffInfo buffInfo)
    {
        if (_buffViewDict.TryGetValue(buffInfo.Identity, out var buffView))
        {
            buffView.SetBuffInfo(buffInfo, _simpleHintView);
        }
    }
}

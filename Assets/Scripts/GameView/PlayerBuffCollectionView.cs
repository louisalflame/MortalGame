using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffCollectionView : MonoBehaviour
{
    [SerializeField]
    private BuffViewFactory _buffViewFactory;

    [SerializeField]
    private Transform _buffViewParent;

    private List<PlayerBuffView> _buffViews = new List<PlayerBuffView>();
    private Dictionary<Guid, PlayerBuffView> _buffViewDict = new Dictionary<Guid, PlayerBuffView>();
    private SimpleTitleIInfoHintView _simpleHintView;

    public void Init(SimpleTitleIInfoHintView simpleHintView)
    {
        _simpleHintView = simpleHintView;
    }

    public void AddBuff(PlayerBuffInfo buffInfo)
    {
        var buffView = _buffViewFactory.CreatePrefab();
        buffView.transform.SetParent(_buffViewParent, false);
        buffView.SetBuffInfo(buffInfo, _simpleHintView);

        _buffViews.Add(buffView);
        _buffViewDict.Add(buffInfo.Identity, buffView);
    }
    public void RemoveBuff(PlayerBuffInfo buffInfo)
    {
        if (_buffViewDict.TryGetValue(buffInfo.Identity, out var buffView))
        {
            _buffViews.Remove(buffView);
            _buffViewDict.Remove(buffInfo.Identity);
            _buffViewFactory.RecyclePrefab(buffView);
        }
    }
    public void UpdateBuff(PlayerBuffInfo buffInfo)
    {
        if (_buffViewDict.TryGetValue(buffInfo.Identity, out var buffView))
        {
            buffView.SetBuffInfo(buffInfo, _simpleHintView);
        }
    }
}

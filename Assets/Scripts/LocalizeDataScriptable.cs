using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizeData", menuName = "Scriptable Objects/Localize Data")]
public class LocalizeDataScriptable : SerializedScriptableObject
{
    public Dictionary<string, string> Data =>
        DataList
            .GroupBy(d => d.Key)
            .ToDictionary(
                g => g.Key,
                g => g.First().Value);

    [ShowInInspector]
    [TableList(DrawScrollView = true)]
    public List<LocalizeTextData> DataList = new ();
}

[Serializable]
public class LocalizeTextData
{
    [HideLabel]
    [MultiLineProperty]
    [TableColumnWidth(200, resizable: false)]
    public string Key;

    [HideLabel]
    [MultiLineProperty]
    public string Value;
}
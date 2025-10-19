using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizeTitleInfoDataScriptable", menuName = "Scriptable Objects/LocalizeTitleInfoDataScriptable")]
public class LocalizeTitleInfoDataScriptable : SerializedScriptableObject
{
    public Dictionary<string, LocalizeTitleInfoData> Data =>
        DataList
            .GroupBy(d => d.Key)
            .ToDictionary(
                g => g.Key,
                g => new LocalizeTitleInfoData(g.First().Title, g.First().Info));


    [ShowInInspector]
    [TableList(DrawScrollView = true)]
    public List<LocalizeInfoData> DataList = new ();
}

[Serializable]
public class LocalizeInfoData
{
    [HideLabel]
    [MultiLineProperty]
    [TableColumnWidth(200, resizable: false)]
    public string Key;

    [HideLabel]
    [MultiLineProperty]
    [TableColumnWidth(200, resizable: false)]
    public string Title;

    [HideLabel]
    [MultiLineProperty]
    public string Info;
}

public record LocalizeTitleInfoData(string Title, string Info);
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizeTitleInfoDataScriptable", menuName = "Scriptable Objects/LocalizeTitleInfoDataScriptable")]
public class LocalizeTitleInfoDataScriptable : SerializedScriptableObject
{
    [ShowInInspector]
    [TableList]
    public Dictionary<string, LocalizeTitleInfoData> Data;
}

[Serializable]
public class LocalizeTitleInfoData
{
    [HideLabel]
    [HorizontalGroup(width: 0.25f, MinWidth = 100)]
            public string Title;

    [HideLabel]
    [HorizontalGroup()]
    [MultiLineProperty]
    public string Info;
}
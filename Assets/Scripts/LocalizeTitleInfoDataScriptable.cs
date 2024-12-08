using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizeTitleInfoDataScriptable", menuName = "Scriptable Objects/LocalizeTitleInfoDataScriptable")]
public class LocalizeTitleInfoDataScriptable : SerializedScriptableObject
{
    public Dictionary<string, LocalizeTitleInfoData> Data;
}

public class LocalizeTitleInfoData
{
    public string Title;
    public string Info;
}
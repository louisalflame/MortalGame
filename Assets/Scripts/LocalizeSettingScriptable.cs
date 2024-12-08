using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizeSetting", menuName = "Scriptable Objects/Localize Setting")]
public class LocalizeSettingScriptable : SerializedScriptableObject
{
    public Dictionary<LocalizeSimpleType, LocalizeDataScriptable> AllLocalizeSimpleData;
    public Dictionary<LocalizeTitleInfoType, LocalizeTitleInfoDataScriptable> AllLocalizeTileInfoData;
}
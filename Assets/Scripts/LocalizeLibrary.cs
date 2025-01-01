using System.Collections.Generic;
using UnityEngine;

public enum LocalizeSimpleType
{
    UI,
    PlayerName, 
}

public enum LocalizeTitleInfoType
{
    Card,
    CardStatus,
    Buff,
    Disposition,
    GameKeyWord,
}


public class LocalizeLibrary
{
    private Dictionary<LocalizeSimpleType, Dictionary<string, string>> _localizeSimpleDatas;
    private Dictionary<LocalizeTitleInfoType, Dictionary<string, LocalizeTitleInfoData>> _localizeTitleInfoDatas;

    public LocalizeLibrary(
        Dictionary<LocalizeSimpleType, Dictionary<string, string>> localizeDatas, 
        Dictionary<LocalizeTitleInfoType, Dictionary<string, LocalizeTitleInfoData>> localizeTitleInfoDatas)
    {
        _localizeSimpleDatas = localizeDatas;
        _localizeTitleInfoDatas = localizeTitleInfoDatas;
    }

    public string Get(LocalizeSimpleType localizeType, string key)
    {
        if (_localizeSimpleDatas.TryGetValue(localizeType, out var localizeData))
        {
            if (localizeData.TryGetValue(key, out var value))
            {
                return value;
            }
        }

        return $"{localizeType}|{key}";
    }

    public LocalizeTitleInfoData Get(LocalizeTitleInfoType localizeType, string key)
    {
        if (_localizeTitleInfoDatas.TryGetValue(localizeType, out var localizeData))
        {
            if (localizeData.TryGetValue(key, out var value))
            {
                return value;
            }
        }

        return new LocalizeTitleInfoData
        {
            Title = $"{localizeType}|{key}_Title",
            Info = $"{localizeType}|{key}_Info",
        };
    }
}

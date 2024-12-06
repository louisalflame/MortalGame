using System.Collections.Generic;
using UnityEngine;

public enum LocalizeType
{
    UI,
    PlayerName,
    CardTitle,
    CardInfo,
    CardPropertyTitle,
    CardPropertyInfo,
}

public class LocalizeLibrary
{
    private Dictionary<LocalizeType, LocalizeData> _localizeDatas;

    public LocalizeLibrary(Dictionary<LocalizeType, LocalizeData> localizeDatas)
    {
        _localizeDatas = localizeDatas;
    }

    public string Get(LocalizeType localizeType, string key)
    {
        if (_localizeDatas.TryGetValue(localizeType, out var localizeData))
        {
            if (localizeData.TryGetValue(key, out var value))
            {
                return value;
            }
        }

        return $"{localizeType}|{key}";
    }
}

using System.Collections.Generic;
using UnityEngine;

public enum LocalizeType
{
    Player,
    Card,
    CardBuff,
    PlayerBuff,
    KeyWord,
}

public record LocalizeTitleInfoData(string Title, string Info);

public class LocalizeLibrary
{
    private IReadOnlyDictionary<LocalizeType, IReadOnlyDictionary<string, LocalizeTitleInfoData>> _localizeTitleInfoDatas;

    public LocalizeLibrary(
        IReadOnlyDictionary<LocalizeType, IReadOnlyDictionary<string, LocalizeTitleInfoData>> localizeTitleInfoDatas)
    {
        _localizeTitleInfoDatas = localizeTitleInfoDatas;
    }

    public LocalizeTitleInfoData Get(LocalizeType localizeType, string key)
    {
        if (_localizeTitleInfoDatas.TryGetValue(localizeType, out var localizeData))
        {
            if (localizeData.TryGetValue(key, out var value))
            {
                return value;
            }
        }

        return new LocalizeTitleInfoData(
            $"{localizeType}|{key}_Title",
            $"{localizeType}|{key}_Info");
    }
}

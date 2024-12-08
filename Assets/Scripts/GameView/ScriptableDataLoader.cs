using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScriptableDataLoader : MonoBehaviour
{ 
    [SerializeField]
    private AllCardScriptable _allCardScriptable;
    [SerializeField]
    private AllCardStatusScriptable _allCardStatusScriptable;

    [SerializeField]
    private AllBuffScriptable _allBuffScriptable;

    [SerializeField]
    private AllPlayerScriptable _allPlayerScriptable;

    [SerializeField]
    private LocalizeSettingScriptable _localizeSettingScriptable;

    public CardData[] AllCards => _allCardScriptable.AllCardData.Select(c => c.Data).ToArray();
    public CardStatusData[] AllCardStatus => _allCardStatusScriptable.AllCardStatusData.Select(c => c.Data).ToArray();
    public BuffData[] AllBuffs => _allBuffScriptable.AllBuffData.Select(b => b.Data).ToArray();

    public AllyData Ally => _allPlayerScriptable.AllyObject.Ally;

    public EnemyData[] AllEnemies => _allPlayerScriptable.EnemyObjects.Select(p => p.Enemy).ToArray();

    public Dictionary<LocalizeSimpleType, Dictionary<string, string>> LocalizeSimpleSetting => _localizeSettingScriptable
        .AllLocalizeSimpleData
        .ToDictionary(pair => pair.Key, pair => pair.Value.Data);
    public Dictionary<LocalizeTitleInfoType, Dictionary<string, LocalizeTitleInfoData>> LocalizeTitleInfoSetting => _localizeSettingScriptable
        .AllLocalizeTileInfoData
        .ToDictionary(pair => pair.Key, pair => pair.Value.Data);
}

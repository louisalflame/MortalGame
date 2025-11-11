using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScriptableDataLoader : MonoBehaviour
{ 
    [SerializeField]
    private AllCardScriptable _allCardScriptable;
    [SerializeField]
    private AllCardBuffScriptable _allCardBuffScriptable;

    [SerializeField]
    private AllBuffScriptable _allBuffScriptable;

    [SerializeField]
    private AllPlayerScriptable _allPlayerScriptable;

    [SerializeField]
    private ExcelDatas _excelDatasScriptable;

    public CardData[] AllCards => _allCardScriptable.AllCardData.Select(c => c.Data).ToArray();
    public CardBuffData[] AllCardBuffs => _allCardBuffScriptable.AllCardBuffData.Select(c => c.Data).ToArray();
    public PlayerBuffData[] AllBuffs => _allBuffScriptable.AllBuffData.Select(b => b.Data).ToArray();
    public AllyData Ally => _allPlayerScriptable.AllyObject.Ally;
    public EnemyData[] AllEnemies => _allPlayerScriptable.EnemyObjects.Select(p => p.Enemy).ToArray();

    public DispositionData[] DispositionSettings()
    {
        return _excelDatasScriptable.Disposition
            .Select(row => new DispositionData(
                row.Id,
                row.Range,
                row.RecoverEnergyPoint,
                row.DrawCardCount
            ))
            .ToArray();
    }

    public IReadOnlyDictionary<LocalizeType, IReadOnlyDictionary<string, LocalizeTitleInfoData>> LocalizeTitleInfoSetting()
    {
        return new Dictionary<LocalizeType, IReadOnlyDictionary<string, LocalizeTitleInfoData>>
        {
            {
                LocalizeType.Player,
                ParseTable(_excelDatasScriptable.LocalizePlayer)
            },
            {
                LocalizeType.Card,
                ParseTable(_excelDatasScriptable.LocalizeCard)
            },
            {
                LocalizeType.CardBuff,
                ParseTable(_excelDatasScriptable.LocalizeCardBuff)
            },
            {
                LocalizeType.PlayerBuff,
                ParseTable(_excelDatasScriptable.LocalizePlayerBuff)
            },
            {
                LocalizeType.KeyWord,
                ParseTable(_excelDatasScriptable.LocalizeKeyWord)
            },
        };

        LocalizeTitleInfoData Parse(LocalizeExcelData data) =>
            new LocalizeTitleInfoData(data.Title, data.Info);
        Dictionary<string, LocalizeTitleInfoData> ParseTable(List<LocalizeExcelData> datas)
            => datas.ToDictionary(d => d.Id, d => Parse(d));
    }
}

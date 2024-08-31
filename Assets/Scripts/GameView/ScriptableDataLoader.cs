using System.Linq;
using UnityEngine;

public class ScriptableDataLoader : MonoBehaviour
{ 
    [SerializeField]
    private AllCardScriptable _allCardScriptable;

    [SerializeField]
    private AllBuffScriptable _allBuffScriptable;

    [SerializeField]
    private AllPlayerScriptable _allPlayerScriptable;

    public CardData[] AllCards => _allCardScriptable.AllCardData.Select(c => c.Data).ToArray();
    public BuffData[] AllBuffs => _allBuffScriptable.AllBuffData.Select(b => b.Data).ToArray();

    public AllyData Ally => _allPlayerScriptable.AllyObject.Ally;

    public EnemyData[] AllEnemies => _allPlayerScriptable.EnemyObjects.Select(p => p.Enemy).ToArray();
}

using UnityEngine;

public class Context
{
    public CardData[] AllCards { get; private set; }
    public BuffData[] AllBuffs { get; private set; }
    public AllyData Ally { get; private set; }
    public EnemyData[] AllEnemies { get; private set; }

    public Context(
        ScriptableDataLoader scriptableDataLoader)
    {
        AllCards = scriptableDataLoader.AllCards;
        AllBuffs = scriptableDataLoader.AllBuffs;
        Ally = scriptableDataLoader.Ally;
        AllEnemies = scriptableDataLoader.AllEnemies;
    }
}

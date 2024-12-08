using UnityEngine;

public interface ICardPropertyUseCountData
{
    ICardPropertyUseCountEntity CreateEntity();
}

public class CardPropertyDefaultUseData : ICardPropertyUseCountData
{
    public ICardPropertyUseCountEntity CreateEntity()
    {
        return new CardPropertyDefaultUseEntity();
    }
}

public class CardPropertyBattleCountData : ICardPropertyUseCountData
{
    public int BattleCount;

    public ICardPropertyUseCountEntity CreateEntity()
    {
        return new CardPropertyBattleCountEntity(BattleCount);
    }
}

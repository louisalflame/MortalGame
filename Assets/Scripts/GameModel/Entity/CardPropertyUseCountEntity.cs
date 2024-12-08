using UnityEngine;

public interface ICardPropertyUseCountEntity
{
    void UpdateTiming(GameContextManager contextManager, CardTiming timing);
}

public class CardPropertyDefaultUseEntity : ICardPropertyUseCountEntity
{
    public void UpdateTiming(GameContextManager contextManager, CardTiming timing)
    {
    }
}

public class CardPropertyBattleCountEntity : ICardPropertyUseCountEntity
{
    private int _battleCount;

    public CardPropertyBattleCountEntity(int battleCount)
    {
        _battleCount = battleCount;
    }

    public void UpdateTiming(GameContextManager contextManager, CardTiming timing)
    {
    }
}
using System.Collections.Generic;
using UnityEngine;

public interface IGameEvent
{

}

public class RecycleGraveyardEvent : IGameEvent
{
    public Faction Faction;
    public IReadOnlyCollection<CardInfo> DeckCardInfos;
    public IReadOnlyCollection<CardInfo> GraveyardCardInfos;
}
public class DrawCardEvent : IGameEvent
{
    public Faction Faction;
    public CardInfo NewCardInfo;
    public IReadOnlyCollection<CardInfo> DeckCardInfos;
    public IReadOnlyCollection<CardInfo> HandCardInfos;
}
public class EnemySelectCardEvent : IGameEvent
{
    public CardInfo SelectedCardInfo;
    public IReadOnlyCollection<CardInfo> SelectedCardInfos;
}
public class UsedCardEvent : IGameEvent
{
    public Faction Faction;
    public CardInfo UsedCardInfo;
    public IReadOnlyCollection<CardInfo> HandCardInfos;
    public IReadOnlyCollection<CardInfo> GraveyardCardInfos;
}
public class ConsumeEnergyEvent : IGameEvent
{
    public Faction Faction;
    public int Energy;
    public int DeltaEnergy;
    public int MaxEnergy;
}
public class GainEnergyEvent : IGameEvent
{
    public Faction Faction;
    public int Energy;
    public int DeltaEnergy;
    public int MaxEnergy;
}
public class RoundStartEvent : IGameEvent
{
    public int Round;
    public AllyEntity Player;
    public EnemyEntity Enemy;
}

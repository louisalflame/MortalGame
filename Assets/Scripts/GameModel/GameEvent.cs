using System.Collections.Generic;
using UnityEngine;

public interface IGameEvent
{

}

public class RoundStartEvent : IGameEvent
{
    public int Round;
    public AllyEntity Player;
    public EnemyEntity Enemy;
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
public class EnemyClearSelectedCardsEvent : IGameEvent
{ }
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

public class TakeDamageEvent : IGameEvent
{
    public Faction Faction;
    public int Hp;
    public int Shield;
    public int DeltaHp;
    public int DeltaShield;
    public int DamagePoint;
    public int MaxHp;
}

public class GetHealEvent : IGameEvent
{
    public Faction Faction;
    public int Hp;
    public int Shield;
    public int DeltaHp;
    public int HealPoint;
    public int MaxHp;
}
public class GetShieldEvent : IGameEvent
{
    public Faction Faction;
    public int Hp;
    public int Shield;
    public int DeltaShield;
    public int ShieldPoint;
    public int MaxHp;
}
using System.Collections.Generic;
using UnityEngine;

public interface IActionSource
{ 
}

public class SystemSource : IActionSource
{
    public SystemSource()
    {
    }
}

public class SystemExectueStartSource : IActionSource
{
    public readonly IPlayerEntity Player;
    public SystemExectueStartSource(IPlayerEntity player)
    {
        Player = player;
    }
}
public class SystemExectueEndSource : IActionSource
{
    public readonly IPlayerEntity Player;
    public SystemExectueEndSource(IPlayerEntity player)
    {
        Player = player;
    }
}

public class CardPlaySource : IActionSource
{
    public readonly ICardEntity Card;
    public readonly int HandCardIndex;
    public readonly int HandCardsCount;
    public readonly LoseEnergyResult CostEnergy;
    public readonly IEffectAttribute Attribute;

    public CardPlaySource(ICardEntity card, int handCardIndex, int handCardsCount, LoseEnergyResult costEnergy)
    {
        Card = card;
        HandCardIndex = handCardIndex;
        HandCardsCount = handCardsCount;
        CostEnergy = costEnergy;
        Attribute = new CardPlayAttributeEntity();
    }
}

public class PlayerBuffSource : IActionSource
{
    public readonly IPlayerBuffEntity Buff;

    public PlayerBuffSource(IPlayerBuffEntity buff)
    {
        Buff = buff;
    }
}

public class CardBuffSource : IActionSource
{
    public readonly ICardBuffEntity Buff;

    public CardBuffSource(ICardBuffEntity buff)
    {
        Buff = buff;
    }
}
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
    public IPlayerEntity Player { get; private set; }
    public SystemExectueStartSource(IPlayerEntity player)
    {
        Player = player;
    }
}
public class SystemExectueEndSource : IActionSource
{
    public IPlayerEntity Player { get; private set; }
    public SystemExectueEndSource(IPlayerEntity player)
    {
        Player = player;
    }
}

public class CardPlaySource : IActionSource
{
    public ICardEntity Card { get; private set; }
    public int HandCardIndex { get; private set; }
    public int HandCardsCount { get; private set; }
    public IEffectAttribute Attribute { get; private set; }

    public CardPlaySource(ICardEntity card, int handCardIndex, int handCardsCount)
    {
        Card = card;
        HandCardIndex = handCardIndex;
        HandCardsCount = handCardsCount;
        Attribute = new CardPlayAttributeEntity();
    }
}

public class PlayerBuffSource : IActionSource
{
    public IPlayerBuffEntity Buff { get; private set; }

    public PlayerBuffSource(IPlayerBuffEntity buff)
    {
        Buff = buff;
    }
}

public class CardBuffSource : IActionSource
{
    public ICardBuffEntity Buff { get; private set; }

    public CardBuffSource(ICardBuffEntity buff)
    {
        Buff = buff;
    }
}
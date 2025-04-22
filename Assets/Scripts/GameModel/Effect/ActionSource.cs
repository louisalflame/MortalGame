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

public record CardSource : IActionSource
{
    public ICardEntity Card { get; private set; }

    public CardSource(ICardEntity card)
    {
        Card = card;
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
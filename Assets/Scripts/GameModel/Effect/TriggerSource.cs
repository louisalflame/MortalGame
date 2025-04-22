using UnityEngine;

public interface ITriggerSource
{ 
}

public class CardPlay : ITriggerSource
{
    public ICardEntity Card { get; private set; }

    public CardPlay(ICardEntity card)
    {
        Card = card;
    }
}

public class CardBuffTrigger : ITriggerSource
{
    public ICardBuffEntity Buff { get; private set; }

    public CardBuffTrigger(ICardBuffEntity buff)
    {
        Buff = buff;
    }
}

public class PlayerBuffTrigger : ITriggerSource
{
    public IPlayerBuffEntity Buff { get; private set; }

    public PlayerBuffTrigger(IPlayerBuffEntity buff)
    {
        Buff = buff;
    }
}

public class CharacterBuffTrigger : ITriggerSource
{
    public ICharacterBuffEntity Buff { get; private set; }

    public CharacterBuffTrigger(ICharacterBuffEntity buff)
    {
        Buff = buff;
    }
}
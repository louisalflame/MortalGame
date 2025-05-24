using Unity.VisualScripting;
using UnityEngine;

public interface ITriggerSource
{
}

public class CardPlayTrigger : ITriggerSource
{
    public CardPlaySource CardPlay { get; private set; }

    public CardPlayTrigger(CardPlaySource cardPlay)
    {
        CardPlay = cardPlay;
    }
}

public class CardTrigger : ITriggerSource
{
    public ICardEntity Card { get; private set; }

    public CardTrigger(ICardEntity card)
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
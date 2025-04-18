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
    public ICardStatusEntity Buff { get; private set; }

    public CardBuffSource(ICardStatusEntity buff)
    {
        Buff = buff;
    }
}

public interface IActionTarget
{

}

public class SystemTarget : IActionTarget
{
    public SystemTarget()
    {
    }
}

public class PlayerTarget : IActionTarget
{
    public IPlayerEntity Player { get; private set; }

    public PlayerTarget(IPlayerEntity player)
    {
        Player = player;
    }
}

public class CharacterTarget : IActionTarget
{
    public ICharacterEntity Character { get; private set; }

    public CharacterTarget(ICharacterEntity character)
    {
        Character = character;
    }
}

public class CardTarget : IActionTarget
{
    public ICardEntity Card { get; private set; }

    public CardTarget(ICardEntity card)
    {
        Card = card;
    }
}

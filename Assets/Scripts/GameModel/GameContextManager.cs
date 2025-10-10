using System;
using System.Collections.Generic;

public interface IGameContextManager
{
    CardLibrary CardLibrary { get; }
    CardBuffLibrary CardBuffLibrary { get; }
    PlayerBuffLibrary BuffLibrary { get; }
    DispositionLibrary DispositionLibrary { get; }
    LocalizeLibrary LocalizeLibrary { get; }
    GameContext Context { get; }

    GameContextManager SetClone();
    GameContextManager SetSelectedPlayer(IPlayerEntity selectedPlayer);
    GameContextManager SetSelectedCharacter(ICharacterEntity selectedCharacter);
    GameContextManager SetSelectedCard(ICardEntity selectedCard);
}

public class GameContextManager : IDisposable, IGameContextManager
{
    private readonly CardLibrary _cardLibrary;
    private readonly CardBuffLibrary _cardBuffLibrary;
    private readonly PlayerBuffLibrary _buffLibrary;
    private readonly DispositionLibrary _dispositionLibrary;
    private readonly LocalizeLibrary _localizeLibrary;

    public CardLibrary CardLibrary => _cardLibrary;
    public CardBuffLibrary CardBuffLibrary => _cardBuffLibrary;
    public PlayerBuffLibrary BuffLibrary => _buffLibrary;
    public DispositionLibrary DispositionLibrary => _dispositionLibrary;
    public LocalizeLibrary LocalizeLibrary => _localizeLibrary;

    private Stack<GameContext> _contextStack = new Stack<GameContext>();
    public GameContext Context => _contextStack.Peek();

    public GameContextManager(
        CardLibrary cardLibrary,
        CardBuffLibrary cardBuffLibrary,
        PlayerBuffLibrary buffLibrary,
        DispositionLibrary dispositionLibrary,
        LocalizeLibrary localizeLibrary)
    {
        _cardLibrary = cardLibrary;
        _cardBuffLibrary = cardBuffLibrary;
        _buffLibrary = buffLibrary;
        _dispositionLibrary = dispositionLibrary;
        _localizeLibrary = localizeLibrary;
        _contextStack.Push(GameContext.EMPTY);
    }

    public void Dispose()
    {
        if (_contextStack.Count > 1)
        {
            _contextStack.Pop();
        }
    }

    public GameContextManager SetClone()
    {
        _contextStack.Push(Context with { });
        return this;
    }
    public GameContextManager SetSelectedPlayer(IPlayerEntity selectedPlayer)
    {
        _contextStack.Push(Context with { SelectedPlayer = selectedPlayer });
        return this;
    }
    public GameContextManager SetSelectedCharacter(ICharacterEntity selectedCharacter)
    {
        _contextStack.Push(Context with { SelectedCharacter = selectedCharacter });
        return this;
    }
    public GameContextManager SetSelectedCard(ICardEntity selectedCard)
    {
        _contextStack.Push(Context with { SelectedCard = selectedCard });
        return this;
    }
}

public record GameContext(
    IPlayerEntity SelectedPlayer,
    ICharacterEntity SelectedCharacter,
    ICardEntity SelectedCard)
{ 
    public static GameContext EMPTY => new(null, null, null);
}
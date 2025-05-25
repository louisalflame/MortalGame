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
        _contextStack.Push(new GameContext());
    }

    public void Dispose()
    {
        if (_contextStack.Count > 1)
        {
            var context = _contextStack.Pop();
            context.Dispose();
        }
    }

    public GameContextManager SetClone()
    {
        _contextStack.Push(Context.With());
        return this;
    }
    public GameContextManager SetSelectedPlayer(IPlayerEntity selectedPlayer)
    {
        _contextStack.Push(Context.With(selectedPlayer: selectedPlayer));
        return this;
    }
    public GameContextManager SetSelectedCharacter(ICharacterEntity selectedCharacter)
    {
        _contextStack.Push(Context.With(selectedCharacter: selectedCharacter));
        return this;
    }
    public GameContextManager SetSelectedCard(ICardEntity selectedCard)
    {
        _contextStack.Push(Context.With(selectedCard: selectedCard));
        return this;
    }
}

public class GameContext : IDisposable
{
    public IPlayerEntity        SelectedPlayer;
    public ICharacterEntity     SelectedCharacter;
    public ICardEntity          SelectedCard;
    
    public GameContext() { }
    public GameContext With(
        IPlayerEntity       selectedPlayer = null,
        ICharacterEntity    selectedCharacter = null,
        ICardEntity         selectedCard = null)
    {
        return new GameContext() 
        {
            SelectedPlayer          = selectedPlayer ?? SelectedPlayer,
            SelectedCharacter       = selectedCharacter ?? SelectedCharacter,
            SelectedCard            = selectedCard ?? SelectedCard
        };
    }

    public void Dispose() 
    {
        SelectedPlayer = null;
        SelectedCharacter = null;
        SelectedCard = null;
    }
}
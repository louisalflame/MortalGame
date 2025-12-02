using System;
using System.Collections.Generic;
using Optional;

public interface IGameContextManager
{
    CardLibrary CardLibrary { get; }
    CardBuffLibrary CardBuffLibrary { get; }
    PlayerBuffLibrary PlayerBuffLibrary { get; }
    CharacterBuffLibrary CharacterBuffLibrary { get; }
    DispositionLibrary DispositionLibrary { get; }
    LocalizeLibrary LocalizeLibrary { get; }

    GameContext Context { get; }

    GameContextManager SetClone();
    GameContextManager SetSelectedPlayer(Option<IPlayerEntity> selectedPlayer);
    GameContextManager SetSelectedCharacter(Option<ICharacterEntity> selectedCharacter);
    GameContextManager SetSelectedCard(Option<ICardEntity> selectedCard);
}

public class GameContextManager : IDisposable, IGameContextManager
{
    private readonly CardLibrary _cardLibrary;
    private readonly CardBuffLibrary _cardBuffLibrary;
    private readonly PlayerBuffLibrary _playerBuffLibrary;
    private readonly CharacterBuffLibrary _characterBuffLibrary;
    private readonly DispositionLibrary _dispositionLibrary;
    private readonly LocalizeLibrary _localizeLibrary;

    public CardLibrary CardLibrary => _cardLibrary;
    public CardBuffLibrary CardBuffLibrary => _cardBuffLibrary;
    public PlayerBuffLibrary PlayerBuffLibrary => _playerBuffLibrary;
    public CharacterBuffLibrary CharacterBuffLibrary => _characterBuffLibrary;
    public DispositionLibrary DispositionLibrary => _dispositionLibrary;
    public LocalizeLibrary LocalizeLibrary => _localizeLibrary;

    private Stack<GameContext> _contextStack = new Stack<GameContext>();
    public GameContext Context => _contextStack.Peek();

    public GameContextManager(
        CardLibrary cardLibrary,
        CardBuffLibrary cardBuffLibrary,
        PlayerBuffLibrary playerBuffLibrary,
        CharacterBuffLibrary characterBuffLibrary,
        DispositionLibrary dispositionLibrary,
        LocalizeLibrary localizeLibrary)
    {
        _cardLibrary = cardLibrary;
        _cardBuffLibrary = cardBuffLibrary;
        _playerBuffLibrary = playerBuffLibrary;
        _characterBuffLibrary = characterBuffLibrary;
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
    public GameContextManager SetSelectedPlayer(Option<IPlayerEntity> selectedPlayer)
    {
        return selectedPlayer.Match(
            some: player => {
                _contextStack.Push(Context with { SelectedPlayer = player });
                return this;
            },
            none: () => SetClone()
        );
    }
    public GameContextManager SetSelectedCharacter(Option<ICharacterEntity> selectedCharacter)
    {
        return selectedCharacter.Match(
            some: character => {
                _contextStack.Push(Context with { SelectedCharacter = character });
                return this;
            },
            none: () => SetClone()
        );
    }
    public GameContextManager SetSelectedCard(Option<ICardEntity> selectedCard)
    {
        return selectedCard.Match(
            some: card => {
                _contextStack.Push(Context with { SelectedCard = card });
                return this;
            },
            none: () => SetClone()
        );
    }
}

public record GameContext(
    IPlayerEntity SelectedPlayer,
    ICharacterEntity SelectedCharacter,
    ICardEntity SelectedCard)
{ 
    public static GameContext EMPTY => new(null, null, null);
}
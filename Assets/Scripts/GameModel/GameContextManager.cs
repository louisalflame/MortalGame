using System;
using System.Collections.Generic;

public class GameContextManager : IDisposable
{
    public readonly CardLibrary CardLibrary;
    public readonly CardBuffLibrary CardBuffLibrary;
    public readonly PlayerBuffLibrary BuffLibrary;
    public readonly DispositionLibrary DispositionLibrary;
    public readonly LocalizeLibrary LocalizeLibrary;
    
    private Stack<GameContext> _contextStack = new Stack<GameContext>();
    public GameContext Context => _contextStack.Peek();

    public GameContextManager(
        CardLibrary cardLibrary,
        CardBuffLibrary cardBuffLibrary,
        PlayerBuffLibrary buffLibrary,
        DispositionLibrary dispositionLibrary,
        LocalizeLibrary localizeLibrary)
    {
        CardLibrary = cardLibrary;
        CardBuffLibrary = cardBuffLibrary;
        BuffLibrary = buffLibrary;
        DispositionLibrary = dispositionLibrary;
        LocalizeLibrary = localizeLibrary;
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
using System;
using System.Collections.Generic;

public class GameContextManager : IDisposable
{
    public readonly CardLibrary CardLibrary;
    public readonly CardStatusLibrary CardStatusLibrary;
    public readonly PlayerBuffLibrary BuffLibrary;
    public readonly DispositionLibrary DispositionLibrary;
    public readonly LocalizeLibrary LocalizeLibrary;
    
    private Stack<GameContext> _contextStack = new Stack<GameContext>();
    public GameContext Context => _contextStack.Peek();

    public GameContextManager(
        CardLibrary cardLibrary,
        CardStatusLibrary cardStatusLibrary,
        PlayerBuffLibrary buffLibrary,
        DispositionLibrary dispositionLibrary,
        LocalizeLibrary localizeLibrary)
    {
        CardLibrary = cardLibrary;
        CardStatusLibrary = cardStatusLibrary;
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
    public GameContextManager SetExecutePlayer(IPlayerEntity executePlayer) 
    {
        _contextStack.Push(Context.With(executePlayer: executePlayer));
        return this;
    }
    public GameContextManager SetCardCaster(IPlayerEntity cardCaster) 
    {
        _contextStack.Push(Context.With(cardCaster: cardCaster));
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
    public GameContextManager SetUsingCard(ICardEntity usingCard) 
    {
        _contextStack.Push(Context.With(usingCard: usingCard));
        return this;
    }
    public GameContextManager SetGameTiming(GameTiming gameTiming) 
    {
        _contextStack.Push(Context.With(gameTiming: gameTiming));
        return this;
    }
    public GameContextManager SetEffectTargetPlayer(IPlayerEntity effectTarget) 
    {
        _contextStack.Push(Context.With(effectTargetPlayer: effectTarget));
        return this;
    }
    public GameContextManager SetEffectTargetCharacter(ICharacterEntity effectTarget) 
    {
        _contextStack.Push(Context.With(effectTargetCharacter: effectTarget));
        return this;
    }
    public GameContextManager SetEffectTargetCard(ICardEntity effectTarget) 
    {
        _contextStack.Push(Context.With(effectTargetCard: effectTarget));
        return this;
    }
    public GameContextManager SetTriggeredPlayerBuff(IPlayerBuffEntity triggeredBuff) 
    {
        _contextStack.Push(Context.With(triggeredBuff: triggeredBuff));
        return this;
    }
    public GameContextManager SetAction(IAction action) 
    {
        _contextStack.Push(Context.With(action: action));
        return this;
    }
}

public class GameContext : IDisposable
{
    public IPlayerEntity        ExecutePlayer;
    public IPlayerEntity        CardCaster;
    public IPlayerEntity        SelectedPlayer;
    public ICharacterEntity     SelectedCharacter;
    public ICardEntity          SelectedCard;
    
    public GameTiming           GameTiming;
    public ICardEntity          UsingCard;
    public IPlayerEntity        EffectTargetPlayer;
    public ICharacterEntity     EffectTargetCharacter;
    public ICardEntity          EffectTargetCard;
    public IPlayerBuffEntity    TriggeredBuff;
    public IAction              Action;

    public GameContext() { }
    public GameContext With(
        IPlayerEntity       executePlayer = null,
        IPlayerEntity       cardCaster = null,
        IPlayerEntity       selectedPlayer = null,
        ICharacterEntity    selectedCharacter = null,
        ICardEntity         selectedCard = null,
        GameTiming          gameTiming = default,
        ICardEntity         usingCard = null,
        IPlayerEntity       effectTargetPlayer = null,
        ICharacterEntity    effectTargetCharacter = null,
        ICardEntity         effectTargetCard = null,
        IPlayerBuffEntity   triggeredBuff = null,
        IAction             action = null)
    {
        return new GameContext() 
        {
            ExecutePlayer           = executePlayer ?? ExecutePlayer,
            CardCaster              = cardCaster ?? CardCaster,
            SelectedPlayer          = selectedPlayer ?? SelectedPlayer,
            SelectedCharacter       = selectedCharacter ?? SelectedCharacter,
            SelectedCard            = selectedCard ?? SelectedCard,
            UsingCard               = usingCard ?? UsingCard,
            GameTiming              = gameTiming == GameTiming.None ? GameTiming : gameTiming,
            EffectTargetPlayer      = effectTargetPlayer ?? EffectTargetPlayer,
            EffectTargetCharacter   = effectTargetCharacter ?? EffectTargetCharacter,
            EffectTargetCard        = effectTargetCard ?? EffectTargetCard,
            TriggeredBuff           = triggeredBuff ?? TriggeredBuff,
            Action                  = action ?? Action,
        };
    }

    public void Dispose() 
    {
        ExecutePlayer = null;
        CardCaster = null;
        SelectedPlayer = null;
        SelectedCharacter = null;
        SelectedCard = null;
        UsingCard = null;
        EffectTargetPlayer = null;
        EffectTargetCharacter = null;
        EffectTargetCard = null;
        TriggeredBuff = null;
    }
}
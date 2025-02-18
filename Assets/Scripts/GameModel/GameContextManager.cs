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
            _contextStack.Pop();
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
    public GameContextManager SetCardTiming(CardTiming cardTiming) 
    {
        _contextStack.Push(Context.With(cardTiming: cardTiming));
        return this;
    }
    public GameContextManager SetUsingCardEffect(ICardEffect usingCardEffect) 
    {
        _contextStack.Push(Context.With(usingCardEffect: usingCardEffect));
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
    public GameContextManager SetUsingPlayerBuff(PlayerBuffEntity usingBuff) 
    {
        _contextStack.Push(Context.With(usingBuff: usingBuff));
        return this;
    }
    public GameContextManager SetUsingPlayerBuffEffect(IPlayerBuffEffect usingBuffEffect) 
    {
        _contextStack.Push(Context.With(usingBuffEffect: usingBuffEffect));
        return this;
    }
}

public class GameContext
{
    public IPlayerEntity        ExecutePlayer;
    public IPlayerEntity        CardCaster;
    public IPlayerEntity        SelectedPlayer;
    public ICharacterEntity     SelectedCharacter;
    public ICardEntity          SelectedCard;
    public CardTiming           CardTiming;
    public ICardEntity          UsingCard;
    public ICardEffect          UsingCardEffect;
    public IPlayerEntity        EffectTargetPlayer;
    public ICharacterEntity     EffectTargetCharacter;
    public ICardEntity          EffectTargetCard;
    public PlayerBuffEntity           UsingBuff;
    public IPlayerBuffEffect          UsingBuffEffect;

    public GameContext() { }
    public GameContext With(
        IPlayerEntity       executePlayer = null,
        IPlayerEntity       cardCaster = null,
        IPlayerEntity       selectedPlayer = null,
        ICharacterEntity    selectedCharacter = null,
        ICardEntity         selectedCard = null,
        CardTiming          cardTiming = default,
        ICardEntity         usingCard = null,
        ICardEffect         usingCardEffect = null,
        IPlayerEntity       effectTargetPlayer = null,
        ICharacterEntity    effectTargetCharacter = null,
        ICardEntity         effectTargetCard = null,
        PlayerBuffEntity          usingBuff = null,
        IPlayerBuffEffect         usingBuffEffect = null)
    {
        return new GameContext() 
        {
            ExecutePlayer           = executePlayer ?? ExecutePlayer,
            CardCaster              = cardCaster ?? CardCaster,
            SelectedPlayer          = selectedPlayer ?? SelectedPlayer,
            SelectedCharacter       = selectedCharacter ?? SelectedCharacter,
            SelectedCard            = selectedCard ?? SelectedCard,
            UsingCard               = usingCard ?? UsingCard,
            CardTiming              = cardTiming == CardTiming.None ? CardTiming : cardTiming,
            UsingCardEffect         = usingCardEffect ?? UsingCardEffect,
            EffectTargetPlayer      = effectTargetPlayer ?? EffectTargetPlayer,
            EffectTargetCharacter   = effectTargetCharacter ?? EffectTargetCharacter,
            EffectTargetCard        = effectTargetCard ?? EffectTargetCard,
            UsingBuff               = usingBuff ?? UsingBuff,
            UsingBuffEffect         = usingBuffEffect ?? UsingBuffEffect
        };
    }
}
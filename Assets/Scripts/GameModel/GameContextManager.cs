using System;
using System.Collections.Generic;

public class GameContextManager : IDisposable
{
    public readonly CardLibrary CardLibrary;
    public readonly CardStatusLibrary CardStatusLibrary;
    public readonly BuffLibrary BuffLibrary;
    public readonly DispositionLibrary DispositionLibrary;
    public readonly LocalizeLibrary LocalizeLibrary;
    
    private Stack<GameContext> _contextStack = new Stack<GameContext>();
    public GameContext Context => _contextStack.Peek();

    public GameContextManager(
        CardLibrary cardLibrary,
        CardStatusLibrary cardStatusLibrary,
        BuffLibrary buffLibrary,
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

    public GameContextManager SetExecutePlayer(IPlayerEntity ExecutePlayer) 
    {
        _contextStack.Push(Context.With(executePlayer: ExecutePlayer));
        return this;
    }
    public GameContextManager SetCardCaster(IPlayerEntity CardCaster) 
    {
        _contextStack.Push(Context.With(cardCaster: CardCaster));
        return this;
    }
    public GameContextManager SetSelectedPlayer(IPlayerEntity SelectedPlayer) 
    {
        _contextStack.Push(Context.With(selectedPlayer: SelectedPlayer));
        return this;
    }
    public GameContextManager SetSelectedCard(ICardEntity SelectedCard) 
    {
        _contextStack.Push(Context.With(selectedCard: SelectedCard));
        return this;
    }
    public GameContextManager SetUsingCard(ICardEntity UsingCard) 
    {
        _contextStack.Push(Context.With(usingCard: UsingCard));
        return this;
    }
    public GameContextManager SetCardTiming(CardTiming CardTiming) 
    {
        _contextStack.Push(Context.With(cardTiming: CardTiming));
        return this;
    }
    public GameContextManager SetUsingCardEffect(ICardEffect UsingCardEffect) 
    {
        _contextStack.Push(Context.With(usingCardEffect: UsingCardEffect));
        return this;
    }
    public GameContextManager SetEffectTargetPlayer(IPlayerEntity EffectTarget) 
    {
        _contextStack.Push(Context.With(effectTargetPlayer: EffectTarget));
        return this;
    }
    public GameContextManager SetEffectTargetCard(ICardEntity EffectTarget) 
    {
        _contextStack.Push(Context.With(effectTargetCard: EffectTarget));
        return this;
    }
    public GameContextManager SetUsingBuff(BuffEntity UsingBuff) 
    {
        _contextStack.Push(Context.With(usingBuff: UsingBuff));
        return this;
    }
    public GameContextManager SetUsingBuffEffect(IBuffEffect UsingBuffEffect) 
    {
        _contextStack.Push(Context.With(usingBuffEffect: UsingBuffEffect));
        return this;
    }
}

public class GameContext
{
    public IPlayerEntity ExecutePlayer;
    public IPlayerEntity CardCaster;
    public IPlayerEntity SelectedPlayer;
    public ICardEntity SelectedCard;
    public CardTiming CardTiming;
    public ICardEntity UsingCard;
    public ICardEffect UsingCardEffect;
    public IPlayerEntity EffectTargetPlayer;
    public ICardEntity EffectTargetCard;
    public BuffEntity UsingBuff;
    public IBuffEffect UsingBuffEffect;

    public GameContext() { }
    public GameContext With(
        IPlayerEntity executePlayer = null,
        IPlayerEntity cardCaster = null,
        IPlayerEntity selectedPlayer = null,
        ICardEntity selectedCard = null,
        CardTiming cardTiming = default,
        ICardEntity usingCard = null,
        ICardEffect usingCardEffect = null,
        IPlayerEntity effectTargetPlayer = null,
        ICardEntity effectTargetCard = null,
        BuffEntity usingBuff = null,
        IBuffEffect usingBuffEffect = null)
    {
        return new GameContext() 
        {
            ExecutePlayer = executePlayer ?? ExecutePlayer,
            CardCaster = cardCaster ?? CardCaster,
            SelectedPlayer = selectedPlayer ?? SelectedPlayer,
            SelectedCard = selectedCard ?? SelectedCard,
            UsingCard = usingCard ?? UsingCard,
            CardTiming = cardTiming == CardTiming.None ? CardTiming : cardTiming,
            UsingCardEffect = usingCardEffect ?? UsingCardEffect,
            EffectTargetPlayer = effectTargetPlayer ?? EffectTargetPlayer,
            EffectTargetCard = effectTargetCard ?? EffectTargetCard,
            UsingBuff = usingBuff ?? UsingBuff,
            UsingBuffEffect = usingBuffEffect ?? UsingBuffEffect
        };
    }
}
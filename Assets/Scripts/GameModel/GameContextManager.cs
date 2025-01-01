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

    public GameContextManager SetExecutePlayer(PlayerEntity ExecutePlayer) 
    {
        _contextStack.Push(Context.With(executePlayer: ExecutePlayer));
        return this;
    }
    public GameContextManager SetCardCaster(PlayerEntity CardCaster) 
    {
        _contextStack.Push(Context.With(cardCaster: CardCaster));
        return this;
    }
    public GameContextManager SetSelectedPlayer(PlayerEntity SelectedPlayer) 
    {
        _contextStack.Push(Context.With(slectedPlayer: SelectedPlayer));
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
    public GameContextManager SetEffectTarget(PlayerEntity EffectTarget) 
    {
        _contextStack.Push(Context.With(effectTarget: EffectTarget));
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
    public PlayerEntity ExecutePlayer;
    public PlayerEntity CardCaster;
    public PlayerEntity SelectedPlayer;
    public ICardEntity SelectedCard;
    public CardTiming CardTiming;
    public ICardEntity UsingCard;
    public ICardEffect UsingCardEffect;
    public PlayerEntity EffectTarget;
    public BuffEntity UsingBuff;
    public IBuffEffect UsingBuffEffect;

    public GameContext() { }
    public GameContext With(
        PlayerEntity executePlayer = null,
        PlayerEntity cardCaster = null,
        PlayerEntity slectedPlayer = null,
        ICardEntity selectedCard = null,
        CardTiming cardTiming = default,
        ICardEntity usingCard = null,
        ICardEffect usingCardEffect = null,
        PlayerEntity effectTarget = null,
        BuffEntity usingBuff = null,
        IBuffEffect usingBuffEffect = null)
    {
        return new GameContext() 
        {
            ExecutePlayer = executePlayer ?? ExecutePlayer,
            CardCaster = cardCaster ?? CardCaster,
            SelectedPlayer = slectedPlayer ?? SelectedPlayer,
            SelectedCard = selectedCard ?? SelectedCard,
            UsingCard = usingCard ?? UsingCard,
            CardTiming = cardTiming == CardTiming.None ? CardTiming : cardTiming,
            UsingCardEffect = usingCardEffect ?? UsingCardEffect,
            EffectTarget = effectTarget ?? EffectTarget,
            UsingBuff = usingBuff ?? UsingBuff,
            UsingBuffEffect = usingBuffEffect ?? UsingBuffEffect
        };
    }
}
using System.Collections.Generic;

public class GameContextManager
{
    public CardLibrary CardLibrary;
    public BuffLibrary BuffLibrary;
    
    private Stack<GameContext> _contextStack = new Stack<GameContext>();
    public GameContext Context => _contextStack.Peek();

    public GameContextManager(
        CardLibrary cardLibrary,
        BuffLibrary buffLibrary)
    {
        CardLibrary = cardLibrary;
        BuffLibrary = buffLibrary;
        _contextStack.Push(new GameContext());
    }
    public GameContext Popout() 
    {
        return _contextStack.Pop();
    }

    public void SetCaster(PlayerEntity Caster) 
    {
        _contextStack.Push(Context.With(caster: Caster));
    }
    public void SetSelectedPlayer(PlayerEntity SelectedPlayer) 
    {
        _contextStack.Push(Context.With(slectedPlayer: SelectedPlayer));
    }
    public void SetSelectedCard(CardEntity SelectedCard) 
    {
        _contextStack.Push(Context.With(selectedCard: SelectedCard));
    }
    public void SetUsingCard(CardEntity UsingCard) 
    {
        _contextStack.Push(Context.With(usingCard: UsingCard));
    }
    public void SetCardTiming(CardTiming CardTiming) 
    {
        _contextStack.Push(Context.With(cardTiming: CardTiming));
    }
    public void SetUsingEffect(ICardEffect UsingEffect) 
    {
        _contextStack.Push(Context.With(UsingEffect: UsingEffect));
    }
    public void SetEffectTarget(PlayerEntity EffectTarget) 
    {
        _contextStack.Push(Context.With(EffectTarget: EffectTarget));
    }
}

public class GameContext
{
    public PlayerEntity Caster;
    public PlayerEntity SelectedPlayer;
    public CardEntity SelectedCard;
    public CardTiming CardTiming;
    public CardEntity UsingCard;
    public ICardEffect UsingEffect;
    public PlayerEntity EffectTarget;

    public GameContext() { }
    public GameContext With(
        PlayerEntity caster = null,
        PlayerEntity slectedPlayer = null,
        CardEntity selectedCard = null,
        CardTiming cardTiming = default,
        CardEntity usingCard = null,
        ICardEffect UsingEffect = null,
        PlayerEntity EffectTarget = null)
    {
        return new GameContext() 
        {
            Caster = caster ?? Caster,
            SelectedPlayer = slectedPlayer ?? SelectedPlayer,
            SelectedCard = selectedCard ?? SelectedCard,
            UsingCard = usingCard ?? UsingCard,
            CardTiming = cardTiming == CardTiming.None ? CardTiming : cardTiming,
            UsingEffect = UsingEffect ?? UsingEffect,
            EffectTarget = EffectTarget ?? EffectTarget
        };
    }
}
using System;

public interface ISessionValue
{
    void Update(GameStatus gameStatus, GameContext context);
}

[Serializable]
public class SessionBoolean : ISessionValue
{
    public bool Value;

    // condition
    // set
    //  -- overwrite
    //  -- and
    //  -- or
    //  -- not
    
    public void Update(GameStatus gameStatus, GameContext context)
    {}
}

[Serializable]
public class SessionInteger : ISessionValue
{
    public int Value;

    // condition
    // set
    //  -- overwrite
    //  -- add
    //  -- multiply
    //  -- divide
    //  -- mod
    
    public void Update(GameStatus gameStatus, GameContext context)
    {}
}

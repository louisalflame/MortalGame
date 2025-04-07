using System;

public interface ISessionValueData
{
    ISessionValueEntity GetEntity(IGameplayStatusWatcher gameWatcher);
}

[Serializable]
public class SessionBoolean : ISessionValueData
{
    public bool Value;

    // condition -> TODO: should lookup library 
    // set
    //  -- overwrite
    //  -- and
    //  -- or
    //  -- not
    
    public ISessionValueEntity GetEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new SessionBooleanEntity(Value, gameWatcher);
    }
}

[Serializable]
public class SessionInteger : ISessionValueData
{
    public int Value;

    // condition
    // set
    //  -- overwrite
    //  -- add
    //  -- multiply
    //  -- divide
    //  -- mod
    
    public ISessionValueEntity GetEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new SessionIntegerEntity(Value, gameWatcher);
    }
}

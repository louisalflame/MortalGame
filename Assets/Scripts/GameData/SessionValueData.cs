using System;
using Sirenix.OdinInspector;

public interface ISessionValueData
{
    ISessionValueEntity GetEntity(IGameplayStatusWatcher gameWatcher);
}


[Serializable]
public class SessionBoolean : ISessionValueData
{
    public bool Value;

    [ShowInInspector]
    public BooleanUpdateRules UpdateRules = new();
    
    public ISessionValueEntity GetEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new SessionBooleanEntity(Value, UpdateRules);
    }
}

[Serializable]
public class SessionInteger : ISessionValueData
{
    public int Value;

    [ShowInInspector]
    public IntegerUpdateRules UpdateRules = new();
    
    public ISessionValueEntity GetEntity(IGameplayStatusWatcher gameWatcher)
    {
        return new SessionIntegerEntity(Value, UpdateRules);
    }
}

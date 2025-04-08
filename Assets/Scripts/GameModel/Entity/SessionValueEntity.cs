using System;

public interface ISessionValueEntity
{
    void Update(IGameplayStatusWatcher gameWatcher, ISessionValueData valueData);
}

[Serializable]
public class SessionBooleanEntity : ISessionValueEntity
{
    public bool Value;
    
    public SessionBooleanEntity(bool value, IGameplayStatusWatcher gameWatcher)
    {
        Value = value;
    }

    public void Update(IGameplayStatusWatcher gameWatcher, ISessionValueData valueData)
    {

    }
}

[Serializable]
public class SessionIntegerEntity : ISessionValueEntity
{
    public int Value;

    public SessionIntegerEntity(int value, IGameplayStatusWatcher gameWatcher)
    {
        Value = value;
    }
    
    public void Update(IGameplayStatusWatcher gameWatcher, ISessionValueData valueData)
    {

    }
}

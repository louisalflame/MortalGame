using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ICardPropertyData
{
    ICardPropertyEntity CreateEntity();
}

public class PreservedPropertyData : ICardPropertyData
{
    public ICardPropertyEntity CreateEntity()
    {
        return new PreservedPropertyEntity();
    }
}

public class InitialPriorityPropertyData : ICardPropertyData
{
    public ICardPropertyEntity CreateEntity()
    {
        return new InitialPriorityPropertyEntity();
    }
}

public class ConsumablePropertyData : ICardPropertyData
{
    public ICardPropertyEntity CreateEntity()
    {
        return new ConsumablePropertyEntity();
    }
}

public class DisposePropertyData : ICardPropertyData
{
    public ICardPropertyEntity CreateEntity()
    {
        return new DisposePropertyEntity();
    }
}

public class AutoDisposePropertyData : ICardPropertyData
{
    public ICardPropertyEntity CreateEntity()
    {
        return new AutoDisposePropertyEntity();
    }
}

public class SealedPropertyData : ICardPropertyData
{
    public ICardPropertyEntity CreateEntity()
    {
        return new SealedPropertyEntity();
    }
}
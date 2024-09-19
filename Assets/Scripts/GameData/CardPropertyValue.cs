using UnityEngine;

public interface ICardPropertyValue
{
}

public class NoneValue : ICardPropertyValue
{
}

public class AddValue : ICardPropertyValue
{
    public int Value;
}

public class OverwriteValue : ICardPropertyValue
{
    public int Value;
}
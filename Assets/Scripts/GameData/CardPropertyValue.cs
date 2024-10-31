using UnityEngine;

public interface ICardPropertyValue
{
    int Eval(int originValue);
}

public class NoneValue : ICardPropertyValue
{
    public int Eval(int originValue)
    {
        return originValue;
    }
}

public class ArithmeticValue : ICardPropertyValue
{
    public ArithmeticType Type;
    public int Value;

    public int Eval(int originValue)
    {
        switch(Type)
        {
            case ArithmeticType.Add:
                return originValue + Value;
            case ArithmeticType.Multiply:
                return originValue * Value;
            default:
                return originValue;
        }
    }
}
using UnityEngine;


public interface ICardEffect
{

}

public class DamageEffect : ICardEffect
{
    public int Value;
}

public class ShieldEffect : ICardEffect
{
    public int Value;
}
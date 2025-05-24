using UnityEngine;

public interface IEffectAttribute
{
    int Power { get; set; }
    int NormalDamageAddition { get; set; }
    int NormalDamageRatio { get; set; }
}

public class CardPlayAttributeEntity : IEffectAttribute
{
    public int Power { get; set; } = 0;
    public int NormalDamageAddition { get; set; } = 0;
    public int NormalDamageRatio { get; set; } = 0;

    public CardPlayAttributeEntity()
    {
    }
}

using UnityEngine;

public interface IEffectAttribute
{
    int PowerAddition { get; set; }
    int PowerRatio { get; set; }
    int NormalDamageAddition { get; set; }
    int NormalDamageRatio { get; set; }
}

public class CardPlayAttributeEntity : IEffectAttribute
{
    public int PowerAddition { get; set; } = 0;
    public int PowerRatio { get; set; } = 0;
    public int NormalDamageAddition { get; set; } = 0;
    public int NormalDamageRatio { get; set; } = 0;

    public CardPlayAttributeEntity()
    {
    }
}

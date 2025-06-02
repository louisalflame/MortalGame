using UnityEngine;

public interface IEffectAttribute
{
    int PowerAddition { get; }
    float PowerRatio { get; }
    int NormalDamageAddition { get; }
    float NormalDamageRatio { get; }

    void ApplyModify(EffectAttributeType type, float value);
}

public class CardPlayAttributeEntity : IEffectAttribute
{
    public int PowerAddition { get; private set; } = 0;
    public float PowerRatio { get; private set; } = 0f;
    public int NormalDamageAddition { get; private set; } = 0;
    public float NormalDamageRatio { get; private set; } = 0f;

    public CardPlayAttributeEntity()
    {
    }

    public void ApplyModify(EffectAttributeType type, float value)
    {
        switch (type)
        {
            case EffectAttributeType.PowerAddition:
                PowerAddition += (int)value;
                break;
            case EffectAttributeType.PowerRatio:
                PowerRatio += value;
                break;
            case EffectAttributeType.NormalDamageAddition:
                NormalDamageAddition += (int)value;
                break;
            case EffectAttributeType.NormalDamageRatio:
                NormalDamageRatio += value;
                break;
            default:
                break;
        }
    }
}

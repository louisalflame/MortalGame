using UnityEngine;

public interface IEffectAttribute
{
    int PowerAddition { get; }
    int PowerRatio { get; }
    int NormalDamageAddition { get; }
    int NormalDamageRatio { get; }

    void ApplyModify(EffectAttributeType type, int value);
}

public class CardPlayAttributeEntity : IEffectAttribute
{
    public int PowerAddition { get; private set; } = 0;
    public int PowerRatio { get; private set; } = 0;
    public int NormalDamageAddition { get; private set; } = 0;
    public int NormalDamageRatio { get; private set; } = 0;

    public CardPlayAttributeEntity()
    {
    }

    public void ApplyModify(EffectAttributeType type, int value)
    {
        switch (type)
        {
            case EffectAttributeType.PowerAddition:
                PowerAddition += value;
                break;
            case EffectAttributeType.PowerRatio:
                PowerRatio += value;
                break;
            case EffectAttributeType.NormalDamageAddition:
                NormalDamageAddition += value;
                break;
            case EffectAttributeType.NormalDamageRatio:
                NormalDamageRatio += value;
                break;
            default:
                break;
        }
    }
}

using Sirenix.OdinInspector;

public class DispositionData
{
    [TitleGroup("BasicData")]
    public string ID;
    [TitleGroup("BasicData")]
    public int Range;

    [BoxGroup("Effect")]
    public int RecoverEnergyPoint;
    [BoxGroup("Effect")]
    public int DrawCardCount;
}
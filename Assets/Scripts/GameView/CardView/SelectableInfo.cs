using UnityEngine;

public class MainSelectableInfo
{
    public TargetType TargetType { get; private set; }   

    public MainSelectableInfo(TargetType targetType)
    {
        TargetType = targetType;
    }
}

public class SubSelectableInfo
{
    public TargetType TargetType { get; private set; }   
    public int TargetCount { get; private set; }

    public SubSelectableInfo(TargetType targetType, int targetCount)
    {
        TargetType = targetType;
        TargetCount = targetCount;
    }
}
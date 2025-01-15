using UnityEngine;

public class MainSelectableInfo
{
    public SelectType SelectType { get; private set; }   

    public MainSelectableInfo(SelectType selectType)
    {
        SelectType = selectType;
    }
}

public class SubSelectableInfo
{
    public SelectType SelectType { get; private set; }   
    public int SelectCount { get; private set; }

    public SubSelectableInfo(SelectType selectType, int selectCount)
    {
        SelectType = selectType;
        SelectCount = selectCount;
    }
}

public static class SelectableInfoUtility
{
    public static bool IsSelectable(this SelectType selectType, TargetType targetType)
    {
        switch(selectType)
        {
            case SelectType.None:
                return false;
            case SelectType.Player:
                return targetType == TargetType.Ally || 
                       targetType == TargetType.Enemy;
            case SelectType.Ally:
                return targetType == TargetType.Ally;
            case SelectType.Enemy:  
                return targetType == TargetType.Enemy;
            case SelectType.Card:
                return targetType == TargetType.AllyCard || 
                       targetType == TargetType.EnemyCard;
            case SelectType.AllyCard:   
                return targetType == TargetType.AllyCard;
            case SelectType.EnemyCard:  
                return targetType == TargetType.EnemyCard;
            default:    
                return false;        
        }
    }
}
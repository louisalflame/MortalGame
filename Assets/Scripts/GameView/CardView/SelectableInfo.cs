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
            case SelectType.Character:
                return targetType == TargetType.AllyCharacter || 
                       targetType == TargetType.EnemyCharacter;
            case SelectType.AllyCharacter:
                return targetType == TargetType.AllyCharacter;
            case SelectType.EnemyCharacter:  
                return targetType == TargetType.EnemyCharacter;
            case SelectType.Card:
                return targetType == TargetType.AllyCard || 
                       targetType == TargetType.EnemyCard;
            case SelectType.AllyCard:   
                return targetType == TargetType.AllyCard;
            case SelectType.EnemyCard:  
                return targetType == TargetType.EnemyCard;
            case SelectType.None:
            default:    
                return false;        
        }
    }
}
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
            case SelectType.Character:
            case SelectType.AllyCharacter:
            case SelectType.EnemyCharacter:  
                return targetType == TargetType.Character;
            case SelectType.Card:
            case SelectType.AllyCard:   
            case SelectType.EnemyCard:  
                return targetType == TargetType.Card;
            default:    
                return false;        
        }
    }
}
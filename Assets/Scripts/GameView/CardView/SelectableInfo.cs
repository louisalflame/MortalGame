using UnityEngine;

public record MainSelectableInfo(
    SelectType SelectType,
    TargetLogicTag LogicTag);
public record SubSelectableInfo(
    SelectType SelectType,
    int SelectCount,
    TargetLogicTag LogicTag);

public static class SelectableInfoUtility
{
    public static MainSelectableInfo ToInfo(this MainTargetSelectLogic mainTargetLogic)
    {
        return new MainSelectableInfo(
            mainTargetLogic.MainSelectable.SelectType, mainTargetLogic.LogicTag);
    } 

    public static SubSelectableInfo ToInfo(this SubTargetSelectLogic subTargetLogic)
    {
        return new SubSelectableInfo(
            subTargetLogic.SubSelectable.SelectType, subTargetLogic.SubSelectable.TargetCount, subTargetLogic.LogicTag);
    }

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
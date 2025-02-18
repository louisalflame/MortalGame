using UnityEngine;
 
public interface ITargetSelectable
{
    SelectType SelectType { get; }
}
public interface IMainTargetSelectable : ITargetSelectable
{ 
}
public interface ISubTargetSelectable : ITargetSelectable
{
    int TargetCount { get; }
}

public class NoneSelectable : IMainTargetSelectable
{
    public SelectType SelectType => SelectType.None;
}

public class CharacterSelectable : IMainTargetSelectable
{
    public SelectType SelectType => SelectType.Character;
}

public class CharacterAllySelectable : IMainTargetSelectable
{
    public SelectType SelectType => SelectType.AllyCharacter;
}

public class CharacterEnemySelectable : IMainTargetSelectable
{
    public SelectType SelectType => SelectType.EnemyCharacter;
}

public class CardSelectable : IMainTargetSelectable 
{
    public SelectType SelectType => SelectType.Card;
}

public class CardAllySelectable : IMainTargetSelectable
{
    public SelectType SelectType => SelectType.AllyCard;
}

public class CardEnemySelectable : IMainTargetSelectable
{
    public SelectType SelectType => SelectType.EnemyCard;
}

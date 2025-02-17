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

public class PlayerSelectable : IMainTargetSelectable
{
    public SelectType SelectType => SelectType.Character;
}

public class AllySelectable : IMainTargetSelectable
{
    public SelectType SelectType => SelectType.AllyCharacter;
}

public class EnemySelectable : IMainTargetSelectable
{
    public SelectType SelectType => SelectType.EnemyCharacter;
}

public class CardSelectable : IMainTargetSelectable 
{
    public SelectType SelectType => SelectType.Card;
}

public class AllyCardSelectable : IMainTargetSelectable
{
    public SelectType SelectType => SelectType.AllyCard;
}

public class EnemyCardSelectable : IMainTargetSelectable
{
    public SelectType SelectType => SelectType.EnemyCard;
}

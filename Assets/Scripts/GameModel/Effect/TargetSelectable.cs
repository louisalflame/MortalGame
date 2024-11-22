using UnityEngine;
 
public interface ITargetSelectable
{
    TargetType TargetType { get; }
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
    public TargetType TargetType => TargetType.None;
}

public class PlayerSelectable : IMainTargetSelectable
{
    public TargetType TargetType => TargetType.Player;
}

public class CardSelectable : IMainTargetSelectable 
{
    public TargetType TargetType => TargetType.Card;
}


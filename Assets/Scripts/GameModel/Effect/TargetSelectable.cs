using UnityEngine;
 
public interface ITargetSelectable
{
    TargetType TargetType { get; }
}

public class PlayerSelectable : ITargetSelectable
{
    public TargetType TargetType => TargetType.Player;
}

public class CardSelectable : ITargetSelectable 
{
    public TargetType TargetType => TargetType.Card;
}

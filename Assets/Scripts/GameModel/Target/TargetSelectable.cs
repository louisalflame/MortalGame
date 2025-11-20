 
public interface IMainTargetSelectable
{ 
    SelectType SelectType { get; }
}

public class NoneSelectable : IMainTargetSelectable
{
    public SelectType SelectType => SelectType.None;
    public int TargetCount => 0;
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




public interface ISubSelectionGroup
{
}

public class ExistCardSelectionGroup : ISubSelectionGroup
{
}

public record NewCardSelectionGroup() : ISubSelectionGroup
{
}

public record NewPartialCardSelectionGroup() : ISubSelectionGroup
{
}

public record NewEffectSelectionGroup() : ISubSelectionGroup
{
}


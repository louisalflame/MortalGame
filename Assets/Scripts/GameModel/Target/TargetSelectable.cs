
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public interface IMainTargetSelectable
{ 
    SelectType SelectType { get; }
}

[Serializable]
public class NoneSelectable : IMainTargetSelectable
{
    public SelectType SelectType => SelectType.None;
    public int TargetCount => 0;
}

[Serializable]
public class CharacterSelectable : IMainTargetSelectable
{
    public SelectType SelectType => SelectType.Character;
}

[Serializable]
public class CharacterAllySelectable : IMainTargetSelectable
{
    public SelectType SelectType => SelectType.AllyCharacter;
}

[Serializable]
public class CharacterEnemySelectable : IMainTargetSelectable
{
    public SelectType SelectType => SelectType.EnemyCharacter;
}

[Serializable]
public class CardSelectable : IMainTargetSelectable 
{
    public SelectType SelectType => SelectType.Card;
}

[Serializable]
public class CardAllySelectable : IMainTargetSelectable
{
    public SelectType SelectType => SelectType.AllyCard;
}

[Serializable]
public class CardEnemySelectable : IMainTargetSelectable
{
    public SelectType SelectType => SelectType.EnemyCard;
}

public interface ISubSelectionGroup
{
}

[Serializable]
public class ExistCardSelectionGroup : ISubSelectionGroup
{
    public ITargetCardCollectionValue CardCandidates;

    public IIntegerValue SelectCount;

    public IBooleanValue AllowLowerSelection;
}

[Serializable]
public record NewCardSelectionGroup() : ISubSelectionGroup
{
}

[Serializable]
public record NewPartialCardSelectionGroup() : ISubSelectionGroup
{
}

[Serializable]
public record NewEffectSelectionGroup() : ISubSelectionGroup
{
}


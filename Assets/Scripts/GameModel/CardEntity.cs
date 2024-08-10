using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CardEntity
{
    public string Indentity;
    public string Title;
    public string Info;

    public CardType Type;
    public int Cost;
    public int Power;

    public IReadOnlyCollection<ICardEffect> Effects;
}


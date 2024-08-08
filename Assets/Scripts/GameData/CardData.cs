using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class CardData
{
    [BoxGroup("Identification")]
    public string ID;

    [TitleGroup("BasicData")]
    public CardRarity Rarity;
    [TitleGroup("BasicData")]
    public CardType Type;
    [TitleGroup("BasicData")]
    [Range(0, 10)]
    public int Cost;
    [TitleGroup("BasicData")]
    [Range(0, 20)]
    public int Power;

    [BoxGroup("Effects")]
    public ICardEffect[] OnUseEffects;
    [BoxGroup("Effects")]
    public ICardEffect[] OnHandEffects;
    [BoxGroup("Effects")]
    public ICardEffect[] OnDeckEffects;

    [TitleGroup("Localization")]
    public string TitleKey;
}
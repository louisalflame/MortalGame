using System;
using System.Collections.Generic;
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
    public CardTheme[] Themes;
    [TitleGroup("BasicData")]
    [Range(0, 10)]
    public int Cost;
    [TitleGroup("BasicData")]
    [Range(0, 20)]
    public int Power;
    
    [TitleGroup("Target")]
    public TargetType TargetType = TargetType.Player;
    [TitleGroup("Target")]
    public ITargetCardValue TargetCard = new NoneCard();
    [TitleGroup("Target")]
    public ITargetPlayerValue TargetPlayer = new NonePlayer();


    [BoxGroup("Effects")]
    public ICardEffect[] OnUseEffects;
    [BoxGroup("Effects")]
    public ICardEffect[] OnHandEffects;
    [BoxGroup("Effects")]
    public ICardEffect[] OnDeckEffects;

    [TitleGroup("Localization")]
    public string TitleKey = string.Empty;
    public string InfoKey = string.Empty;
}